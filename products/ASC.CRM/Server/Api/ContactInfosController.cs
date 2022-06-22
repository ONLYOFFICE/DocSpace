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

using ASC.Api.CRM;
using ASC.Common.Web;
using ASC.CRM.ApiModels;
using ASC.CRM.Classes;
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
    public class ContactInfosController : BaseApiController
    {
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;

        public ContactInfosController(CrmSecurity crmSecurity,
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
        ///   Returns the list of all available contact categories
        /// </summary>
        /// <param name="infoType">
        ///    Contact information type
        /// </param>
        /// <short>Get all categories</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   List of all available contact categories
        /// </returns>
        [HttpGet(@"contact/data/{infoType}/category")]
        public IEnumerable<string> GetContactInfoCategory(ContactInfoType infoType)
        {
            return Enum.GetNames(ContactInfo.GetCategory(infoType));
        }

        /// <summary>
        ///   Returns the list of all available contact information types
        /// </summary>
        /// <short>Get all contact info types</short> 
        /// <category>Contacts</category>
        /// <returns></returns>
        [HttpGet(@"contact/data/infoType")]
        public IEnumerable<string> GetContactInfoType()
        {
            return Enum.GetNames(typeof(ContactInfoType));
        }

        /// <summary>
        ///    Returns the detailed information for the contact
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <short>Get contact information</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact information
        /// </returns>
        [HttpGet(@"contact/{contactid:int}/data")]
        public IEnumerable<ContactInfoDto> GetContactInfo(int contactid)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);

            if (contact == null || !_crmSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            var sourceData = _daoFactory.GetContactInfoDao().GetList(contactid, null, null, null)
                .OrderByDescending(info => info.ID)
                .ToList();

            return _mapper.Map<List<ContactInfo>, List<ContactInfoDto>>(sourceData);
        }

        /// <summary>
        ///   Returns the detailed list of all information available for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information ID</param>
        /// <short>Get contact info</short> 
        /// <category>Contacts</category>
        /// <returns>Contact information</returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"contact/{contactid:int}/data/{id:int}")]
        public ContactInfoDto GetContactInfoByID(int contactid, int id)
        {
            if (contactid <= 0 || id <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            var contactInfo = _daoFactory.GetContactInfoDao().GetByID(id);

            if (contactInfo == null || contactInfo.ContactID != contactid) throw new ArgumentException();

            return _mapper.Map<ContactInfoDto>(contactInfo);
        }

        /// <summary>
        ///    Adds the information with the parameters specified in the request to the contact with the selected ID
        /// </summary>
        ///<param name="contactid">Contact ID</param>
        ///<param name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param name="isPrimary">Contact importance: primary or not</param>
        ///<param   name="category">Category</param>
        ///<short> Add contact info</short> 
        ///<category>Contacts</category>
        /// <seealso cref="GetContactInfoType"/>
        /// <seealso cref="GetContactInfoCategory"/>
        /// <returns>
        ///    Contact information
        /// </returns> 
        ///<exception cref="ArgumentException"></exception>
        [HttpPost(@"contact/{contactid:int}/data")]
        public ContactInfoDto CreateContactInfo(
            [FromRoute] int contactid,
            [FromBody] CreateContactInfoRequestDto inDto)
        {

            var data = inDto.Data;
            var infoType = inDto.InfoType;
            var category = inDto.Category;
            var isPrimary = inDto.IsPrimary;

            if (string.IsNullOrEmpty(data) || contactid <= 0) throw new ArgumentException();
            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null) throw new ItemNotFoundException();

            if (infoType == ContactInfoType.Twitter)
            {
                if (!_crmSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();
            }
            else
            {
                if (!_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();
            }

            var categoryType = ContactInfo.GetCategory(infoType);
            if (!Enum.IsDefined(categoryType, category)) throw new ArgumentException();


            var contactInfo = new ContactInfo
            {
                Data = data,
                InfoType = infoType,
                ContactID = contactid,
                IsPrimary = isPrimary,
                Category = (int)Enum.Parse(categoryType, category)
            };

            if (contactInfo.InfoType == ContactInfoType.Address)
            {
                Address res;
                if (!Address.TryParse(contactInfo, out res))
                    throw new ArgumentException();
            }

            var contactInfoID = _daoFactory.GetContactInfoDao().Save(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            _messageService.Send(messageAction, _messageTarget.Create(contact.ID), contact.GetTitle());

            var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);

            contactInfoDto.Id = contactInfoID;

            return contactInfoDto;
        }

        /// <summary>
        ///    Adds the address information to the contact with the selected ID
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address data</param>
        /// <short>Add address info</short> 
        /// <category>Contacts</category>
        /// <seealso cref="GetContactInfoType"/>
        /// <seealso cref="GetContactInfoCategory"/>
        /// <returns>
        ///    Contact information
        /// </returns> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPost(@"contact/{contactid:int}/addressdata")]
        public ContactInfoDto CreateContactInfoAddress([FromRoute] int contactid, Address address)
        {
            if (contactid <= 0) throw new ArgumentException("Invalid value", "contactid");

            var contact = _daoFactory.GetContactDao().GetByID(contactid);

            if (contact == null || !_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            if (address == null) throw new ArgumentException("Value cannot be null", "address");

            if (!Enum.IsDefined(typeof(AddressCategory), address.Category)) throw new ArgumentException("Value does not fall within the expected range.", "address.Category");

            address.CategoryName = ((AddressCategory)address.Category).ToLocalizedString();

            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var contactInfo = new ContactInfo
            {
                InfoType = ContactInfoType.Address,
                ContactID = contactid,
                IsPrimary = address.IsPrimary,
                Category = address.Category,
                Data = JsonSerializer.Serialize(address, settings)
            };

            contactInfo.ID = _daoFactory.GetContactInfoDao().Save(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            _messageService.Send(messageAction, _messageTarget.Create(contact.ID), contact.GetTitle());

            return _mapper.Map<ContactInfoDto>(contactInfo);
        }

        /// <summary>
        ///  Creates contact information (add new information to the old list) with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<short>Group contact info</short> 
        /// <param name="contactid">Contact ID</param>
        /// <param name="items">Contact information</param>
        /// <remarks>
        /// <![CDATA[
        ///  items has format
        ///  [{infoType : 1, category : 1, categoryName : 'work', data : "myemail@email.com", isPrimary : true}, {infoType : 0, category : 0, categoryName : 'home', data : "+8999111999111", isPrimary : true}]
        /// ]]>
        /// </remarks>
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [HttpPost(@"contact/{contactid:int}/batch")]
        public IEnumerable<ContactInfoDto> CreateBatchContactInfo([FromRoute] int contactid, [FromBody] IEnumerable<ContactInfoDto> items)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var itemsList = items != null ? items.ToList() : new List<ContactInfoDto>();
            var contactInfoList = itemsList.Select(FromContactInfoDto).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    Address res;
                    if (!Address.TryParse(contactInfo, out res))
                        throw new ArgumentException();
                }
                contactInfo.ContactID = contactid;
            }

            var ids = _daoFactory.GetContactInfoDao().SaveList(contactInfoList, contact);

            for (var index = 0; index < itemsList.Count; index++)
            {
                var infoDto = itemsList[index];
                infoDto.Id = ids[index];
            }
            return itemsList;
        }

        /// <summary>
        ///   Updates the information with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<param name="id">Contact information record ID</param>
        ///<param name="contactid">Contact ID</param>
        ///<param optional="true" name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param optional="true" name="isPrimary">Contact importance: primary or not</param>
        ///<param optional="true" name="category">Contact information category</param>
        ///<short>Update contact info</short> 
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [HttpPut(@"contact/{contactid:int}/data/{id:int}")]
        public ContactInfoDto UpdateContactInfo(int id, int contactid, ContactInfoType? infoType, string data, bool? isPrimary, string category)
        {
            if (id <= 0 || string.IsNullOrEmpty(data) || contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var contactInfo = _daoFactory.GetContactInfoDao().GetByID(id);

            if (infoType != null)
            {
                var categoryType = ContactInfo.GetCategory(infoType.Value);

                if (!string.IsNullOrEmpty(category) && Enum.IsDefined(categoryType, category))
                {
                    contactInfo.Category = (int)Enum.Parse(categoryType, category);
                }

                contactInfo.InfoType = infoType.Value;
            }

            contactInfo.ContactID = contactid;

            if (isPrimary != null)
            {
                contactInfo.IsPrimary = isPrimary.Value;
            }

            contactInfo.Data = data;

            if (contactInfo.InfoType == ContactInfoType.Address)
            {
                Address res;
                if (!Address.TryParse(contactInfo, out res))
                    throw new ArgumentException();
            }

            _daoFactory.GetContactInfoDao().Update(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;

            _messageService.Send(messageAction, _messageTarget.Create(contact.ID), contact.GetTitle());

            var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);

            return contactInfoDto;
        }

        /// <summary>
        ///   Updates the address information with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        /// <param name="id">Contact information record ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address data</param>
        /// <short>Update address info</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [HttpPut(@"contact/{contactid:int}/addressdata/{id:int}")]
        public ContactInfoDto UpdateContactInfoAddress(int id, int contactid, Address address)
        {
            if (id <= 0) throw new ArgumentException("Invalid value", "id");

            var contactInfo = _daoFactory.GetContactInfoDao().GetByID(id);

            if (contactInfo == null || contactInfo.InfoType != ContactInfoType.Address) throw new ItemNotFoundException();

            if (contactid <= 0) throw new ArgumentException("Invalid value", "contactid");

            var contact = _daoFactory.GetContactDao().GetByID(contactid);

            if (contact == null || !_crmSecurity.CanEdit(contact) || contactInfo.ContactID != contactid) throw new ItemNotFoundException();

            if (address == null) throw new ArgumentException("Value cannot be null", "address");

            if (!Enum.IsDefined(typeof(AddressCategory), address.Category)) throw new ArgumentException("Value does not fall within the expected range.", "address.Category");

            address.CategoryName = ((AddressCategory)address.Category).ToLocalizedString();

            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            contactInfo.IsPrimary = address.IsPrimary;
            contactInfo.Category = address.Category;
            contactInfo.Data = JsonSerializer.Serialize(address, settings);

            _daoFactory.GetContactInfoDao().Update(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;

            _messageService.Send(messageAction, _messageTarget.Create(contact.ID), contact.GetTitle());

            return _mapper.Map<ContactInfoDto>(contactInfo);
        }

        /// <summary>
        ///  Updates contact information (delete old information and add new list) with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<short>Group contact info update</short> 
        ///<param name="contactid">Contact ID</param>
        ///<param name="items">Contact information</param>
        /// <![CDATA[
        ///  items has format
        ///  [{infoType : 1, category : 1, categoryName : 'work', data : "myemail@email.com", isPrimary : true}, {infoType : 0, category : 0, categoryName : 'home', data : "+8999111999111", isPrimary : true}]
        /// ]]>
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [HttpPut(@"contact/{contactid:int}/batch")]
        public IEnumerable<ContactInfoDto> UpdateBatchContactInfo(int contactid, IEnumerable<ContactInfoDto> items)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var itemsList = items != null ? items.ToList() : new List<ContactInfoDto>();
            var contactInfoList = itemsList.Select(FromContactInfoDto).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    Address res;
                    if (!Address.TryParse(contactInfo, out res))
                        throw new ArgumentException();
                }
                contactInfo.ContactID = contactid;
            }

            _daoFactory.GetContactInfoDao().DeleteByContact(contactid);
            var ids = _daoFactory.GetContactInfoDao().SaveList(contactInfoList, contact);

            for (var index = 0; index < itemsList.Count; index++)
            {
                var infoDto = itemsList[index];
                infoDto.Id = ids[index];
            }
            return itemsList;
        }

        /// <summary>
        ///    Returns the detailed information for the contact with the selected ID by the information type specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="infoType">Contact information type</param>
        /// <short>Get contact information by type</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact information
        /// </returns>
        [HttpGet(@"contact/{contactid:int}/data/{infoType}")]
        public IEnumerable<string> GetContactInfo(int contactid, ContactInfoType infoType)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            return _daoFactory.GetContactInfoDao().GetListData(contactid, infoType);
        }


        /// <summary>
        ///   Deletes the contact information for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information record ID</param>
        /// <short>Delete contact info</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [HttpDelete(@"contact/{contactid:int}/data/{id:int}")]
        public ContactInfoDto DeleteContactInfo(int contactid, int id)
        {
            if (id <= 0 || contactid <= 0) throw new ArgumentException();

            var contact = _daoFactory.GetContactDao().GetByID(contactid);
            if (contact == null || !_crmSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var contactInfo = _daoFactory.GetContactInfoDao().GetByID(id);
            if (contactInfo == null) throw new ItemNotFoundException();

            var wrapper = _mapper.Map<ContactInfoDto>(contactInfo);

            _daoFactory.GetContactInfoDao().Delete(id);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;

            _messageService.Send(messageAction, _messageTarget.Create(contact.ID), contact.GetTitle());

            return wrapper;
        }

        private ContactInfo FromContactInfoDto(ContactInfoDto contactInfoDto)
        {
            return new ContactInfo
            {
                ID = contactInfoDto.Id,
                Category = contactInfoDto.Category,
                Data = contactInfoDto.Data,
                InfoType = contactInfoDto.InfoType,
                IsPrimary = contactInfoDto.IsPrimary
            };
        }
    }
}