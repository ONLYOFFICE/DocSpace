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

using ASC.Api.CRM;
using ASC.Common.Web;
using ASC.CRM.ApiModels;

using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.MessagingSystem.Core;
using ASC.MessagingSystem.Models;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class CustomFieldsController : BaseApiController
    {
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;

        public CustomFieldsController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _messageTarget = messageTarget;
            _messageService = messageService;
        }


        /// <summary>
        ///    Returns the list of descriptions for all existing user fields
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Type</param>
        /// <short>Get user field list</short> 
        /// <category>User fields</category>
        ///<returns>
        ///    User field list
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"{entityType:regex(contact|person|company|opportunity|case)}/customfield/definitions")]
        public IEnumerable<CustomFieldDto> GetCustomFieldDefinitions(string entityType)
        {
            return _mapper.Map<List<CustomField>, List<CustomFieldDto>>(_daoFactory.GetCustomFieldDao().GetFieldsDescription(ToEntityType(entityType)));
        }

        /// <summary>
        ///   Returns the list of all user field values using the entity type and entity ID specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Type</param>
        /// <param name="entityid">ID</param>
        /// <short>Get user field values</short> 
        /// <category>User fields</category>
        /// <returns></returns>
        [HttpGet(@"{entityType:regex(contact|person|company|opportunity|case)}/{entityid:int}/customfield")]
        public IEnumerable<CustomFieldBaseDto> GetCustomFieldForSubject(string entityType, int entityid)
        {
            return _mapper.Map<List<CustomField>, List<CustomFieldDto>>(_daoFactory.GetCustomFieldDao().GetEnityFields(ToEntityType(entityType), entityid, false));
        }

        /// <summary>
        ///    Sets the new user field value using the entity type, ID, field ID and value specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Type</param>
        /// <param name="entityid">ID</param>
        /// <param name="fieldid">Field ID</param>
        /// <param name="fieldValue">Field Value</param>
        /// <short>Set user field value</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        [HttpPost(@"{entityType:regex(contact|person|company|opportunity|case)}/{entityid:int}/customfield/{fieldid:int}")]
        public CustomFieldBaseDto SetEntityCustomFieldValue(
            [FromRoute] string entityType,
            [FromRoute] int entityid,
            [FromRoute] int fieldid,
            [FromBody] string fieldValue)
        {
            var customField = _daoFactory.GetCustomFieldDao().GetFieldDescription(fieldid);

            var entityTypeStr = ToEntityType(entityType);

            customField.EntityID = entityid;
            customField.Value = fieldValue;

            _daoFactory.GetCustomFieldDao().SetFieldValue(entityTypeStr, entityid, fieldid, fieldValue);

            return _mapper.Map<CustomFieldBaseDto>(customField);
        }

        /// <summary>
        ///    Creates a new user field with the parameters (entity type, field title, type, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Entity type</param>
        /// <param optional="false" name="label">Field title</param>
        /// <param name="fieldType" 
        /// remark="Allowed values: TextField, TextArea, SelectBox, CheckBox, Heading or Date">
        ///   User field value
        /// </param>
        /// <param optional="true" name="position">Field position</param>
        /// <param optional="true" name="mask" remark="Sent in json format only" >Mask</param>
        /// <short>Create user field</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        ///<example>
        /// <![CDATA[
        /// 
        /// Data transfer in application/json format:
        /// 
        /// 1) Creation of a user field of  TextField type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample TextField",
        ///    fieldType: 0,
        ///    position: 0,
        ///    mask: {"size":"40"}        - this is the text field size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 2) Creation of a user field of TextArea type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample TextArea",
        ///    fieldType: 1,
        ///    position: 1,
        ///    mask: '{"rows":"2","cols":"30"}'        - this is the TextArea size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 3) Creation of a user field of   SelectBox type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample SelectBox",
        ///    fieldType: 2,
        ///    position: 0,
        ///    mask: ["1","2","3"]   - SelectBox values.
        /// }
        /// 
        /// 
        /// 
        /// 4) Creation of a user field of  CheckBox type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample CheckBox",
        ///    fieldType: 3,
        ///    position: 0,
        ///    mask: ""     
        /// }
        /// 
        /// 
        /// 
        /// 5) Creation of a user field of   Heading type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample Heading",
        ///    fieldType: 4,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// 
        /// 6) Creation of a user field of   Date type
        /// 
        /// data: {
        ///    entityType: "contact",
        ///    label: "Sample Date",
        ///    fieldType: 5,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// ]]>
        /// </example>
        [HttpPost(@"{entityType:regex(contact|person|company|opportunity|case)}/customfield")]
        public CustomFieldDto CreateCustomFieldValue(
            [FromRoute] string entityType,
            [FromBody] CreateOrUpdateCustomFieldValueRequestDto inDto
         )
        {

            var label = inDto.Label;
            var fieldType = inDto.FieldType;
            var position = inDto.Position;
            var mask = inDto.Mask;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            var entityTypeObj = ToEntityType(entityType);
            var fieldID = _daoFactory.GetCustomFieldDao().CreateField(entityTypeObj, label, (CustomFieldType)fieldType, mask);
            var wrapper = _daoFactory.GetCustomFieldDao().GetFieldDescription(fieldID);

            var messageAction = GetCustomFieldCreatedAction(entityTypeObj);
            _messageService.Send(messageAction, _messageTarget.Create(wrapper.ID), wrapper.Label);

            return _mapper.Map<CustomFieldDto>(_daoFactory.GetCustomFieldDao().GetFieldDescription(fieldID));
        }

        /// <summary>
        ///    Updates the selected user field with the parameters (entity type, field title, type, etc.) specified in the request
        /// </summary>
        /// <param name="id">User field id</param>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Entity type</param>
        /// <param optional="false" name="label">Field title</param>
        /// <param name="fieldType" 
        /// remark="Allowed values: 0 (TextField),1 (TextArea),2 (SelectBox),3 (CheckBox),4 (Heading) or 5 (Date)">
        ///   User field value
        /// </param>
        /// <param optional="true" name="position">Field position</param>
        /// <param optional="true" name="mask" remark="Sent in json format only" >Mask</param>
        /// <short> Updates the selected user field</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        ///<remarks>
        /// <![CDATA[
        ///  You can update field if there is no related elements. If such elements exist there will be updated only label and mask, other parameters will be ignored.
        /// ]]>
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"{entityType:regex(contact|person|company|opportunity|case)}/customfield/{id:int}")]
        public CustomFieldDto UpdateCustomFieldValue(int id, string entityType, string label, int fieldType, int position, string mask)
        {
            if (id <= 0) throw new ArgumentException();
            if (!_daoFactory.GetCustomFieldDao().IsExist(id)) throw new ItemNotFoundException();

            var entityTypeObj = ToEntityType(entityType);

            var customField = new CustomField
            {
                EntityType = entityTypeObj,
                Type = (CustomFieldType)fieldType,
                ID = id,
                Mask = mask,
                Label = label,
                SortOrder = position
            };

            _daoFactory.GetCustomFieldDao().EditItem(customField);

            customField = _daoFactory.GetCustomFieldDao().GetFieldDescription(id);

            var messageAction = GetCustomFieldUpdatedAction(entityTypeObj);
            _messageService.Send(messageAction, _messageTarget.Create(customField.ID), customField.Label);

            return _mapper.Map<CustomFieldDto>(customField);
        }

        /// <summary>
        ///    Deletes the user field with the ID specified in the request
        /// </summary>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Type</param>
        /// <param name="fieldid">Field ID</param>
        /// <short>Delete user field</short> 
        /// <category>User fields</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    User field
        /// </returns>
        [HttpDelete(@"{entityType:regex(contact|person|company|opportunity|case)}/customfield/{fieldid:int}")]
        public CustomFieldDto DeleteCustomField(string entityType, int fieldid)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();
            if (fieldid <= 0) throw new ArgumentException();

            var customField = _daoFactory.GetCustomFieldDao().GetFieldDescription(fieldid);
            if (customField == null) throw new ItemNotFoundException();

            var result = _mapper.Map<CustomFieldDto>(customField);

            _daoFactory.GetCustomFieldDao().DeleteField(fieldid);

            var messageAction = GetCustomFieldDeletedAction(ToEntityType(entityType));
            _messageService.Send(messageAction, _messageTarget.Create(customField.ID), result.Label);

            return result;
        }

        /// <summary>
        ///    Updates user fields order
        /// </summary>
        /// <param name="fieldids">User field ID list</param>
        /// <param name="entityType" remark="Allowed values: contact,person,company,opportunity,case">Entity type</param>
        /// <category>User fields</category>
        /// <returns>
        ///    User fields
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"{entityType:regex(contact|person|company|opportunity|case)}/customfield/reorder")]
        public IEnumerable<CustomFieldBaseDto> UpdateCustomFieldsOrder(IEnumerable<int> fieldids, string entityType)
        {
            if (fieldids == null) throw new ArgumentException();
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            var customFields = new List<CustomField>();
            foreach (var id in fieldids)
            {
                if (!_daoFactory.GetCustomFieldDao().IsExist(id)) throw new ItemNotFoundException();
                customFields.Add(_daoFactory.GetCustomFieldDao().GetFieldDescription(id));
            }

            _daoFactory.GetCustomFieldDao().ReorderFields(fieldids.ToArray());

            var messageAction = GetCustomFieldsUpdatedOrderAction(ToEntityType(entityType));
            _messageService.Send(messageAction, _messageTarget.Create(fieldids), customFields.Select(x => x.Label));

            return _mapper.Map<List<CustomField>, List<CustomFieldDto>>(customFields);
        }

        private static MessageAction GetCustomFieldCreatedAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactUserFieldCreated;
                case EntityType.Person:
                    return MessageAction.PersonUserFieldCreated;
                case EntityType.Company:
                    return MessageAction.CompanyUserFieldCreated;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityUserFieldCreated;
                case EntityType.Case:
                    return MessageAction.CaseUserFieldCreated;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private static MessageAction GetCustomFieldUpdatedAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactUserFieldUpdated;
                case EntityType.Person:
                    return MessageAction.PersonUserFieldUpdated;
                case EntityType.Company:
                    return MessageAction.CompanyUserFieldUpdated;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityUserFieldUpdated;
                case EntityType.Case:
                    return MessageAction.CaseUserFieldUpdated;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private static MessageAction GetCustomFieldDeletedAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactUserFieldDeleted;
                case EntityType.Person:
                    return MessageAction.PersonUserFieldDeleted;
                case EntityType.Company:
                    return MessageAction.CompanyUserFieldDeleted;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityUserFieldDeleted;
                case EntityType.Case:
                    return MessageAction.CaseUserFieldDeleted;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

        private static MessageAction GetCustomFieldsUpdatedOrderAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactUserFieldsUpdatedOrder;
                case EntityType.Person:
                    return MessageAction.PersonUserFieldsUpdatedOrder;
                case EntityType.Company:
                    return MessageAction.CompanyUserFieldsUpdatedOrder;
                case EntityType.Opportunity:
                    return MessageAction.OpportunityUserFieldsUpdatedOrder;
                case EntityType.Case:
                    return MessageAction.CaseUserFieldsUpdatedOrder;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }
    }
}