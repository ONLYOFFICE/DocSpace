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

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Common.Web;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.MessagingSystem.Core;
using ASC.MessagingSystem.Models;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class TagsController : BaseApiController
    {
        private readonly ApiContext _apiContext;
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;

        public TagsController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     ApiContext apiContext,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     IMapper mapper)
          : base(daoFactory, crmSecurity, mapper)
        {
            _apiContext = apiContext;
            _messageTarget = messageTarget;
            _messageService = messageService;
        }



        /// <summary>
        ///  Returns the list of all tags associated with the entity with the ID and type specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <short>Get entity tags</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"{entityType:regex(contact|opportunity|case)}/tag/{entityid:int}")]
        public IEnumerable<string> GetEntityTags(string entityType, int entityid)
        {
            if (string.IsNullOrEmpty(entityType) || entityid <= 0) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                case EntityType.Person:
                case EntityType.Company:
                    var contact = _daoFactory.GetContactDao().GetByID(entityid);
                    if (contact == null || !_crmSecurity.CanAccessTo(contact))
                        throw new ItemNotFoundException();
                    break;
                case EntityType.Case:
                    var cases = _daoFactory.GetCasesDao().GetByID(entityid);
                    if (cases == null || !_crmSecurity.CanAccessTo(cases))
                        throw new ItemNotFoundException();
                    break;
                case EntityType.Opportunity:
                    var deal = _daoFactory.GetDealDao().GetByID(entityid);
                    if (deal == null || !_crmSecurity.CanAccessTo(deal))
                        throw new ItemNotFoundException();
                    break;
            }

            return _daoFactory.GetTagDao().GetEntityTags(entityTypeObj, entityid);
        }

        /// <summary>
        ///    Returns the list of all tags for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <short>Get all contact tags</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   List of contact tags
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"contact/{contactid:int}/tag")]
        public IEnumerable<string> GetContactTags(int contactid)
        {
            if (contactid <= 0) throw new ArgumentException();
            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanAccessTo(contact))
                throw new ItemNotFoundException();
            return _daoFactory.GetTagDao().GetEntityTags(EntityType.Contact, contactid);
        }

        /// <summary>
        ///  Creates the tag for the selected entity with the tag name specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <param name="tagName">Tag name</param>
        /// <short>Create tag</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpPost(@"{entityType:regex(contact|opportunity|case)}/tag")]
        public string CreateTag([FromRoute] string entityType, [FromBody] string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            var messageAction = GetEntityTagCreatedAction(entityTypeObj);
            _daoFactory.GetTagDao().AddTag(entityTypeObj, tagName);

            _messageService.Send(messageAction, tagName);

            return tagName;
        }

        /// <summary>
        ///  Returns the list of all tags associated with the entity type specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <short>Get tags for entity type</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"{entityType:regex(contact|opportunity|case)}/tag")]
        public IEnumerable<TagDto> GetAllTags(string entityType)
        {
            if (string.IsNullOrEmpty(entityType)) throw new ArgumentException();
            var entType = ToEntityType(entityType);

            var tagTitles = _daoFactory.GetTagDao().GetAllTags(entType).ToList();
            var relativeItemsCountArrayJSON = _daoFactory.GetTagDao().GetTagsLinkCount(entType).ToList();
            if (tagTitles.Count != relativeItemsCountArrayJSON.Count) throw new ArgumentException();

            var result = new List<TagDto>();
            for (var i = 0; i < tagTitles.Count; i++)
            {
                result.Add(new TagDto(tagTitles[i], relativeItemsCountArrayJSON[i]));
            }
            return result.OrderBy(x => x.Title.Trim(), StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        ///    Adds a group of tags to the entity with the ID specified in the request
        /// </summary>
        /// <short>Add tag group to entity</short> 
        /// <category>Tags</category>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Tag type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Tag
        /// </returns> 
        [HttpPost(@"{entityType:regex(contact|opportunity|case)}/taglist")]
        public string AddTagToBatch([FromRoute] string entityType,
                                    [FromBody] AddTagToBatchRequestDto inDto)
        {
            var entityid = inDto.Entityid;
            var tagName = inDto.TagName;

            var ids = entityid.ToList();
            if (entityid == null || !ids.Any()) throw new ArgumentException();

            foreach (var entityID in ids)
            {
                AddTagTo(entityType, entityID, tagName);
            }
            return tagName;
        }

        /// <summary>
        ///    Adds the selected tag to the group of contacts with the parameters specified in the request
        /// </summary>
        /// <short>Add tag to contact group</short> 
        /// <category>Tags</category>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactStage">Contact stage ID (warmth)</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Tag
        /// </returns> 
        [HttpPost(@"contact/filter/taglist")]
        public string AddTagToBatchContacts(
         [FromBody] AddTagToBatchContactsRequestDto inDto)
        {
            var tags = inDto.Tags;
            var contactStage = inDto.ContactStage;
            var contactType = inDto.ContactType;
            var contactListView = inDto.ContactListView;
            var fromDate = inDto.FromDate;
            var toDate = inDto.ToDate;
            var tagName = inDto.TagName;

            var contacts = _daoFactory
                .GetContactDao()
                .GetContacts(_apiContext.FilterValue,
                             tags,
                             contactStage,
                             contactType,
                             contactListView,
                             fromDate,
                             toDate,
                             0,
                             0,
                             null).Where(_crmSecurity.CanEdit).ToList();

            foreach (var contact in contacts)
            {
                AddTagTo("contact", contact.ID, tagName);
            }

            return tagName;
        }

        /// <summary>
        ///    Adds the selected tag to the group of opportunities with the parameters specified in the request
        /// </summary>
        /// <short>Add tag to opportunity group</short> 
        /// <category>Tags</category>
        /// <param optional="true" name="responsibleid">Opportunity responsible</param>
        /// <param optional="true" name="opportunityStagesid">Opportunity stage ID</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="contactAlsoIsParticipant">Participation status: take into account opportunities where the contact is a participant or not</param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <param optional="true" name="stageType" remark="Allowed values: {Open, ClosedAndWon, ClosedAndLost}">Opportunity stage type</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Tag
        /// </returns> 
        [HttpPost(@"opportunity/filter/taglist")]
        public string AddTagToBatchDeals(
         [FromBody]  AddTagToBatchDealsRequestDto inDto)
        {
            var responsibleid = inDto.Responsibleid;
            var opportunityStagesid = inDto.OpportunityStagesid;
            var tags = inDto.Tags;
            var contactid = inDto.Contactid;
            var stageType = inDto.StageType;
            var contactAlsoIsParticipant = inDto.ContactAlsoIsParticipant;
            var fromDate = inDto.FromDate;
            var toDate = inDto.ToDate;
            var tagName = inDto.TagName;

            var deals = _daoFactory
                .GetDealDao()
                .GetDeals(
                    _apiContext.FilterValue,
                    responsibleid,
                    opportunityStagesid,
                    tags,
                    contactid,
                    stageType,
                    contactAlsoIsParticipant,
                    fromDate, toDate, 0, 0, null).Where(_crmSecurity.CanAccessTo).ToList();

            foreach (var deal in deals)
            {
                AddTagTo("opportunity", deal.ID, tagName);
            }
            return tagName;
        }

        /// <summary>
        ///    Adds the selected tag to the group of cases with the parameters specified in the request
        /// </summary>
        /// <short>Add tag to case group</short> 
        /// <category>Tags</category>
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="isClosed">Case status</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Tag
        /// </returns> 
        [HttpPost(@"case/filter/taglist")]
        public string AddTagToBatchCases([FromBody] AddTagToBatchCasesRequestDto inDto)
        {
            var contactid = inDto.ContactId;
            var tagName = inDto.TagName;
            var tags = inDto.Tags;
            var isClosed = inDto.isClosed;

            var caseses = _daoFactory.GetCasesDao().GetCases(_apiContext.FilterValue, contactid, isClosed, tags, 0, 0, null)
                .Where(_crmSecurity.CanAccessTo).ToList();

            if (!caseses.Any()) return tagName;

            foreach (var casese in caseses)
            {
                AddTagTo("case", casese.ID, tagName);
            }

            return tagName;
        }

        /// <summary>
        ///  Deletes all the unused tags from the entities with the type specified in the request
        /// </summary>
        /// <short>Delete unused tags</short> 
        /// <category>Tags</category>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <returns>Tags</returns>
        [HttpDelete(@"{entityType:regex(contact|opportunity|case)}/tag/unused")]
        public IEnumerable<string> DeleteUnusedTag(string entityType)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var entityTypeObj = ToEntityType(entityType);

            var result = _daoFactory.GetTagDao().GetUnusedTags(entityTypeObj);

            _daoFactory.GetTagDao().DeleteUnusedTags(entityTypeObj);

            return new List<string>(result);
        }

        /// <summary>
        ///  Adds the selected tag to the entity with the type and ID specified in the request
        /// </summary>
        /// <short>Add tag</short> 
        /// <category>Tags</category>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Tag
        /// </returns> 
        [HttpPost(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/tag")]
        public string AddTagTo([FromRoute] string entityType, [FromRoute] int entityid, [FromBody] string tagName)
        {
            if (entityid <= 0 || string.IsNullOrEmpty(tagName)) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);
            DomainObject entityObj;
            var entityTitle = GetEntityTitle(entityTypeObj, entityid, true, out entityObj);

            if (entityTypeObj == EntityType.Contact && !_crmSecurity.CanEdit(entityObj as Contact)) throw _crmSecurity.CreateSecurityException();

            _daoFactory.GetTagDao().AddTagToEntity(entityTypeObj, entityid, tagName);

            var messageAction = GetTagCreatedAction(entityTypeObj, entityid);
            _messageService.Send(messageAction, _messageTarget.Create(entityid), entityTitle, tagName);

            return tagName;
        }

        /// <summary>
        ///   Adds the selected tag to the entity (company or person) specified in the request and to all related contacts
        /// </summary>
        /// <short>Add tag</short> 
        /// <param name="entityType" remark="Allowed values: company,person">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <category>Tags</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>
        ///   Tag
        /// </returns>
        [HttpPost(@"{entityType:regex(company|person)}/{entityid:int}/tag/group")]
        public string AddContactTagToGroup([FromRoute] string entityType, [FromRoute] int entityid, [FromBody] string tagName)
        {
            if (entityid <= 0 || string.IsNullOrEmpty(tagName)) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);
            if (entityTypeObj != EntityType.Company && entityTypeObj != EntityType.Person) throw new ArgumentException();

            var contactInst = _daoFactory.GetContactDao().GetByID(entityid);
            if (contactInst == null || !_crmSecurity.CanAccessTo(contactInst)) throw new ItemNotFoundException();

            if (contactInst is Person && entityTypeObj == EntityType.Company) throw new Exception(CRMErrorsResource.ContactIsNotCompany);
            if (contactInst is Company && entityTypeObj == EntityType.Person) throw new Exception(CRMErrorsResource.ContactIsNotPerson);


            var contactIDsToAddTag = new List<int>();


            if (contactInst is Company)
            {
                contactIDsToAddTag.Add(contactInst.ID);

                var members = _daoFactory.GetContactDao().GetMembersIDsAndShareType(contactInst.ID);

                foreach (var m in members)
                {
                    if (_crmSecurity.CanAccessTo(m.Key, EntityType.Person, m.Value, 0))
                    {
                        contactIDsToAddTag.Add(m.Key);
                    }
                }
            }
            else
            {
                var CompanyID = ((Person)contactInst).CompanyID;
                if (CompanyID != 0)
                {
                    var cnt = _daoFactory.GetContactDao().GetByID(CompanyID);
                    if (cnt != null && cnt is Company && _crmSecurity.CanAccessTo(cnt))
                    {
                        contactIDsToAddTag.Add(CompanyID);

                        var members = _daoFactory.GetContactDao().GetMembersIDsAndShareType(CompanyID);

                        foreach (var m in members)
                        {
                            if (_crmSecurity.CanAccessTo(m.Key, EntityType.Person, m.Value, 0))
                            {
                                contactIDsToAddTag.Add(m.Key);
                            }
                        }
                    }
                    else
                    {
                        contactIDsToAddTag.Add(contactInst.ID);
                    }
                }
                else
                {
                    contactIDsToAddTag.Add(contactInst.ID);
                }
            }

            _daoFactory.GetTagDao().AddTagToContacts(contactIDsToAddTag.ToArray(), tagName);


            var entityTitle = contactInst.GetTitle();
            var messageActions = GetTagCreatedGroupAction(entityTypeObj);
            foreach (var messageAction in messageActions)
            {
                _messageService.Send(messageAction, _messageTarget.Create(contactInst.ID), entityTitle, tagName);
            }

            return tagName;
        }

        /// <summary>
        ///   Deletes the selected tag from the entity with the type specified in the request
        /// </summary>
        /// <short>Delete tag</short> 
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <param name="tagName">Tag name</param>
        /// <category>Tags</category>
        /// <exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Tag
        /// </returns>
        [HttpDelete(@"{entityType:regex(contact|opportunity|case)}/tag")]
        public string DeleteTag(string entityType, string tagName)
        {
            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(tagName)) throw new ArgumentException();


            var entityTypeObj = ToEntityType(entityType);

            if (!_daoFactory.GetTagDao().IsExist(entityTypeObj, tagName)) throw new ItemNotFoundException();

            _daoFactory.GetTagDao().DeleteTag(entityTypeObj, tagName);

            var messageAction = GetEntityTagDeletedAction(entityTypeObj);
            _messageService.Send(messageAction, tagName);

            return tagName;
        }

        /// <summary>
        ///  Deletes the selected tag from the entity with the type and ID specified in the request
        /// </summary>
        /// <short>Remove tag</short> 
        /// <category>Tags</category>
        /// <param name="entityType" remark="Allowed values: contact,opportunity,case">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Tag
        /// </returns> 
        [HttpDelete(@"{entityType:regex(contact|opportunity|case)}/{entityid:int}/tag")]
        public string DeleteTagFrom(string entityType, int entityid, string tagName)
        {
            if (string.IsNullOrEmpty(entityType) || entityid <= 0 || string.IsNullOrEmpty(tagName)) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);
            DomainObject entityObj;
            var entityTitle = GetEntityTitle(entityTypeObj, entityid, true, out entityObj);

            if (entityTypeObj == EntityType.Contact && !_crmSecurity.CanEdit(entityObj as Contact)) throw _crmSecurity.CreateSecurityException();

            if (!_daoFactory.GetTagDao().IsExist(entityTypeObj, tagName)) throw new ItemNotFoundException();

            _daoFactory.GetTagDao().DeleteTagFromEntity(entityTypeObj, entityid, tagName);

            var messageAction = GetTagDeletedAction(entityTypeObj, entityid);

            _messageService.Send(messageAction, _messageTarget.Create(entityid), entityTitle, tagName);

            return tagName;
        }

        /// <summary>
        ///   Deletes the selected tag from the entity (company or person) specified in the request and from all related contacts
        /// </summary>
        /// <short>Delete tag</short> 
        /// <param name="entityType" remark="Allowed values: company,person">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <category>Tags</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>
        ///   Tag
        /// </returns>
        [HttpDelete(@"{entityType:regex(company|person)}/{entityid:int}/tag/group")]
        public string DeleteContactTagFromGroup(string entityType, int entityid, string tagName)
        {
            if (entityid <= 0 || string.IsNullOrEmpty(tagName)) throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);
            if (entityTypeObj != EntityType.Company && entityTypeObj != EntityType.Person) throw new ArgumentException();

            var contactInst = _daoFactory.GetContactDao().GetByID(entityid);
            if (contactInst == null) throw new ItemNotFoundException();

            if (contactInst is Person && entityTypeObj == EntityType.Company) throw new Exception(CRMErrorsResource.ContactIsNotCompany);
            if (contactInst is Company && entityTypeObj == EntityType.Person) throw new Exception(CRMErrorsResource.ContactIsNotPerson);

            var contactIDsForDeleteTag = new List<int>();

            if (contactInst is Company)
            {
                contactIDsForDeleteTag.Add(contactInst.ID);

                var members = _daoFactory.GetContactDao().GetMembersIDsAndShareType(contactInst.ID);

                foreach (var m in members)
                {
                    if (_crmSecurity.CanAccessTo(m.Key, EntityType.Person, m.Value, 0))
                    {
                        contactIDsForDeleteTag.Add(m.Key);
                    }
                }
            }
            else
            {
                var CompanyID = ((Person)contactInst).CompanyID;
                if (CompanyID != 0)
                {
                    var cnt = _daoFactory.GetContactDao().GetByID(CompanyID);
                    if (cnt != null && cnt is Company && _crmSecurity.CanAccessTo(cnt))
                    {
                        contactIDsForDeleteTag.Add(CompanyID);

                        var members = _daoFactory.GetContactDao().GetMembersIDsAndShareType(CompanyID);

                        foreach (var m in members)
                        {
                            if (_crmSecurity.CanAccessTo(m.Key, EntityType.Person, m.Value, 0))
                            {
                                contactIDsForDeleteTag.Add(m.Key);
                            }
                        }
                    }
                    else
                    {
                        contactIDsForDeleteTag.Add(contactInst.ID);
                    }
                }
                else
                {
                    contactIDsForDeleteTag.Add(contactInst.ID);
                }
            }

            _daoFactory.GetTagDao().DeleteTagFromContacts(contactIDsForDeleteTag.ToArray(), tagName);

            return tagName;
        }


        private static MessageAction GetEntityTagCreatedAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactsCreatedTag;
                case EntityType.Opportunity:
                    return MessageAction.OpportunitiesCreatedTag;
                case EntityType.Case:
                    return MessageAction.CasesCreatedTag;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private static MessageAction GetEntityTagDeletedAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactsDeletedTag;
                case EntityType.Opportunity:
                    return MessageAction.OpportunitiesDeletedTag;
                case EntityType.Case:
                    return MessageAction.CasesDeletedTag;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private MessageAction GetTagCreatedAction(EntityType entityType, int entityId)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    var entity = _daoFactory.GetContactDao().GetByID(entityId);
                    return entity is Company ? MessageAction.CompanyCreatedTag : MessageAction.PersonCreatedTag;
                case EntityType.Company:
                    return MessageAction.CompanyCreatedTag;
                case EntityType.Person:
                    return MessageAction.PersonCreatedTag;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityCreatedTag;
                case EntityType.Case:
                    return MessageAction.CaseCreatedTag;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private static IEnumerable<MessageAction> GetTagCreatedGroupAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Company:
                    return new List<MessageAction> { MessageAction.CompanyCreatedTag, MessageAction.CompanyCreatedPersonsTag };
                case EntityType.Person:
                    return new List<MessageAction> { MessageAction.PersonCreatedTag, MessageAction.PersonCreatedCompanyTag };
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private MessageAction GetTagDeletedAction(EntityType entityType, int entityId)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    var entity = _daoFactory.GetContactDao().GetByID(entityId);
                    return entity is Company ? MessageAction.CompanyDeletedTag : MessageAction.PersonDeletedTag;
                case EntityType.Company:
                    return MessageAction.CompanyDeletedTag;
                case EntityType.Person:
                    return MessageAction.PersonDeletedTag;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityDeletedTag;
                case EntityType.Case:
                    return MessageAction.CaseDeletedTag;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }
    }
}