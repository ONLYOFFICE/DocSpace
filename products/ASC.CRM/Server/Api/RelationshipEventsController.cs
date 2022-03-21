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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Api.Documents;
using ASC.Common.Web;
using ASC.Core;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.CRM.Api
{
    public class RelationshipEventsController : BaseApiController
    {
        private readonly FileUploader _fileUploader;
        private readonly ASC.Files.Core.Data.DaoFactory _filesDaoFactory;
        private readonly FileWrapperHelper _fileWrapperHelper;
        private readonly FilesSettingsHelper _filesSettingsHelper;
        private readonly ApiContext _apiContext;
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;
        private readonly SecurityContext _securityContext;
        private readonly NotifyClient _notifyClient;

        public RelationshipEventsController(
                     CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     ApiContext apiContext,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     FileWrapperHelper fileWrapperHelper,
                     ASC.Files.Core.Data.DaoFactory filesDaoFactory,
                     FileUploader fileUploader,
                     SecurityContext securityContext,
                     NotifyClient notifyClient,
                     FilesSettingsHelper filesSettingsHelper,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _apiContext = apiContext;
            _messageTarget = messageTarget;
            _messageService = messageService;
            _fileWrapperHelper = fileWrapperHelper;
            _filesDaoFactory = filesDaoFactory;
            _fileUploader = fileUploader;
            _securityContext = securityContext;
            _notifyClient = notifyClient;
            _filesSettingsHelper = filesSettingsHelper;
        }


        /// <summary>
        ///   Returns the list of all events matching the parameters specified in the request
        /// </summary>
        /// <short>
        ///   Get event list
        /// </short>
        /// <category>History</category>
        /// <param optional="true" name="entityType" remark="Allowed values: opportunity, contact or case">Related entity type</param>
        /// <param optional="true" name="entityId">Related entity ID</param>
        /// <param optional="true" name="categoryId">Task category ID</param>
        /// <param optional="true" name="createBy">Event author</param>
        /// <param optional="true" name="fromDate">Earliest task due date</param>
        /// <param optional="true" name="toDate">Latest task due date</param>
        /// <returns>
        ///   Event list
        /// </returns>
        [Read(@"history/filter")]
        public Task<IEnumerable<RelationshipEventDto>> GetHistoryAsync(
            string entityType,
            int entityId,
            int categoryId,
            Guid createBy,
            ApiDateTime fromDate,
            ApiDateTime toDate)
        {
            var entityTypeObj = ToEntityType(entityType);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                    var contact = _daoFactory.GetContactDao().GetByID(entityId);
                    if (contact == null || !_crmSecurity.CanAccessTo(contact))
                        throw new ItemNotFoundException();
                    break;
                case EntityType.Case:
                    var cases = _daoFactory.GetCasesDao().GetByID(entityId);
                    if (cases == null || !_crmSecurity.CanAccessTo(cases))
                        throw new ItemNotFoundException();
                    break;
                case EntityType.Opportunity:
                    var deal = _daoFactory.GetDealDao().GetByID(entityId);
                    if (deal == null || !_crmSecurity.CanAccessTo(deal))
                        throw new ItemNotFoundException();
                    break;
                default:
                    if (entityId != 0)
                    {
                        throw new ArgumentException();
                    }
                    break;
            }

            return InternalGetHistoryAsync(entityTypeObj, entityId, categoryId, createBy, fromDate, toDate);
        }

        private async Task<IEnumerable<RelationshipEventDto>> InternalGetHistoryAsync(
            EntityType entityTypeObj,
            int entityId,
            int categoryId,
            Guid createBy,
            ApiDateTime fromDate,
            ApiDateTime toDate)
        {
            RelationshipEventByType eventByType;

            IEnumerable<RelationshipEventDto> result;

            OrderBy eventOrderBy;

            if (ASC.CRM.Classes.EnumExtension.TryParse(_apiContext.SortBy, true, out eventByType))
            {
                eventOrderBy = new OrderBy(eventByType, !_apiContext.SortDescending);
            }
            else if (string.IsNullOrEmpty(_apiContext.SortBy))
            {
                eventOrderBy = new OrderBy(RelationshipEventByType.Created, false);
            }
            else
            {
                eventOrderBy = null;
            }

            if (eventOrderBy != null)
            {
                result = await ToListRelationshipEventDtoAsync(_daoFactory.GetRelationshipEventDao().GetItems(
                    _apiContext.FilterValue,
                    entityTypeObj,
                    entityId,
                    createBy,
                    categoryId,
                    fromDate,
                    toDate,
                    (int)_apiContext.StartIndex,
                    (int)_apiContext.Count,
                    eventOrderBy));

                _apiContext.SetDataPaginated();
                _apiContext.SetDataFiltered();
                _apiContext.SetDataSorted();
            }
            else
            {
                result = await ToListRelationshipEventDtoAsync(_daoFactory.GetRelationshipEventDao().GetItems(
                    _apiContext.FilterValue,
                    entityTypeObj,
                    entityId,
                    createBy,
                    categoryId,
                    fromDate,
                    toDate,
                    0,
                    0,
                    null));
            }

            return result;
        }

        /// <summary>
        ///     Deletes the event with the ID specified in the request and all the files associated with this event
        /// </summary>
        /// <short>
        ///     Delete event and related files
        /// </short>
        /// <category>History</category>
        /// <param name="id">Event ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Event
        /// </returns>
        [Delete(@"history/{id:int}")]
        public Task<RelationshipEventDto> DeleteHistoryAsync(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var item = _daoFactory.GetRelationshipEventDao().GetByID(id);
            if (item == null) throw new ItemNotFoundException();

            return InternalDeleteHistoryAsync(id, item);
        }

        private async Task<RelationshipEventDto> InternalDeleteHistoryAsync(int id, RelationshipEvent item)
        {
            var wrapper = _mapper.Map<RelationshipEventDto>(item);

            await _daoFactory.GetRelationshipEventDao().DeleteItemAsync(id);

            var messageAction = GetHistoryDeletedAction(item.EntityType, item.ContactID);
            var entityTitle = wrapper.Contact == null ? wrapper.Entity.EntityTitle : wrapper.Contact.DisplayName;
            _messageService.Send(messageAction, _messageTarget.Create(item.ID), entityTitle, wrapper.Category.Title);

            return wrapper;
        }

        /// <summary>
        /// Creates a text (.txt) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt</short>
        /// <category>Files</category>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>
        ///     File info
        /// </returns>
        [Create(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/files/text")]
        public Task<FileWrapper<int>> CreateTextFileAsync(
            [FromRoute] string entityType,
            [FromRoute] int entityid,
            [FromBody] RelationshipEventCreateTextFileRequestDto inDto)
        {
            var title = inDto.Title;
            var content = inDto.Content;

            if (title == null) throw new ArgumentNullException("title");
            if (content == null) throw new ArgumentNullException("content");

            return InternalCreateTextFileAsync(entityType, entityid, inDto);
        }

        private async Task<FileWrapper<int>> InternalCreateTextFileAsync(
            string entityType,
            int entityid,
            RelationshipEventCreateTextFileRequestDto inDto)
        {
            var title = inDto.Title;
            var content = inDto.Content;

            var folderid = await GetRootFolderIDAsync();

            FileWrapper<int> result;

            var extension = ".txt";
            if (!string.IsNullOrEmpty(content))
            {
                if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
                {
                    extension = ".html";
                }
            }

            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                title = title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension);
                result = await SaveFileAsync(folderid, memStream, title);
            }

            await AttachFilesAsync(entityType, entityid, new List<int> { (int)result.Id });

            return result;
        }

        /// <summary>
        /// Upload file 
        /// </summary>
        /// <short>Upload file</short>
        /// <category>Files</category>
        /// <remarks>
        /// <![CDATA[
        ///  Upload can be done in 2 different ways:
        ///  <ol>
        /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
        /// <li>Using standart multipart/form-data method</li>
        /// </ol>]]>
        /// </remarks>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="file" visible="false">Request Input stream</param>
        /// <param name="contentType" visible="false">Content-Type Header</param>
        /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
        /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
        /// <param name="storeOriginalFileFlag" visible="false">If True, upload documents in original formats as well</param>
        /// <returns>
        /// File info
        /// </returns>
        [Create(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/files/upload")]
        public async Task<FileWrapper<int>> UploadFileInCRMAsync([FromBody] UploadFileInCRMRequestDto inDto)
        {
            string entityType = inDto.EntityType;
            int entityid = inDto.Entityid;
            Stream file = inDto.File;
            ContentType contentType = inDto.ContentType;
            ContentDisposition contentDisposition = inDto.ContentDisposition;
            IEnumerable<IFormFile> files = inDto.Files;
            bool storeOriginalFileFlag = inDto.StoreOriginalFileFlag;

            _filesSettingsHelper.StoreOriginalFiles = storeOriginalFileFlag;

            var folderid = await GetRootFolderIDAsync();

            var fileNames = new List<string>();

            FileWrapper<int> uploadedFile = null;
            if (files != null && files.Any())
            {
                //For case with multiple files
                foreach (var postedFile in files)
                {
                    using var fileStream = postedFile.OpenReadStream();
                    uploadedFile = await SaveFileAsync(folderid, fileStream, postedFile.FileName);
                    fileNames.Add(uploadedFile.Title);
                }
            }
            else if (file != null)
            {
                uploadedFile = await SaveFileAsync(folderid, file, contentDisposition.FileName);
                fileNames.Add(uploadedFile.Title);
            }

            return uploadedFile;
        }

        private async Task<FileWrapper<int>> SaveFileAsync(int folderid, Stream file, string fileName)
        {
            var resultFile = await _fileUploader.ExecAsync<int>(folderid, fileName, file.Length, file);

            return await _fileWrapperHelper.GetAsync<int>(resultFile);
        }

        /// <summary>
        ///   Creates the event with the parameters specified in the request
        /// </summary>
        /// <short>
        ///   Create event
        /// </short>
        /// <category>History</category>
        /// <param optional="true" name="contactId">Contact ID</param>
        /// <param optional="true" name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param optional="true" name="entityId">Related entity ID</param>
        /// <remarks>
        /// <![CDATA[
        ///  You should obligatorily set the value for 'contactId' if 'entityId' is not set or the value for parameters 'entityId' and 'entityType' if 'contactId' is not set.
        /// ]]>
        /// </remarks>
        /// <param optional="false" name="content">Contents</param>
        /// <param optional="false" name="categoryId">Category ID</param>
        /// <param optional="true" name="created">Event creation date</param>
        /// <param optional="true" name="fileId">List of IDs of the files associated with the event</param>
        /// <param optional="true" name="notifyUserList">User field list</param>
        /// <returns>
        ///   Created event
        /// </returns>
        [Create(@"history")]
        public RelationshipEventDto AddHistoryTo([FromBody] AddHistoryToRequestDto inDto)
        {
            string entityType = inDto.EntityType;
            int entityId = inDto.EntityId;
            int contactId = inDto.ContactId;
            string content = inDto.Content;
            int categoryId = inDto.CategoryId;
            ApiDateTime created = inDto.Created;
            IEnumerable<int> fileId = inDto.FileId;
            IEnumerable<Guid> notifyUserList = inDto.NotifyUserList;

            if (!string.IsNullOrEmpty(entityType) &&
                !(
                     string.Equals(entityType, "opportunity", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(entityType, "case", StringComparison.OrdinalIgnoreCase))
                )
                throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            var entityTitle = "";
            if (contactId > 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(contactId);
                if (contact == null || !_crmSecurity.CanAccessTo(contact))
                    throw new ArgumentException();
                entityTitle = contact.GetTitle();
            }

            if (entityTypeObj == EntityType.Case)
            {
                var cases = _daoFactory.GetCasesDao().GetByID(entityId);
                if (cases == null || !_crmSecurity.CanAccessTo(cases))
                    throw new ArgumentException();
                if (contactId <= 0)
                {
                    entityTitle = cases.Title;
                }
            }
            if (entityTypeObj == EntityType.Opportunity)
            {
                var deal = _daoFactory.GetDealDao().GetByID(entityId);
                if (deal == null || !_crmSecurity.CanAccessTo(deal))
                    throw new ArgumentException();
                if (contactId <= 0)
                {
                    entityTitle = deal.Title;
                }
            }

            var relationshipEvent = new RelationshipEvent
            {
                CategoryID = categoryId,
                EntityType = entityTypeObj,
                EntityID = entityId,
                Content = content,
                ContactID = contactId,
                CreateOn = created,
                CreateBy = _securityContext.CurrentAccount.ID
            };

            var category = _daoFactory.GetListItemDao().GetByID(categoryId);
            if (category == null) throw new ArgumentException();

            var item = _daoFactory.GetRelationshipEventDao().CreateItem(relationshipEvent);


            notifyUserList = notifyUserList != null ? notifyUserList.ToList() : new List<Guid>();
            var needNotify = notifyUserList.Any();

            var fileListInfoHashtable = new Hashtable();

            if (fileId != null)
            {
                var fileIds = fileId.ToList();
                var files = _filesDaoFactory.GetFileDao<int>().GetFilesAsync(fileIds.ToArray()).ToListAsync().Result;

                if (needNotify)
                {
                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file.Title);
                        if (extension == null) continue;

                        var fileInfo = $"{file.Title} ({extension.ToUpper()})";
                        if (!fileListInfoHashtable.ContainsKey(fileInfo))
                        {
                            fileListInfoHashtable.Add(fileInfo, file.DownloadUrl);
                        }
                        else
                        {
                            fileInfo = $"{file.Title} ({extension.ToUpper()}, {file.UniqID})";
                            fileListInfoHashtable.Add(fileInfo, file.DownloadUrl);
                        }
                    }
                }

                _daoFactory.GetRelationshipEventDao().AttachFiles(item.ID, fileIds.ToArray());

                if (files.Any())
                {
                    var fileAttachAction = GetFilesAttachAction(entityTypeObj, contactId);

                    _messageService.Send(fileAttachAction, _messageTarget.Create(item.ID), entityTitle, files.Select(x => x.Title));
                }
            }

            if (needNotify)
            {
                _notifyClient.SendAboutAddRelationshipEventAdd(item, fileListInfoHashtable, _daoFactory, notifyUserList.ToArray());
            }

            var historyCreatedAction = GetHistoryCreatedAction(entityTypeObj, contactId);

            _messageService.Send(historyCreatedAction, _messageTarget.Create(item.ID), entityTitle, category.Title);

            return _mapper.Map<RelationshipEventDto>(item);
        }

        /// <summary>
        ///     Associates the selected file(s) with the entity with the ID or type specified in the request
        /// </summary>
        /// <short>
        ///     Associate file with entity
        /// </short>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="fileids">List of IDs of the files</param>
        /// <category>Files</category>
        /// <returns>Entity with the file attached</returns>
        [Create(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/files")]
        public Task<RelationshipEventDto> AttachFilesAsync(
            [FromRoute] string entityType,
            [FromRoute] int entityid,
            [FromBody] IEnumerable<int> fileids)
        {
            if (entityid <= 0 || fileids == null) throw new ArgumentException();

            return InternalAttachFilesAsync(entityType, entityid, fileids);
        }

        private async Task<RelationshipEventDto> InternalAttachFilesAsync(
            string entityType,
            int entityid,
            IEnumerable<int> fileids)
        {
            var files = await _filesDaoFactory.GetFileDao<int>().GetFilesAsync(fileids.ToArray()).ToListAsync();

            var folderid = await GetRootFolderIDAsync();

            if (files.Exists(file => file.FolderID.ToString() != folderid.ToString()))
                throw new ArgumentException("invalid file folder");

            var entityTypeObj = ToEntityType(entityType);

            DomainObject entityObj;

            var entityTitle = GetEntityTitle(entityTypeObj, entityid, true, out entityObj);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                    var relationshipEvent1 = _daoFactory.GetRelationshipEventDao().AttachFiles(entityid, EntityType.Any, 0, fileids.ToArray());
                    var messageAction = entityObj is Company ? MessageAction.CompanyAttachedFiles : MessageAction.PersonAttachedFiles;
                    _messageService.Send(messageAction, _messageTarget.Create(entityid), entityTitle, files.Select(x => x.Title));
                    return _mapper.Map<RelationshipEventDto>(relationshipEvent1);
                case EntityType.Opportunity:
                    var relationshipEvent2 = _daoFactory.GetRelationshipEventDao().AttachFiles(0, entityTypeObj, entityid, fileids.ToArray());
                    _messageService.Send(MessageAction.OpportunityAttachedFiles, _messageTarget.Create(entityid), entityTitle, files.Select(x => x.Title));
                    return _mapper.Map<RelationshipEventDto>(relationshipEvent2);
                case EntityType.Case:
                    var relationshipEvent3 = _daoFactory.GetRelationshipEventDao().AttachFiles(0, entityTypeObj, entityid, fileids.ToArray());
                    _messageService.Send(MessageAction.CaseAttachedFiles, _messageTarget.Create(entityid), entityTitle, files.Select(x => x.Title));
                    return _mapper.Map<RelationshipEventDto>(relationshipEvent3);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Returns the ID for the root folder used to store the files for the CRM module
        /// </summary>
        /// <short>Get root folder ID</short> 
        /// <category>Files</category>
        /// <returns>
        ///   Root folder ID
        /// </returns>
        [Read(@"files/root")]
        public Task<int> GetRootFolderIDAsync()
        {
            return _daoFactory.GetFileDao().GetRootAsync();
        }

        /// <summary>
        ///    Returns the list of all files for the entity with the ID or type specified in the request
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <short>Get file list</short> 
        /// <category>Files</category>
        /// <returns>
        ///    File list
        /// </returns>
        [Read(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/files")]
        public IAsyncEnumerable<FileWrapper<int>> GetFiles(string entityType, int entityid)
        {
            if (entityid <= 0) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                    return _daoFactory.GetRelationshipEventDao().GetAllFilesAsync(new[] { entityid }, EntityType.Any, 0).SelectAwait(async file => await _fileWrapperHelper.GetAsync<int>(file));
                case EntityType.Opportunity:
                case EntityType.Case:
                    return _daoFactory.GetRelationshipEventDao().GetAllFilesAsync(null, entityTypeObj, entityid).SelectAwait(async file => await _fileWrapperHelper.GetAsync<int>(file));
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Deletes the file with the ID specified in the request
        /// </summary>
        /// <short>Delete file</short> 
        /// <category>Files</category>
        /// <param name="fileid">File ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    File Info
        /// </returns>
        [Delete(@"files/{fileid:int}")]
        public Task<FileWrapper<int>> DeleteCRMFileAsync(int fileid)
        {
            if (fileid < 0) throw new ArgumentException();

            return InternalDeleteCRMFileAsync(fileid);
        }

        private async Task<FileWrapper<int>> InternalDeleteCRMFileAsync(int fileid)
        {
            var file = await _filesDaoFactory.GetFileDao<int>().GetFileAsync(fileid);
            if (file == null) throw new ItemNotFoundException();
            var result = _fileWrapperHelper.GetAsync(file);

            var _eventsDao = _daoFactory.GetRelationshipEventDao();
            var eventIDs = _eventsDao.RemoveFileAsync(file);
            var events = new List<RelationshipEvent>();

            await eventIDs.ForEachAsync(id => events.Add(_eventsDao.GetByID(id)));

            foreach (var evt in events)
            {
                DomainObject entityObj;
                var entityTitle = evt.ContactID > 0
                                  ? GetEntityTitle(EntityType.Contact, evt.ContactID, false, out entityObj)
                                  : GetEntityTitle(evt.EntityType, evt.EntityID, false, out entityObj);
                var messageAction = GetFilesDetachAction(evt.EntityType, evt.ContactID);

                _messageService.Send(messageAction, _messageTarget.Create(file.ID), entityTitle, file.Title);
            }

            return await result;
        }

        private Task<IEnumerable<RelationshipEventDto>> ToListRelationshipEventDtoAsync(List<RelationshipEvent> itemList)
        {
            if (itemList.Count == 0) return System.Threading.Tasks.Task.FromResult<IEnumerable<RelationshipEventDto>>(new List<RelationshipEventDto>());

            var contactIDs = new List<int>();
            var eventIDs = new List<int>();
            var categoryIDs = new List<int>();
            var entityDtosIDs = new Dictionary<EntityType, List<int>>();


            foreach (var item in itemList)
            {
                eventIDs.Add(item.ID);

                if (!categoryIDs.Contains(item.CategoryID))
                {
                    categoryIDs.Add(item.CategoryID);
                }

                if (item.ContactID > 0 && !contactIDs.Contains(item.ContactID))
                {
                    contactIDs.Add(item.ContactID);
                }

                if (item.EntityID <= 0) continue;

                if (!entityDtosIDs.ContainsKey(item.EntityType))
                {
                    entityDtosIDs.Add(item.EntityType, new List<int>
                        {
                            item.EntityID
                        });
                }
                else if (!entityDtosIDs[item.EntityType].Contains(item.EntityID))
                {
                    entityDtosIDs[item.EntityType].Add(item.EntityID);
                }
            }

            var entityDtos = new Dictionary<string, EntityDto>();

            foreach (var entityType in entityDtosIDs.Keys)
            {
                switch (entityType)
                {
                    case EntityType.Opportunity:
                        _daoFactory.GetDealDao().GetDeals(entityDtosIDs[entityType].Distinct().ToArray())
                                  .ForEach(item =>
                                      {
                                          if (item == null) return;

                                          entityDtos.Add(
                                              string.Format("{0}_{1}", (int)entityType, item.ID),
                                              new EntityDto
                                              {
                                                  EntityId = item.ID,
                                                  EntityTitle = item.Title,
                                                  EntityType = "opportunity"
                                              });
                                      });
                        break;
                    case EntityType.Case:
                        _daoFactory.GetCasesDao().GetByID(entityDtosIDs[entityType].ToArray())
                                  .ForEach(item =>
                                      {
                                          if (item == null) return;

                                          entityDtos.Add(
                                              string.Format("{0}_{1}", (int)entityType, item.ID),
                                              new EntityDto
                                              {
                                                  EntityId = item.ID,
                                                  EntityTitle = item.Title,
                                                  EntityType = "case"
                                              });
                                      });
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            return InternalToListRelationshipEventDtoAsync(itemList, entityDtos, categoryIDs, eventIDs, contactIDs);
        }

        private async Task<IEnumerable<RelationshipEventDto>> InternalToListRelationshipEventDtoAsync(
            List<RelationshipEvent> itemList, Dictionary<string, EntityDto> entityDtos, List<int> categoryIDs, List<int> eventIDs, List<int> contactIDs)
        {

            var result = new List<RelationshipEventDto>();

            var categories = _daoFactory.GetListItemDao().GetItems(categoryIDs.ToArray()).ToDictionary(x => x.ID, x => _mapper.Map<HistoryCategoryDto>(x));

            var filesTask = _daoFactory.GetRelationshipEventDao().GetFilesAsync(eventIDs.ToArray());

            var contacts = _daoFactory.GetContactDao().GetContacts(contactIDs.ToArray()).ToDictionary(item => item.ID, x => _mapper.Map<ContactBaseDto>(x));

            foreach (var item in itemList)
            {
                var eventObjWrap = _mapper.Map<RelationshipEventDto>(item);

                if (contacts.ContainsKey(item.ContactID))
                {
                    eventObjWrap.Contact = contacts[item.ContactID];
                }

                if (item.EntityID > 0)
                {
                    var entityStrKey = string.Format("{0}_{1}", (int)item.EntityType, item.EntityID);

                    if (entityDtos.ContainsKey(entityStrKey))
                    {
                        eventObjWrap.Entity = entityDtos[entityStrKey];
                    }
                }

                List<FileWrapper<int>> tmpList;
                var files = await filesTask;
                if (files.ContainsKey(item.ID))
                {
                    tmpList = new List<FileWrapper<int>>(files[item.ID].Count);
                    foreach (var file in files[item.ID])
                    {
                        tmpList.Add(await _fileWrapperHelper.GetAsync(file));
                    }
                }
                else
                    tmpList = new List<FileWrapper<int>>();

                eventObjWrap.Files = tmpList;

                if (categories.ContainsKey(item.CategoryID))
                {
                    eventObjWrap.Category = categories[item.CategoryID];
                }

                result.Add(eventObjWrap);
            }

            return result;
        }



        private MessageAction GetHistoryCreatedAction(EntityType entityType, int contactId)
        {
            if (contactId > 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(contactId);
                return contact is Company ? MessageAction.CompanyCreatedHistoryEvent : MessageAction.PersonCreatedHistoryEvent;
            }

            switch (entityType)
            {
                case EntityType.Opportunity:
                    return MessageAction.OpportunityCreatedHistoryEvent;
                case EntityType.Case:
                    return MessageAction.CaseCreatedHistoryEvent;
                case EntityType.Any:
                    var contact = _daoFactory.GetContactDao().GetByID(contactId);
                    return contact is Company ? MessageAction.CompanyCreatedHistoryEvent : MessageAction.PersonCreatedHistoryEvent;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private MessageAction GetHistoryDeletedAction(EntityType entityType, int contactId)
        {
            if (contactId > 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(contactId);
                return contact is Company ? MessageAction.CompanyDeletedHistoryEvent : MessageAction.PersonDeletedHistoryEvent;
            }

            switch (entityType)
            {
                case EntityType.Opportunity:
                    return MessageAction.OpportunityDeletedHistoryEvent;
                case EntityType.Case:
                    return MessageAction.CaseDeletedHistoryEvent;
                case EntityType.Any:
                    var contact = _daoFactory.GetContactDao().GetByID(contactId);
                    return contact is Company ? MessageAction.CompanyDeletedHistoryEvent : MessageAction.PersonDeletedHistoryEvent;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private MessageAction GetFilesAttachAction(EntityType entityType, int contactId)
        {
            if (contactId > 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(contactId);
                return contact is Company ? MessageAction.CompanyAttachedFiles : MessageAction.PersonAttachedFiles;
            }

            switch (entityType)
            {
                case EntityType.Opportunity:
                    return MessageAction.OpportunityAttachedFiles;
                case EntityType.Case:
                    return MessageAction.CaseAttachedFiles;
                case EntityType.Any:
                    var contact = _daoFactory.GetContactDao().GetByID(contactId);
                    return contact is Company ? MessageAction.CompanyAttachedFiles : MessageAction.PersonAttachedFiles;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private MessageAction GetFilesDetachAction(EntityType entityType, int contactId)
        {
            if (contactId > 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(contactId);
                return contact is Company ? MessageAction.CompanyDetachedFile : MessageAction.PersonDetachedFile;
            }

            switch (entityType)
            {
                case EntityType.Opportunity:
                    return MessageAction.OpportunityDetachedFile;
                case EntityType.Case:
                    return MessageAction.CaseDetachedFile;
                case EntityType.Any:
                    var contact = _daoFactory.GetContactDao().GetByID(contactId);
                    return contact is Company ? MessageAction.CompanyDetachedFile : MessageAction.PersonAttachedFiles;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }
    }
}