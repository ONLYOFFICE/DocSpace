﻿/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Enums.Filter;
using ASC.Mail.Exceptions;
using ASC.Mail.Models;
using ASC.Mail.Utils;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ASC.Mail.Core.Engine
{
    [Scope]
    public class FilterEngine
    {
        private int Tenant => TenantManager.GetCurrentTenant().TenantId;
        private string User => SecurityContext.CurrentAccount.ID.ToString();

        private ILog Log { get; }
        private MessageEngine MessageEngine { get; }
        private UserFolderEngine UserFolderEngine { get; }
        private TagEngine TagEngine { get; }
        private SecurityContext SecurityContext { get; }
        private TenantManager TenantManager { get; }
        private IMailDaoFactory MailDaoFactory { get; }

        public FilterEngine(
            SecurityContext securityContext,
            TenantManager tenantManager,
            IMailDaoFactory mailDaoFactory,
            MessageEngine messageEngine,
            UserFolderEngine userFolderEngine,
            TagEngine tagEngine,
            IOptionsMonitor<ILog> option)
        {
            MessageEngine = messageEngine;
            UserFolderEngine = userFolderEngine;
            TagEngine = tagEngine;
            SecurityContext = securityContext;
            TenantManager = tenantManager;

            MailDaoFactory = mailDaoFactory;

            Log = option.Get("ASC.Mail.FilterEngine");
        }

        public MailSieveFilterData Get(int id)
        {
            var filter = MailDaoFactory.GetFilterDao().Get(id);

            return ToFilterData(filter);
        }

        public List<MailSieveFilterData> GetList()
        {
            var filters = MailDaoFactory.GetFilterDao().GetList();

            return filters
                .ConvertAll(ToFilterData)
                .OrderBy(p => p.Position)
                .ToList();
        }

        public int Create(MailSieveFilterData filterData)
        {
            if (filterData == null)
                throw new ArgumentNullException("filterData");

            if (filterData.Id != 0)
                throw new ArgumentException("filterData.Id should be 0");

            return Save(filterData);
        }

        public bool Update(MailSieveFilterData filterData)
        {
            if (filterData == null)
                throw new ArgumentNullException("filterData");

            if (filterData.Id <= 0)
                throw new InvalidDataException("filterData.Id should be more then 0");

            return Save(filterData) > 0;
        }

        public static MailSieveFilterData GetValidFilter(MailSieveFilterData filterData)
        {
            var validFilter = new MailSieveFilterData
            {
                Id = filterData.Id,
                Enabled = filterData.Enabled
            };

            validFilter.Name = !string.IsNullOrEmpty(filterData.Name) && filterData.Name.Length > 64
                ? filterData.Name.Substring(0, 64)
                : filterData.Name;

            validFilter.Position = filterData.Position < 0 ? 0 : filterData.Position;

            if (filterData.Options == null)
                throw new ArgumentException("No options");

            if (filterData.Options.ApplyTo == null)
                throw new ArgumentException("No options apply to section");

            if (filterData.Options.ApplyTo.Folders == null || !filterData.Options.ApplyTo.Folders.Any())
                throw new ArgumentException("No folders in options");

            var aceptedFolders = new[] { (int)FolderType.Inbox, (int)FolderType.Sent, (int)FolderType.Spam };

            if (filterData.Options.ApplyTo.Folders.Any(f => !aceptedFolders.Contains(f)))
                throw new ArgumentException("Some folder is not accepted in the options");

            if (filterData.Options.ApplyTo.Mailboxes == null)
                filterData.Options.ApplyTo.Mailboxes = new int[] { };

            validFilter.Options = filterData.Options;

            if (filterData.Conditions.Any(c => string.IsNullOrEmpty(c.Value)))
                throw new ArgumentException("No condition value");

            if (filterData.Conditions.Any(c => c.Value.Length > 200))
                throw new ArgumentException("Too long condition value (limit is 200)");

            validFilter.Conditions = filterData.Conditions;

            if (!filterData.Actions.Any())
                throw new ArgumentException("No actions");

            validFilter.Actions = filterData.Actions.Distinct(new FilterActionEqualityComparer()).ToList();

            if (filterData.Actions.Count > 1 && filterData.Actions.Any(a => a.Action == ActionType.DeleteForever))
            {
                validFilter.Actions = new List<MailSieveFilterActionData>
                {
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.DeleteForever
                    }
                };
            }
            else
            {
                foreach (var actionData in validFilter.Actions)
                {
                    switch (actionData.Action)
                    {
                        case ActionType.MoveTo:
                            if (string.IsNullOrEmpty(actionData.Data))
                                throw new ArgumentException("Empty folder of 'Move to' action");

                            JObject dataJson;

                            try
                            {
                                dataJson = JObject.Parse(actionData.Data);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Not json data of 'Move to' action");
                            }

                            if (!dataJson.HasValues)
                                throw new ArgumentException("Have no values in json data of 'Move to' action");

                            if (!dataJson.ContainsKey("type"))
                                throw new ArgumentException("Have no type value in json data of 'Move to' action");

                            FolderType folderType;

                            if (!Enum.TryParse(dataJson["type"].ToString(), true, out folderType) ||
                                !Enum.IsDefined(typeof(FolderType), folderType))
                            {
                                throw new ArgumentException("Not valid type value in json data of 'Move to' action");
                            }

                            if (folderType == FolderType.UserFolder)
                            {
                                if (!dataJson.ContainsKey("userFolderId"))
                                    throw new ArgumentException(
                                        "Have no userFolderId value in json data of 'Move to' action");


                                if (!uint.TryParse(dataJson["userFolderId"].ToString(), out uint userFolderId))
                                    throw new ArgumentException(
                                        "Not valid userFolderId value in json data of 'Move to' action");
                            }

                            break;
                        case ActionType.MarkTag:
                            if (string.IsNullOrEmpty(actionData.Data))
                                throw new ArgumentException("Empty tag of 'Add tag' action");

                            int parsedId;
                            if (!int.TryParse(actionData.Data, out parsedId))
                                throw new ArgumentException("Not numeric tag id of 'Add tag' action");

                            break;
                        case ActionType.MarkAsImportant:
                        case ActionType.MarkAsRead:
                        case ActionType.DeleteForever:

                            if (!string.IsNullOrEmpty(actionData.Data))
                                actionData.Data = null;

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return validFilter;
        }

        private int Save(MailSieveFilterData filterDataObj)
        {
            var filterData = GetValidFilter(filterDataObj);

            var filterJsonStr = JsonConvert.SerializeObject(filterData, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new ShouldSerializeContractResolver()
            });

            var filter = new Filter
            {
                Id = filterData.Id,
                User = User,
                Tenant = Tenant,
                Enabled = filterData.Enabled,
                FilterData = filterJsonStr,
                Position = filterData.Position
            };

            var id = MailDaoFactory.GetFilterDao().Save(filter);

            return id;
        }

        public bool Delete(int id)
        {
            var res = MailDaoFactory.GetFilterDao().Delete(id);

            return res > 0;
        }

        public bool IsConditionSucceed(MailSieveFilterConditionData condition, MailMessageData message)
        {
            try
            {
                bool success;

                Func<ConditionOperationType, string, string, bool> isSucceed = (o, vLeft, vRight) =>
                {
                    if (string.IsNullOrEmpty(vLeft))
                        return string.IsNullOrEmpty(vRight);

                    if (vRight == null)
                        return false;

                    switch (o)
                    {
                        case ConditionOperationType.Matches:
                            return vLeft.Equals(vRight, StringComparison.InvariantCultureIgnoreCase);
                        case ConditionOperationType.NotMatches:
                            return !vLeft.Equals(vRight, StringComparison.InvariantCultureIgnoreCase);
                        case ConditionOperationType.Contains:
                            return vLeft.IndexOf(vRight, StringComparison.InvariantCultureIgnoreCase) >= 0;
                        case ConditionOperationType.NotContains:
                            return vLeft.IndexOf(vRight, StringComparison.InvariantCultureIgnoreCase) == -1;
                        default:
                            throw new ArgumentOutOfRangeException("o", o, null);
                    }
                };

                Func<List<MailAddress>, ConditionOperationType, string, bool> compareToFilter =
                    (addresses, o, v) =>
                    {
                        return addresses.Any(a => isSucceed(o, a.DisplayName, v) || isSucceed(o, a.Address, v));
                    };

                switch (condition.Key)
                {
                    case ConditionKeyType.From:
                        MailAddress address = null;

                        MailUtil.SkipErrors(() =>
                        {
                            var a = Parser.ParseAddress(message.From);
                            address = new MailAddress(a.Email, a.Name);
                        });

                        success = address == null
                            ? isSucceed(condition.Operation, message.From, condition.Value)
                            : compareToFilter(new List<MailAddress> { address }, condition.Operation, condition.Value);
                        break;
                    case ConditionKeyType.ToOrCc:
                        success = compareToFilter(message.ToList, condition.Operation, condition.Value) ||
                                  compareToFilter(message.CcList, condition.Operation, condition.Value);
                        break;
                    case ConditionKeyType.To:
                        success = compareToFilter(message.ToList, condition.Operation, condition.Value);
                        break;
                    case ConditionKeyType.Cc:
                        success = compareToFilter(message.CcList, condition.Operation, condition.Value);
                        break;
                    case ConditionKeyType.Subject:
                        success = isSucceed(condition.Operation, message.Subject, condition.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return success;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return false;
        }

        public void ApplyFilters(MailMessageData message, MailBoxData mailbox, MailFolder folder,
            List<MailSieveFilterData> filters)
        {
            var listAppliedFilters = new List<MailSieveFilterData>();

            if (!filters.Any())
                return;

            foreach (var filter in filters)
            {
                if (!filter.Enabled)
                    continue;

                if (filter.Options.ApplyTo.Folders.Any() &&
                    !filter.Options.ApplyTo.Folders.Contains((int)folder.Folder))
                {
                    continue;
                }

                if (filter.Options.ApplyTo.Mailboxes.Any() &&
                    !filter.Options.ApplyTo.Mailboxes.Contains(mailbox.MailBoxId))
                {
                    continue;
                }

                var appliedCount = filter.Conditions.Count(c =>
                {
                    var success = IsConditionSucceed(c, message);
                    if (success)
                    {
                        Log.InfoFormat("Filter condition succeed -> {0} {1} '{2}'",
                            Enum.GetName(typeof(ConditionKeyType), c.Key),
                            Enum.GetName(typeof(ConditionOperationType), c.Operation), c.Value);
                    }
                    return success;
                });

                switch (filter.Options.MatchMultiConditions)
                {
                    case MatchMultiConditionsType.MatchAll:
                    case MatchMultiConditionsType.None:
                        if (filter.Conditions.Count == appliedCount)
                        {
                            listAppliedFilters.Add(filter);
                        }
                        else if (appliedCount > 0)
                        {
                            Log.InfoFormat("Skip filter by not match all conditions");
                        }
                        break;
                    case MatchMultiConditionsType.MatchAtLeastOne:
                        if (appliedCount > 0)
                            listAppliedFilters.Add(filter);
                        break;
                    default:
                        Log.Error("Unknown MatchMultiConditionsType");
                        break;
                }

                if (appliedCount > 0 && filter.Options.IgnoreOther)
                    break;
            }

            foreach (var filter in listAppliedFilters)
            {
                foreach (var action in filter.Actions)
                {
                    try
                    {
                        Log.InfoFormat("Apply filter (id={0}) action: '{1}'{2}", filter.Id,
                            Enum.GetName(typeof(ActionType), action.Action),
                            action.Action == ActionType.MarkTag || action.Action == ActionType.MoveTo
                                ? " id=" + action.Data
                                : "");

                        ApplyAction(new List<int> { message.Id }, action);
                    }
                    catch (NotFoundFilterDataException ex)
                    {
                        Log.Error(ex.ToString());

                        Log.DebugFormat("Disable filter with id={0}", filter.Id);

                        filter.Enabled = false;

                        Update(filter);

                        break;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("ApplyFilters(filterId = {0}, mailId = {1}) Exception:\r\n{2}\r\n", filter.Id,
                            message.Id, e.ToString());
                    }
                }
            }
        }

        public void ApplyAction(List<int> ids, MailSieveFilterActionData action)
        {
            switch (action.Action)
            {
                case ActionType.DeleteForever:
                    MessageEngine.SetRemoved(ids);
                    break;
                case ActionType.MarkAsRead:
                    MessageEngine.SetUnread(ids, false);
                    break;
                case ActionType.MoveTo:
                    var dataJson = JObject.Parse(action.Data);

                    FolderType folderType;

                    if (!Enum.TryParse(dataJson["type"].ToString(), true, out folderType) ||
                        !Enum.IsDefined(typeof(FolderType), folderType))
                    {
                        throw new ArgumentException("Not valid type value in json data of 'Move to' action");
                    }

                    int folderId;

                    if (folderType == FolderType.UserFolder)
                    {
                        var userFolderId = int.Parse(dataJson["userFolderId"].ToString());

                        var userFolder = UserFolderEngine.Get(userFolderId);
                        if (userFolder == null)
                        {
                            throw new NotFoundFilterDataException(string.Format("User folder with id={0} not found",
                                userFolderId));
                        }

                        folderId = userFolderId;
                    }
                    else
                    {
                        folderId = (int)folderType;
                    }

                    MessageEngine.SetFolder(ids, folderType,
                        folderType == FolderType.UserFolder ? folderId : null);
                    break;
                case ActionType.MarkTag:
                    var tagId = Convert.ToInt32(action.Data);

                    var tag = TagEngine.GetTag(tagId);

                    if (tag == null)
                    {
                        throw new NotFoundFilterDataException(string.Format("Tag with id={0} not found", tagId));
                    }

                    TagEngine.SetMessagesTag(ids, tagId);
                    break;
                case ActionType.MarkAsImportant:
                    MessageEngine.SetImportant(ids, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected MailSieveFilterData ToFilterData(Filter filter)
        {
            if (filter == null) return null;

            var res = JsonConvert.DeserializeObject<MailSieveFilterData>(filter.FilterData, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new ShouldSerializeContractResolver()
            });

            res.Id = filter.Id;
            res.Enabled = filter.Enabled;
            res.Position = filter.Position;

            return res;
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        private const string ID = "id";
        private const string ENABLED = "enabled";
        private const string ACTION = "action";
        private const string KEY = "key";
        private const string OPERATION = "operation";
        private const string APPLY_TO_MESSAGES = "applyToMessages";
        private const string APPLY_TO_ATTACHMENTS = "applyToAttachments";
        private const string MATCH_MULTI_CONDITIONS = "matchMultiConditions";
        private const string POSITION = "position";

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType != typeof(MailSieveFilterData) &&
                property.DeclaringType != typeof(MailSieveFilterConditionData) &&
                property.DeclaringType != typeof(MailSieveFilterActionData))
                return property;

            switch (property.PropertyName)
            {
                case ID:
                case ENABLED:
                case POSITION:
                    property.ShouldSerialize = i => false;
                    return property;
                case ACTION:
                case KEY:
                case OPERATION:
                case APPLY_TO_MESSAGES:
                case APPLY_TO_ATTACHMENTS:
                case MATCH_MULTI_CONDITIONS:
                    property.Converter = new StringEnumConverter();
                    return property;
                default:
                    return property;
            }
        }
    }

    public class FilterActionEqualityComparer : IEqualityComparer<MailSieveFilterActionData>
    {
        public bool Equals(MailSieveFilterActionData action1, MailSieveFilterActionData action2)
        {
            if (action1 == null && action2 == null)
                return true;

            if (action1 == null || action2 == null)
                return false;

            return action1.Action == action2.Action;
        }

        public int GetHashCode(MailSieveFilterActionData obj)
        {
            return obj.Action.GetHashCode();
        }
    }
}
