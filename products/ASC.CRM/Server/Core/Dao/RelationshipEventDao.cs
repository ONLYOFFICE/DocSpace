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


using ASC.Collections;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.Files.Core;
using ASC.Web.CRM;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;
using ASC.Web.Studio.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Core.Dao
{

    public class CachedRelationshipEventDao : RelationshipEventDao
    {
        private readonly HttpRequestDictionary<RelationshipEvent> _relationshipEventCache;

        public CachedRelationshipEventDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FilesIntegration filesIntegration,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            SetupInfo setupInfo,
            PathProvider pathProvider,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> logger

            ) :
                        base(dbContextManager,
                            tenantManager,
                            securityContext,
                            filesIntegration,
                            cRMSecurity,
                            tenantUtil,
                            setupInfo,
                            pathProvider,
                            logger)
        {
            _relationshipEventCache = new HttpRequestDictionary<RelationshipEvent>(httpContextAccessor?.HttpContext, "crm_relationshipEvent");
        }

        public override RelationshipEvent GetByID(int eventID)
        {
            return _relationshipEventCache.Get(eventID.ToString(), () => GetByIDBase(eventID));
        }

        private RelationshipEvent GetByIDBase(int eventID)
        {
            return base.GetByID(eventID);
        }

        private void ResetCache(int dealID)
        {
            _relationshipEventCache.Reset(dealID.ToString());
        }
    }

    public class RelationshipEventDao : AbstractDao
    {

        public RelationshipEventDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FilesIntegration filesIntegration,
            CRMSecurity cRMSecurity,
            TenantUtil tenantUtil,
            SetupInfo setupInfo,
            PathProvider pathProvider,
            IOptionsMonitor<ILog> logger
            ) :
                                            base(dbContextManager,
                                                 tenantManager,
                                                 securityContext,
                                                 logger)
        {
            FilesIntegration = filesIntegration;
            TenantUtil = tenantUtil;
            CRMSecurity = cRMSecurity;
            SetupInfo = setupInfo;
            PathProvider = pathProvider;
        }


        public PathProvider PathProvider { get; }

        public SetupInfo SetupInfo { get; }

        public CRMSecurity CRMSecurity { get; }

        public TenantUtil TenantUtil { get; }

        public FilesIntegration FilesIntegration { get; }

        public FactoryIndexer<EventsWrapper> FactoryIndexer { get; }

        public RelationshipEvent AttachFiles(int contactID, EntityType entityType, int entityID, int[] fileIDs)
        {
            if (entityID > 0 && !_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var relationshipEvent = new RelationshipEvent
            {
                CategoryID = (int)HistoryCategorySystem.FilesUpload,
                ContactID = contactID,
                EntityType = entityType,
                EntityID = entityID,
                Content = HistoryCategorySystem.FilesUpload.ToLocalizedString()
            };

            relationshipEvent = CreateItem(relationshipEvent);

            AttachFiles(relationshipEvent.ID, fileIDs);

            return relationshipEvent;
        }

        public void AttachFiles(int eventID, int[] fileIDs)
        {
            if (fileIDs.Length == 0) return;

            var dao = FilesIntegration.DaoFactory.GetTagDao<int>();

            var tags = fileIDs.ToList().ConvertAll(fileID => new Tag("RelationshipEvent_" + eventID, TagType.System, Guid.Empty) { EntryType = FileEntryType.File, EntryId = fileID });

            dao.SaveTags(tags);

            if (fileIDs.Length > 0)
            {
                var dbRelationshipEvent = new DbRelationshipEvent
                {
                    Id = eventID,
                    HaveFiles = true,
                    TenantId = TenantID
                };

                CRMDbContext.RelationshipEvent.Attach(dbRelationshipEvent);
                CRMDbContext.Entry(dbRelationshipEvent).Property(x => x.HaveFiles).IsModified = true;
                CRMDbContext.SaveChanges();
            }
        }

        public int GetFilesCount(int[] contactID, EntityType entityType, int entityID)
        {
            return GetFilesIDs(contactID, entityType, entityID).Length;
        }

        private int[] GetFilesIDs(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            var sqlQuery = Query(CRMDbContext.RelationshipEvent);

            if (contactID != null && contactID.Length > 0)
                sqlQuery = sqlQuery.Where(x => contactID.Contains(x.ContactId));

            if (entityID > 0)
                sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);

            sqlQuery = sqlQuery.Where(x => x.HaveFiles);

            var tagNames = sqlQuery.Select(x => String.Format("RelationshipEvent_{0}", x.Id));
            var tagdao = FilesIntegration.DaoFactory.GetTagDao<int>();

            return tagdao.GetTags(tagNames.ToArray(), TagType.System)
               .Where(t => t.EntryType == FileEntryType.File)
               .Select(t => Convert.ToInt32(t.EntryId)).ToArray();
             
        }

        public List<File<int>> GetAllFiles(int[] contactID, EntityType entityType, int entityID)
        {
            var filedao = FilesIntegration.DaoFactory.GetFileDao<int>();

            var ids = GetFilesIDs(contactID, entityType, entityID);
            var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File<int>>();

            files.ForEach(CRMSecurity.SetAccessTo);

            return files.ToList();
        }

        public Dictionary<int, List<File<int>>> GetFiles(int[] eventID)
        {
            if (eventID == null || eventID.Length == 0)
                throw new ArgumentException("eventID");

            var tagdao = FilesIntegration.DaoFactory.GetTagDao<int>();
            var filedao = FilesIntegration.DaoFactory.GetFileDao<int>();

            var findedTags = tagdao.GetTags(eventID.Select(item => String.Concat("RelationshipEvent_", item)).ToArray(),
                TagType.System).Where(t => t.EntryType == FileEntryType.File);

            var filesID = findedTags.Select(t => Convert.ToInt32(t.EntryId)).ToArray();

            var files = 0 < filesID.Length ? filedao.GetFiles(filesID) : new List<File<int>>();

            var filesTemp = new Dictionary<object, File<int>>();

            files.ForEach(item =>
                              {
                                  if (!filesTemp.ContainsKey(item.ID))
                                      filesTemp.Add(item.ID, item);
                              });

            return findedTags.Where(x => filesTemp.ContainsKey(x.EntryId)).GroupBy(x => x.TagName).ToDictionary(x => Convert.ToInt32(x.Key.Split(new[] { '_' })[1]),
                                                              x => x.Select(item => filesTemp[item.EntryId]).ToList());
        }

        public List<File<int>> GetFiles(int eventID)
        {
            if (eventID == 0)
                throw new ArgumentException("eventID");

            var tagdao = FilesIntegration.DaoFactory.GetTagDao<int>();
            var filedao = FilesIntegration.DaoFactory.GetFileDao<int>();

            var ids = tagdao.GetTags(String.Concat("RelationshipEvent_", eventID), TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => Convert.ToInt32(t.EntryId)).ToArray();
            var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File<int>>();

            files.ForEach(CRMSecurity.SetAccessTo);

            return files.ToList();
        }

        private void RemoveAllFiles(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
            {
                throw new ArgumentException();
            }

            var files = GetAllFiles(contactID, entityType, entityID);

            var dao = FilesIntegration.DaoFactory.GetFileDao<int>();

            foreach (var file in files)
            {
                dao.DeleteFile(file.ID);
            }
        }

        public List<int> RemoveFile(File<int> file)
        {
            CRMSecurity.DemandDelete(file);

            List<int> eventIDs;

            var tagdao = FilesIntegration.DaoFactory.GetTagDao<int>();

            var tags = tagdao.GetTags(file.ID, FileEntryType.File, TagType.System).ToList().FindAll(tag => tag.TagName.StartsWith("RelationshipEvent_"));

            eventIDs = tags.Select(item => Convert.ToInt32(item.TagName.Split(new[] { '_' })[1])).ToList();

            var dao = FilesIntegration.DaoFactory.GetFileDao<int>();

            dao.DeleteFile(file.ID);

            foreach (var eventID in eventIDs)
            {
                if (GetFiles(eventID).Count == 0)
                {
                    var dbRelationshipEvent = new DbRelationshipEvent { 
                        Id = eventID,
                        HaveFiles = false,
                        TenantId = TenantID
                    };

                    CRMDbContext.Attach(dbRelationshipEvent);
                    CRMDbContext.Entry(dbRelationshipEvent).Property(x => x.HaveFiles).IsModified = true;

                    CRMDbContext.SaveChanges();
                }
            }

            var itemToUpdate = Query(CRMDbContext.Invoices).FirstOrDefault(x => x.FileId == Convert.ToInt32(file.ID));

            itemToUpdate.FileId = 0;

            CRMDbContext.Update(itemToUpdate);

            CRMDbContext.SaveChanges();

            return eventIDs;
        }


        public int GetCount(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            var sqlQuery = Query(CRMDbContext.RelationshipEvent);

            if (contactID.Length > 0)
                sqlQuery = sqlQuery.Where(x => contactID.Contains(x.ContactId));

            if (entityID > 0)
                sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);

            return sqlQuery.Count();
        }

        public RelationshipEvent CreateItem(RelationshipEvent item)
        {
            CRMSecurity.DemandCreateOrUpdate(item);

            var htmlBody = String.Empty;

            if (item.CreateOn == DateTime.MinValue)
                item.CreateOn = TenantUtil.DateTimeNow();

            item.CreateBy = SecurityContext.CurrentAccount.ID;
            item.LastModifedBy = SecurityContext.CurrentAccount.ID;

            if (item.CategoryID == (int)HistoryCategorySystem.MailMessage)
            {
                var jsonObj = JObject.Parse(item.Content);
                var messageId = jsonObj.Value<Int32>("message_id");

                //var apiServer = new ApiServer();
                //var msg = apiServer.GetApiResponse(
                //    String.Format("{0}mail/messages/{1}.json?id={1}&loadImages=true&needSanitize=true", SetupInfo.WebApiBaseUrl, messageId), "GET");

                String msg = null;
//                if (msg == null)
                    throw new ArgumentException("Mail message cannot be found");

                //var msgResponseWrapper = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(msg)));
                //var msgRequestObj = msgResponseWrapper.Value<JObject>("response");
                //string messageUrl;

                //htmlBody = msgRequestObj.Value<String>("htmlBody");

                //using (var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlBody)))
                //{
                //    var filePath = String.Format("folder_{0}/message_{1}.html", (messageId / 1000 + 1) * 1000, messageId);

                //    Global.GetStore().Save("mail_messages", filePath, fileStream);

                //    messageUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=mailmessage&message_id={1}", PathProvider.BaseAbsolutePath, messageId).ToLower();

                //}

                //var msg_date_created = msgRequestObj.Value<String>("date");
                //var message_id = msgRequestObj.Value<Int32>("id");
                //item.Content = JsonConvert.SerializeObject(new
                //{
                //    @from = msgRequestObj.Value<String>("from"),
                //    to = msgRequestObj.Value<String>("to"),
                //    cc = msgRequestObj.Value<String>("cc"),
                //    bcc = msgRequestObj.Value<String>("bcc"),
                //    subject = msgRequestObj.Value<String>("subject"),
                //    important = msgRequestObj.Value<Boolean>("important"),
                //    chain_id = msgRequestObj.Value<String>("chainId"),
                //    is_sended = msgRequestObj.Value<Int32>("folder") != 1,
                //    date_created = msg_date_created,
                //    introduction = msgRequestObj.Value<String>("introduction"),
                //    message_id = message_id,
                //    message_url = messageUrl
                //});

                //item.CreateOn = DateTime.Parse(msg_date_created, CultureInfo.InvariantCulture);

                //var sqlQueryFindMailsAlready = Query(CRMDbContext.RelationshipEvent)
                //                                    .Where(x => x.ContactId == item.ContactID)
                //                                    .Where(x => x.EntityType == item.EntityType)
                //                                    .Where(x => x.EntityId == item.EntityID)
                //                                    .Where(x => x.CategoryId == item.CategoryID)
                //                                    .Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Content, String.Format("\"message_id\":{0},", message_id)));

                //if (sqlQueryFindMailsAlready.Count() > 0)
                //    throw new Exception("Already exists");
            }

            var itemToInsert = new DbRelationshipEvent
            {
                ContactId = item.ContactID,
                Content = item.Content,
                CreateOn = TenantUtil.DateTimeToUtc(item.CreateOn),
                CreateBy = item.CreateBy,
                EntityType = item.EntityType,
                EntityId = item.EntityID,
                CategoryId = item.CategoryID,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = item.LastModifedBy,
                TenantId = TenantID,
                HaveFiles = false
            };

            CRMDbContext.RelationshipEvent.Add(itemToInsert);
            CRMDbContext.SaveChanges();

            item.ID = itemToInsert.Id;

            if (item.CreateOn.Kind == DateTimeKind.Utc)
                item.CreateOn = TenantUtil.DateTimeFromUtc(item.CreateOn);

            FactoryIndexer.IndexAsync(EventsWrapper.FromEvent(TenantID, item));

            return item;
        }

        public virtual RelationshipEvent GetByID(int eventID)
        {
            return ToRelationshipEvent(Query(CRMDbContext.RelationshipEvent)
                                            .FirstOrDefault(x => x.Id == eventID));
        }

        public int GetAllItemsCount()
        {
            return Query(CRMDbContext.RelationshipEvent).Count();
        }

        public List<RelationshipEvent> GetAllItems()
        {
            return GetItems(String.Empty,
                EntityType.Any,
                0,
                Guid.Empty,
                0,
                DateTime.MinValue,
                DateTime.MinValue,
                0,
                0, null);
        }

        public List<RelationshipEvent> GetItems(
            String searchText,
            EntityType entityType,
            int entityID,
            Guid createBy,
            int categoryID,
            DateTime fromDate,
            DateTime toDate,
            int from,
            int count,
            OrderBy orderBy)
        {

            var sqlQuery = Query(CRMDbContext.RelationshipEvent);

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = false;

                        isCompany = Query(CRMDbContext.Contacts).Where(x => x.Id == entityID).Select(x => x.IsCompany).Single();

                        if (isCompany)
                            return GetItems(searchText, EntityType.Company, entityID, createBy, categoryID, fromDate, toDate, from, count, orderBy);
                        else
                            return GetItems(searchText, EntityType.Person, entityID, createBy, categoryID, fromDate, toDate, from, count, orderBy);
                    case EntityType.Person:
                        sqlQuery = sqlQuery.Where(x => x.ContactId == entityID);
                        break;
                    case EntityType.Company:

                        var personIDs = GetRelativeToEntity(entityID, EntityType.Person, null).ToList();

                        if (personIDs.Count == 0)
                        {
                            sqlQuery = sqlQuery.Where(x => x.ContactId == entityID);
                        }
                        else
                        {
                            personIDs.Add(entityID);
                            sqlQuery = sqlQuery.Where(x => personIDs.Contains(x.ContactId));
                        }

                        break;
                    case EntityType.Case:
                    case EntityType.Opportunity:
                        sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);
                        break;
                }

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= TenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1)));
            }
            else if (fromDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= TenantUtil.DateTimeToUtc(fromDate));
            }
            else if (toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn <= TenantUtil.DateTimeToUtc(toDate).AddDays(1).AddMinutes(-1));
            }

            if (createBy != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateBy == createBy);
            }

            if (categoryID != 0)
            {
                sqlQuery = sqlQuery.Where(x => x.CategoryId == categoryID);
            }

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                List<int> eventsIds;

                if (!FactoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out eventsIds))
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Content, k + "%"));
                        }
                    }
                }
                else
                {
                    if (eventsIds.Count == 0) return new List<RelationshipEvent>();

                    sqlQuery = sqlQuery.Where(x => eventsIds.Contains(x.Id));
                }
            }

            if (0 < from && from < int.MaxValue)
                sqlQuery = sqlQuery.Skip(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery = sqlQuery.Take(count);

            if (orderBy != null && Enum.IsDefined(typeof(RelationshipEventByType), orderBy.SortedBy))
                switch ((RelationshipEventByType)orderBy.SortedBy)
                {
                    case RelationshipEventByType.Category:
                        sqlQuery = sqlQuery.OrderBy(x => x.CategoryId);
                        break;
                    case RelationshipEventByType.Content:
                        sqlQuery = sqlQuery.OrderBy(x => x.Content);
                        break;
                    case RelationshipEventByType.CreateBy:
                        sqlQuery = sqlQuery.OrderBy(x => x.CreateBy);
                        break;
                    case RelationshipEventByType.Created:
                        sqlQuery = sqlQuery.OrderBy(x => x.CreateOn);
                        break;
                }
            else
                sqlQuery = sqlQuery.OrderBy(x => x.CreateOn);

            return sqlQuery.ToList().ConvertAll(ToRelationshipEvent);
        }


        private RelationshipEvent ToRelationshipEvent(DbRelationshipEvent relationshipEvent)
        {
            if (relationshipEvent == null) return null;

            return new RelationshipEvent
            {
                ID = relationshipEvent.Id,
                ContactID = relationshipEvent.ContactId,
                Content = relationshipEvent.Content,
                EntityID = relationshipEvent.EntityId,
                EntityType = relationshipEvent.EntityType,
                CreateOn = TenantUtil.DateTimeFromUtc(relationshipEvent.CreateOn),
                CreateBy = relationshipEvent.CreateBy,
                CategoryID = relationshipEvent.CategoryId,
                LastModifedBy = relationshipEvent.LastModifedBy,
                LastModifedOn = relationshipEvent.LastModifedOn
            };
        }

        public void DeleteItem(int id)
        {
            var item = GetByID(id);
            if (item == null) throw new ArgumentException();

            DeleteItem(item);
        }

        public void DeleteItem(RelationshipEvent item)
        {
            CRMSecurity.DemandDelete(item);

            var relativeFiles = GetFiles(item.ID);

            var nowFilesEditing = relativeFiles.Where(file => (file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing);

            if (nowFilesEditing.Count() != 0)
                throw new ArgumentException();

            relativeFiles.ForEach(f => RemoveFile(f));

            var itemToDelete = new DbRelationshipEvent
            {
                Id = item.ID,
                TenantId = TenantID
            };

            CRMDbContext.RelationshipEvent.Remove(itemToDelete);
            CRMDbContext.SaveChanges();

            FactoryIndexer.DeleteAsync(EventsWrapper.FromEvent(TenantID, item));
        }

        [DataContract]
        internal class CrmHistoryContent
        {
            [DataMember]
            public string to;

            [DataMember]
            public string from;

            [DataMember]
            public string cc;

            [DataMember]
            public string bcc;

            [DataMember]
            public string subject;

            [DataMember]
            public bool important;

            [DataMember]
            public string chain_id;

            [DataMember]
            public bool is_sended;

            [DataMember]
            public string date_created;

            [DataMember]
            public string introduction;

            [DataMember]
            public long message_id;

        }

        private static string GetHistoryContentJson(JObject apiResponse)
        {
            var content_struct = new CrmHistoryContent
            {
                @from = apiResponse.Value<String>("from"),
                to = apiResponse.Value<String>("to"),
                cc = apiResponse.Value<String>("cc"),
                bcc = apiResponse.Value<String>("bcc"),
                subject = apiResponse.Value<String>("subject"),
                important = apiResponse.Value<Boolean>("important"),
                chain_id = apiResponse.Value<String>("chainId"),
                is_sended = apiResponse.Value<Int32>("folder") == 1,
                date_created = apiResponse.Value<String>("date"),
                introduction = apiResponse.Value<String>("introduction"),
                message_id = apiResponse.Value<Int32>("id")
            };

            var serializer = new DataContractJsonSerializer(typeof(CrmHistoryContent));

            using (var stream = new System.IO.MemoryStream())
            {
                serializer.WriteObject(stream, content_struct);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    public static class RelationshipEventDaoExtention
    {
        public static DIHelper AddListItemDaoService(this DIHelper services)
        {
            services.TryAddScoped<RelationshipEventDao>();

            return services.AddCRMDbContextService()
                           .AddTenantManagerService()
                           .AddSecurityContextService()
                           .AddFilesIntegrationService()
                           .AddCRMSecurityService()
                           .AddTenantUtilService()
                           .AddSetupInfo()
                           .AddCRMPathProviderService();
        }
    }

}   