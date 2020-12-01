/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Core.Resources;
using ASC.Web.Core.Users;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace ASC.Files.Core
{
    [DebuggerDisplay("{ID} v{Version}")]
    public class EditHistory
    {
        public EditHistory(
            IOptionsMonitor<ILog> options,
            TenantUtil tenantUtil,
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            Logger = options.CurrentValue;
            TenantUtil = tenantUtil;
            UserManager = userManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
        }

        public int ID { get; set; }
        public string Key { get; set; }
        public int Version { get; set; }
        public int VersionGroup { get; set; }

        [JsonPropertyName("user")]
        public EditHistoryAuthor ModifiedBy { get; set; }

        [JsonPropertyName("changeshistory")]
        public string ChangesString { get; set; }

        public List<EditHistoryChanges> Changes
        {
            get
            {
                var changes = new List<EditHistoryChanges>();
                if (string.IsNullOrEmpty(ChangesString)) return changes;

                try
                {
                    var jObject = JObject.Parse(ChangesString);
                    ServerVersion = jObject.Value<string>("serverVersion");

                    if (string.IsNullOrEmpty(ServerVersion))
                        return changes;

                    var jChanges = jObject.Value<JArray>("changes");

                    changes = jChanges.Children()
                                      .Select(jChange =>
                                          {
                                              var jUser = jChange.Value<JObject>("user");
                                              return new EditHistoryChanges(TenantUtil)
                                              {
                                                  Date = jChange.Value<string>("created"),
                                                  Author = new EditHistoryAuthor(UserManager, DisplayUserSettingsHelper)
                                                  {
                                                      Id = new Guid(jUser.Value<string>("id") ?? Guid.Empty.ToString()),
                                                      Name = jUser.Value<string>("name"),
                                                  },
                                              };
                                          })
                                      .ToList();
                    return changes;
                }
                catch (Exception ex)
                {
                    Logger.Error("DeSerialize old scheme exception", ex);
                }

                return changes;
            }
            set { throw new NotImplementedException(); }
        }

        public DateTime ModifiedOn;

        [JsonPropertyName("created")]
        public string ModifiedOnString
        {
            get { return ModifiedOn.Equals(default) ? null : ModifiedOn.ToString("g"); }
            set { throw new NotImplementedException(); }
        }

        public ILog Logger { get; }
        private TenantUtil TenantUtil { get; }
        private UserManager UserManager { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }

        public string ServerVersion;
    }

    [DebuggerDisplay("{Id} {Name}")]
    public class EditHistoryAuthor
    {
        public EditHistoryAuthor(
            UserManager userManager,
            DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            UserManager = userManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
        }

        public Guid Id { get; set; }

        private string _name;

        public string Name
        {
            get
            {
                UserInfo user;
                return
                    Id.Equals(Guid.Empty)
                          || Id.Equals(ASC.Core.Configuration.Constants.Guest.ID)
                          || (user = UserManager.GetUsers(Id)).Equals(Constants.LostUser)
                              ? string.IsNullOrEmpty(_name)
                                    ? FilesCommonResource.Guest
                                    : _name
                              : user.DisplayUserName(false, DisplayUserSettingsHelper);
            }
            set { _name = value; }
        }

        private UserManager UserManager { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
    }

    [DebuggerDisplay("{Author.Name}")]
    public class EditHistoryChanges
    {
        public EditHistoryChanges(TenantUtil tenantUtil)
        {
            TenantUtil = tenantUtil;
        }

        [JsonPropertyName("user")]
        public EditHistoryAuthor Author { get; set; }

        private DateTime _date;

        [JsonPropertyName("created")]
        public string Date
        {
            get { return _date.Equals(default) ? null : _date.ToString("g"); }
            set
            {
                if (DateTime.TryParse(value, out _date))
                {
                    _date = TenantUtil.DateTimeFromUtc(_date);
                }
            }
        }

        private TenantUtil TenantUtil { get; }
    }

    [DebuggerDisplay("{Version}")]
    public class EditHistoryData
    {
        public string ChangesUrl { get; set; }

        public string Key { get; set; }

        public EditHistoryUrl Previous { get; set; }

        public string Token { get; set; }

        public string Url { get; set; }

        public int Version { get; set; }
    }

    [DebuggerDisplay("{Key} - {Url}")]
    public class EditHistoryUrl
    {
        public string Key { get; set; }

        public string Url { get; set; }
    }
}