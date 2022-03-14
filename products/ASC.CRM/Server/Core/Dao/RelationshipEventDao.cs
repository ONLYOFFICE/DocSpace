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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Files.Core;
using ASC.Web.CRM.Core.Search;
using ASC.Web.Files.Api;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class RelationshipEventDao : AbstractDao
    {
        private readonly CrmSecurity _crmSecurity;
        private readonly TenantUtil _tenantUtil;
        private readonly FilesIntegration _filesIntegration;
        private readonly FactoryIndexerEvents _factoryIndexer;

        public RelationshipEventDao(DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            FilesIntegration filesIntegration,
            FactoryIndexerEvents factoryIndexerEvents,
            CrmSecurity crmSecurity,
            TenantUtil tenantUtil,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper
            ) :
                base(dbContextManager,
                        tenantManager,
                        securityContext,
                        logger,
                        ascCache,
                        mapper)
        {
            _filesIntegration = filesIntegration;
            _tenantUtil = tenantUtil;
            _crmSecurity = crmSecurity;
            _factoryIndexer = factoryIndexerEvents;
        }

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

            var dao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var tags = fileIDs.ToList().ConvertAll(fileID => new Tag("RelationshipEvent_" + eventID, TagType.System, Guid.Empty) { EntryType = FileEntryType.File, EntryId = fileID });

            dao.SaveTags(tags);

            if (fileIDs.Length > 0)
            {
                var dbEntity = CrmDbContext.RelationshipEvent.Find(eventID);

                dbEntity.HaveFiles = true;

                CrmDbContext.SaveChanges();
            }
        }

        public async System.Threading.Tasks.Task<int> GetFilesCountAsync(int[] contactID, EntityType entityType, int entityID)
        {
            var filesIds = await GetFilesIDsAsync(contactID, entityType, entityID).ToArrayAsync();
            return filesIds.Length;
        }

        private IAsyncEnumerable<int> GetFilesIDsAsync(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            return InternalGetFilesIDsAsync(contactID, entityType, entityID);
        }

        private async IAsyncEnumerable<int> InternalGetFilesIDsAsync(int[] contactID, EntityType entityType, int entityID)
        {
            var sqlQuery = Query(CrmDbContext.RelationshipEvent);

            if (contactID != null && contactID.Length > 0)
                sqlQuery = sqlQuery.Where(x => contactID.Contains(x.ContactId));

            if (entityID > 0)
                sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);

            sqlQuery = sqlQuery.Where(x => x.HaveFiles);

            var tagNames = await sqlQuery.Select(x => String.Format("RelationshipEvent_{0}", x.Id)).ToArrayAsync();
            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var ids = tagdao.GetTagsAsync(tagNames.ToArray(), TagType.System)
               .Where(t => t.EntryType == FileEntryType.File)
               .Select(t => Convert.ToInt32(t.EntryId));

            await foreach (var id in ids)
                yield return id;

        }

        public async IAsyncEnumerable<File<int>> GetAllFilesAsync(int[] contactID, EntityType entityType, int entityID)
        {
            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            var ids = await GetFilesIDsAsync(contactID, entityType, entityID).ToArrayAsync();
            var files = 0 < ids.Length ? filedao.GetFilesAsync(ids) : AsyncEnumerable.Empty<File<int>>();

            await foreach(var file in files)
            {
                _crmSecurity.SetAccessTo(file);
                yield return file;
            }
        }

        public Task<Dictionary<int, List<File<int>>>> GetFilesAsync(int[] eventID)
        {
            if (eventID == null || eventID.Length == 0)
                throw new ArgumentException("eventID");

            return InternalGetFilesAsync(eventID);
        }

        public async Task<Dictionary<int, List<File<int>>>> InternalGetFilesAsync(int[] eventID)
        {
            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();
            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            var findedTags = await tagdao.GetTagsAsync(eventID.Select(item => String.Concat("RelationshipEvent_", item)).ToArray(),
                TagType.System).Where(t => t.EntryType == FileEntryType.File).ToArrayAsync();

            var filesID = findedTags.Select(t => Convert.ToInt32(t.EntryId)).ToArray();

            var files = 0 < filesID.Length ? filedao.GetFilesAsync(filesID) : AsyncEnumerable.Empty<File<int>>();

            var filesTemp = new Dictionary<object, File<int>>();

            await files.ForEachAsync(item =>
            {
                if (!filesTemp.ContainsKey(item.ID))
                    filesTemp.Add(item.ID, item);
            });

            return findedTags.Where(x => filesTemp.ContainsKey(x.EntryId)).GroupBy(x => x.TagName).ToDictionary(x => Convert.ToInt32(x.Key.Split(new[] { '_' })[1]),
                                                              x => x.Select(item => filesTemp[item.EntryId]).ToList());
        }

        public IAsyncEnumerable<File<int>> GetFilesAsync(int eventID)
        {
            if (eventID == 0)
                throw new ArgumentException("eventID");

            return InternalGetFilesAsync(eventID);
        }

        private async IAsyncEnumerable<File<int>> InternalGetFilesAsync(int eventID)
        {
            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();
            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            var ids = await tagdao.GetTagsAsync(String.Concat("RelationshipEvent_", eventID), TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => Convert.ToInt32(t.EntryId)).ToArrayAsync();
            var files = 0 < ids.Length ? filedao.GetFilesAsync(ids) : AsyncEnumerable.Empty<File<int>>();

            await foreach (var file in files)
            {
                _crmSecurity.SetAccessTo(file);
                yield return file;
            }
        }

        private System.Threading.Tasks.Task RemoveAllFilesAsync(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
            {
                throw new ArgumentException();
            }

            return InternalRemoveAllFilesAsync(contactID, entityType, entityID);
        }

        private async System.Threading.Tasks.Task InternalRemoveAllFilesAsync(int[] contactID, EntityType entityType, int entityID)
        {
            var files = GetAllFilesAsync(contactID, entityType, entityID);

            var dao = _filesIntegration.DaoFactory.GetFileDao<int>();

            await foreach (var file in files)
            {
                await dao.DeleteFileAsync(file.ID);
            }
        }

        public async IAsyncEnumerable<int> RemoveFileAsync(File<int> file)
        {
            _crmSecurity.DemandDelete(file);


            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var tags = tagdao.GetTagsAsync(file.ID, FileEntryType.File, TagType.System).Where(tag => tag.TagName.StartsWith("RelationshipEvent_"));

            var eventIDs = tags.Select(item => Convert.ToInt32(item.TagName.Split(new[] { '_' })[1]));

            var dao = _filesIntegration.DaoFactory.GetFileDao<int>();

            await dao.DeleteFileAsync(file.ID);

            await foreach (var eventID in eventIDs)
            {
                if (await GetFilesAsync(eventID).CountAsync() == 0)
                {
                    var dbEntity = await CrmDbContext.RelationshipEvent.FindAsync(eventID);

                    if (dbEntity.TenantId != TenantID) continue;

                    dbEntity.HaveFiles = false;

                    await CrmDbContext.SaveChangesAsync();
                }

                yield return eventID;
            }

            var itemToUpdate = await Query(CrmDbContext.Invoices).FirstOrDefaultAsync(x => x.FileId == file.ID);

            itemToUpdate.FileId = 0;

            await CrmDbContext.SaveChangesAsync();
        }


        public int GetCount(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            var sqlQuery = Query(CrmDbContext.RelationshipEvent);

            if (contactID.Length > 0)
                sqlQuery = sqlQuery.Where(x => contactID.Contains(x.ContactId));

            if (entityID > 0)
                sqlQuery = sqlQuery.Where(x => x.EntityId == entityID && x.EntityType == entityType);

            return sqlQuery.Count();
        }

        public RelationshipEvent CreateItem(RelationshipEvent item)
        {
            _crmSecurity.DemandCreateOrUpdate(item);

            var htmlBody = String.Empty;

            if (item.CreateOn == DateTime.MinValue)
                item.CreateOn = _tenantUtil.DateTimeNow();

            item.CreateBy = _securityContext.CurrentAccount.ID;
            item.LastModifedBy = _securityContext.CurrentAccount.ID;

            if (item.CategoryID == (int)HistoryCategorySystem.MailMessage)
            {
           //     var jsonObj = JsonDocument.Parse(item.Content).RootElement;
         //       var messageId = jsonObj.GetProperty("message_id").GetInt32();

                //var apiServer = new ApiServer();
                //var msg = apiServer.GetApiResponse(
                //    String.Format("{0}mail/messages/{1}.json?id={1}&loadImages=true&needSanitize=true", SetupInfo.WebApiBaseUrl, messageId), "GET");

        //        String msg = null;
                //                if (msg == null)
                throw new ArgumentException("Mail message cannot be found");

                //var msgResponseDto = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(msg)));
                //var msgRequestObj = msgResponseDto.Value<JObject>("response");
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
                CreateOn = _tenantUtil.DateTimeToUtc(item.CreateOn),
                CreateBy = item.CreateBy,
                EntityType = item.EntityType,
                EntityId = item.EntityID,
                CategoryId = item.CategoryID,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = item.LastModifedBy,
                TenantId = TenantID,
                HaveFiles = false
            };

            CrmDbContext.RelationshipEvent.Add(itemToInsert);
            CrmDbContext.SaveChanges();

            item.ID = itemToInsert.Id;

            if (item.CreateOn.Kind == DateTimeKind.Utc)
                item.CreateOn = _tenantUtil.DateTimeFromUtc(item.CreateOn);

            _factoryIndexer.Index(itemToInsert);

            return item;
        }

        public RelationshipEvent GetByID(int id)
        {
            var dbEntity = CrmDbContext.RelationshipEvent.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            var entity = _mapper.Map<DbRelationshipEvent, RelationshipEvent>(dbEntity);

            _crmSecurity.DemandAccessTo(entity);

            return entity;

        }

        public int GetAllItemsCount()
        {
            return Query(CrmDbContext.RelationshipEvent).Count();
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

            var sqlQuery = Query(CrmDbContext.RelationshipEvent).AsNoTracking();

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = false;

                        isCompany = Query(CrmDbContext.Contacts).Where(x => x.Id == entityID).Select(x => x.IsCompany).Single();

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
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1)));
            }
            else if (fromDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate));
            }
            else if (toDate != DateTime.MinValue)
            {
                sqlQuery = sqlQuery.Where(x => x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate).AddDays(1).AddMinutes(-1));
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

                if (!_factoryIndexer.TrySelectIds(s => s.MatchAll(searchText), out eventsIds))
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
            {

                switch ((RelationshipEventByType)orderBy.SortedBy)
                {
                    case RelationshipEventByType.Category:
                        sqlQuery = sqlQuery.OrderBy("CategoryId", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.Content:
                        sqlQuery = sqlQuery.OrderBy("Content", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.CreateBy:
                        sqlQuery = sqlQuery.OrderBy("CreateBy", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.Created:
                        sqlQuery = sqlQuery.OrderBy("CreateOn", orderBy.IsAsc);
                        break;
                }
            }
            else
            {
                sqlQuery = sqlQuery.OrderBy("CreateOn", orderBy.IsAsc);
            }

            return _mapper.Map<List<DbRelationshipEvent>, List<RelationshipEvent>>(sqlQuery.ToList());
        }

        public System.Threading.Tasks.Task DeleteItemAsync(int id)
        {
            var item = GetByID(id);
            if (item == null) throw new ArgumentException();

            return InternalDeleteItemAsync(item);
        }

        private async System.Threading.Tasks.Task InternalDeleteItemAsync(RelationshipEvent item)
        {
            await DeleteItemAsync(item);
        }

        public async System.Threading.Tasks.Task DeleteItemAsync(RelationshipEvent item)
        {
            _crmSecurity.DemandDelete(item);

            var relativeFiles = await GetFilesAsync(item.ID).ToListAsync();

            var nowFilesEditing = relativeFiles.Where(file => (file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing);

            if (nowFilesEditing.Count() != 0)
                throw new ArgumentException();

            foreach (var file in relativeFiles)
            {
                await RemoveFileAsync(file).ToListAsync();
            }

            var itemToDelete = Query(CrmDbContext.RelationshipEvent).Where(x => x.Id == item.ID).Single();

            _factoryIndexer.Delete(itemToDelete);

            CrmDbContext.RelationshipEvent.Remove(itemToDelete);

            await CrmDbContext.SaveChangesAsync();

        }

        internal class CrmHistoryContent
        {
            public string to;
            public string from;
            public string cc;
            public string bcc;
            public string subject;
            public bool important;
            public string chain_id;
            public bool is_sended;
            public string date_created;
            public string introduction;
            public long message_id;
        }

        private static string GetHistoryContentJson(JsonElement apiResponse)
        {
            var content_struct = new CrmHistoryContent
            {
                @from = apiResponse.GetProperty("from").GetString(),
                to = apiResponse.GetProperty("to").GetString(),
                cc = apiResponse.GetProperty("cc").GetString(),
                bcc = apiResponse.GetProperty("bcc").GetString(),
                subject = apiResponse.GetProperty("subject").GetString(),
                important = apiResponse.GetProperty("important").GetBoolean(),
                chain_id = apiResponse.GetProperty("chainId").GetString(),
                is_sended = apiResponse.GetProperty("folder").GetInt32() == 1,
                date_created = apiResponse.GetProperty("date").GetString(),
                introduction = apiResponse.GetProperty("introduction").GetString(),
                message_id = apiResponse.GetProperty("id").GetInt32()
            };

            return JsonSerializer.Serialize(content_struct);
        }
    }
}