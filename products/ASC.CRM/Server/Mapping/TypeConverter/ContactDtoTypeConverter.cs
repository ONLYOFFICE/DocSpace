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
using ASC.Common;
using ASC.Common.Web;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using ASC.Web.CRM;
using ASC.Web.CRM.Classes;

using AutoMapper;

using Contact = ASC.CRM.Core.Entities.Contact;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class ContactDtoTypeConverter : ITypeConverter<Contact, ContactDto>,
                                           ITypeConverter<List<Contact>, List<ContactDto>>,
                                           ITypeConverter<Contact, ContactBaseDto>,
                                           ITypeConverter<Person, PersonDto>,
                                           ITypeConverter<Company, CompanyDto>,
                                           ITypeConverter<Contact, ContactBaseWithPhoneDto>,
                                           ITypeConverter<Contact, ContactBaseWithEmailDto>


    {
        private readonly DaoFactory _daoFactory;
        private readonly CrmSecurity _crmSecurity;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;
        private readonly CurrencyProvider _currencyProvider;
        private readonly PathProvider _pathProvider;

        public ContactDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeDtoHelper employeeDtoHelper,
                           CrmSecurity crmSecurity,
                           CurrencyProvider currencyProvider,
                           PathProvider pathProvider,
                           DaoFactory daoFactory)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _crmSecurity = crmSecurity;
            _currencyProvider = currencyProvider;
            _pathProvider = pathProvider;
            _daoFactory = daoFactory;
        }



        public ContactDto Convert(Contact source, ContactDto destination, ResolutionContext context)
        {
            ContactDto result;

            var person = source as Person;

            if (person != null)
            {
                var peopleDto = (PersonDto)context.Mapper.Map<ContactBaseDto>(source);

                peopleDto.FirstName = person.FirstName;
                peopleDto.LastName = person.LastName;
                peopleDto.Title = person.JobTitle;

                if (person.CompanyID > 0)
                {
                    peopleDto.Company = context.Mapper.Map<ContactBaseDto>(_daoFactory.GetContactDao().GetByID(person.CompanyID));
                }

                result = peopleDto;
            }
            else
            {
                var company = source as Company;

                if (company != null)
                {
                    result = (CompanyDto)context.Mapper.Map<ContactBaseDto>(source);
                    ((CompanyDto)result).CompanyName = company.CompanyName;
                    ((CompanyDto)result).PersonsCount = _daoFactory.GetContactDao().GetMembersCount(result.Id);
                }
                else throw new ArgumentException();
            }

            if (source.StatusID > 0)
            {
                var listItem = _daoFactory.GetListItemDao().GetByID(source.StatusID);
                if (listItem == null) throw new ItemNotFoundException();

                result.ContactStatus = new ContactStatusBaseDto(listItem);
            }

            result.TaskCount = _daoFactory.GetTaskDao().GetTasksCount(source.ID);
            result.HaveLateTasks = _daoFactory.GetTaskDao().HaveLateTask(source.ID);

            var contactInfos = new List<ContactInfoDto>();
            var addresses = new List<Address>();

            var data = _daoFactory.GetContactInfoDao().GetList(source.ID, null, null, null);

            foreach (var contactInfo in data)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    addresses.Add(new Address(contactInfo));
                }
                else
                {
                    contactInfos.Add(context.Mapper.Map<ContactInfoDto>(contactInfo));
                }
            }

            result.Addresses = addresses;
            result.CommonData = contactInfos;

            result.CustomFields = _daoFactory.GetCustomFieldDao()
                                            .GetEnityFields(source is Person ? EntityType.Person : EntityType.Company, source.ID, false)
                                            .ConvertAll(item => context.Mapper.Map<CustomFieldBaseDto>(item));
            return result;
        }

        public ContactBaseDto Convert(Contact source, ContactBaseDto destination, ResolutionContext context)
        {
            ContactBaseDto result;

            if (source is Company)
                result = new CompanyDto();
            else
                result = new PersonDto();

            result.Id = source.ID;
            result.DisplayName = source.GetTitle();
            result.IsPrivate = _crmSecurity.IsPrivate(source);
            result.IsShared = source.ShareType == ShareType.ReadWrite || source.ShareType == ShareType.Read;
            result.ShareType = source.ShareType;

            result.Currency = !String.IsNullOrEmpty(source.Currency) ?
                                context.Mapper.Map<CurrencyInfoDto>(_currencyProvider.Get(source.Currency)) : null;


            result.SmallFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=1", _pathProvider.BaseAbsolutePath, source.ID, source is Company).ToLower();
            result.MediumFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=2", _pathProvider.BaseAbsolutePath, source.ID, source is Company).ToLower();
            result.IsCompany = source is Company;
            result.CanEdit = _crmSecurity.CanEdit(source);
            result.CanDelete = _crmSecurity.CanDelete(source);

            if (result.IsPrivate)
            {
                result.AccessList = _crmSecurity.GetAccessSubjectTo(source)
                                        .Select(item => _employeeDtoHelper.Get(item.Key));
            }

            return result;
        }

        public PersonDto Convert(Person source, PersonDto destination, ResolutionContext context)
        {
            var result = (PersonDto)context.Mapper.Map<ContactBaseDto>(source);

            result.CreateBy = _employeeDtoHelper.Get(source.CreateBy);
            result.Created = _apiDateTimeHelper.Get(source.CreateOn);
            result.About = source.About;
            result.Industry = source.Industry;
            result.FirstName = source.FirstName;
            result.LastName = source.LastName;
            result.Title = source.JobTitle;

            return result;
        }

        public CompanyDto Convert(Company source, CompanyDto destination, ResolutionContext context)
        {
            var result = (CompanyDto)context.Mapper.Map<ContactBaseDto>(source);

            result.CompanyName = source.CompanyName;

            result.CreateBy = _employeeDtoHelper.Get(source.CreateBy);
            result.Created = _apiDateTimeHelper.Get(source.CreateOn);
            result.About = source.About;
            result.Industry = source.Industry;

            return result;
        }

        public ContactBaseWithPhoneDto Convert(Contact source, ContactBaseWithPhoneDto destination, ResolutionContext context)
        {
            if (source == null) return null;

            var contactBaseDto = context.Mapper.Map<ContactBaseDto>(source);

            var result = new ContactBaseWithPhoneDto
            {
                AccessList = contactBaseDto.AccessList,
                CanDelete = contactBaseDto.CanDelete,
                CanEdit = contactBaseDto.CanEdit,
                Currency = contactBaseDto.Currency,
                DisplayName = contactBaseDto.DisplayName,
                Id = contactBaseDto.Id,
                IsCompany = contactBaseDto.IsCompany,
                IsPrivate = contactBaseDto.IsPrivate,
                IsShared = contactBaseDto.IsShared,
                MediumFotoUrl = contactBaseDto.MediumFotoUrl,
                ShareType = contactBaseDto.ShareType,
                SmallFotoUrl = contactBaseDto.SmallFotoUrl
            };

            var phone = _daoFactory.GetContactInfoDao().GetList(source.ID, ContactInfoType.Phone, null, true).FirstOrDefault();

            result.Phone = context.Mapper.Map<ContactInfoDto>(phone);

            return result;
        }

        public ContactBaseWithEmailDto Convert(Contact source, ContactBaseWithEmailDto destination, ResolutionContext context)
        {
            if (source == null) return null;

            var contactBaseDto = context.Mapper.Map<ContactBaseDto>(source);

            var result = new ContactBaseWithEmailDto
            {
                AccessList = contactBaseDto.AccessList,
                CanDelete = contactBaseDto.CanDelete,
                CanEdit = contactBaseDto.CanEdit,
                Currency = contactBaseDto.Currency,
                DisplayName = contactBaseDto.DisplayName,
                Id = contactBaseDto.Id,
                IsCompany = contactBaseDto.IsCompany,
                IsPrivate = contactBaseDto.IsPrivate,
                IsShared = contactBaseDto.IsShared,
                MediumFotoUrl = contactBaseDto.MediumFotoUrl,
                ShareType = contactBaseDto.ShareType,
                SmallFotoUrl = contactBaseDto.SmallFotoUrl
            };

            var email = _daoFactory.GetContactInfoDao().GetList(source.ID, ContactInfoType.Email, null, true).FirstOrDefault();

            result.Email = context.Mapper.Map<ContactInfoDto>(email);

            return result;
        }

        public List<ContactDto> Convert(List<Contact> source, List<ContactDto> destination, ResolutionContext context)
        {
            if (source.Count == 0) return new List<ContactDto>();

            var result = new List<ContactDto>();

            var personsIDs = new List<int>();
            var companyIDs = new List<int>();
            var contactIDs = new int[source.Count];

            var peopleCompanyIDs = new List<int>();
            var peopleCompanyList = new Dictionary<int, ContactBaseDto>();


            var contactDao = _daoFactory.GetContactDao();


            for (var index = 0; index < source.Count; index++)
            {
                var contact = source[index];

                if (contact is Company)
                {
                    companyIDs.Add(contact.ID);
                }
                else
                {
                    var person = contact as Person;
                    if (person != null)
                    {
                        personsIDs.Add(person.ID);

                        if (person.CompanyID > 0)
                        {
                            peopleCompanyIDs.Add(person.CompanyID);
                        }
                    }
                }

                contactIDs[index] = source[index].ID;
            }

            if (peopleCompanyIDs.Count > 0)
            {
                var tmpList = contactDao.GetContacts(peopleCompanyIDs.ToArray()).ConvertAll(item => context.Mapper.Map<ContactBaseDto>(item));
                var tmpListCanDelete = contactDao.CanDelete(tmpList.Select(item => item.Id).ToArray());

                foreach (var contactBaseDtoQuick in tmpList)
                {
                    contactBaseDtoQuick.CanDelete = contactBaseDtoQuick.CanEdit && tmpListCanDelete[contactBaseDtoQuick.Id];
                    peopleCompanyList.Add(contactBaseDtoQuick.Id, contactBaseDtoQuick);
                }
            }

            var companiesMembersCount = contactDao.GetMembersCount(companyIDs.Distinct().ToArray());

            var contactStatusIDs = source.Select(item => item.StatusID).Distinct().ToArray();
            var contactInfos = new Dictionary<int, List<ContactInfoDto>>();

            var haveLateTask = _daoFactory.GetTaskDao().HaveLateTask(contactIDs);
            var contactStatus = _daoFactory.GetListItemDao()
                                          .GetItems(contactStatusIDs)
                                          .ToDictionary(item => item.ID, item => new ContactStatusBaseDto(item));

            var personsCustomFields = _daoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, personsIDs.ToArray());
            var companyCustomFields = _daoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, companyIDs.ToArray());

            var customFields = personsCustomFields.Union(companyCustomFields)
                                                  .GroupBy(item => item.EntityID).ToDictionary(item => item.Key, item => item.Select(x => context.Mapper.Map<CustomFieldBaseDto>(x)));

            var addresses = new Dictionary<int, List<Address>>();
            var taskCount = _daoFactory.GetTaskDao().GetTasksCount(contactIDs);

            var contactTags = _daoFactory.GetTagDao().GetEntitiesTags(EntityType.Contact);

            _daoFactory.GetContactInfoDao().GetAll(contactIDs).ForEach(
                item =>
                {
                    if (item.InfoType == ContactInfoType.Address)
                    {
                        if (!addresses.ContainsKey(item.ContactID))
                            addresses.Add(item.ContactID, new List<Address> { new Address(item) });
                        else
                            addresses[item.ContactID].Add(new Address(item));
                    }
                    else
                    {
                        if (!contactInfos.ContainsKey(item.ContactID))
                            contactInfos.Add(item.ContactID, new List<ContactInfoDto> { context.Mapper.Map<ContactInfoDto>(item) });
                        else
                            contactInfos[item.ContactID].Add(context.Mapper.Map<ContactInfoDto>(item));
                    }
                }
                );


            foreach (var contact in source)
            {
                ContactDto contactDto;

                var person = contact as Person;
                if (person != null)
                {
                    var people = person;

                    var peopleDto = context.Mapper.Map<PersonDto>(people);

                    if (people.CompanyID > 0 && peopleCompanyList.ContainsKey(people.CompanyID))
                    {
                        peopleDto.Company = peopleCompanyList[people.CompanyID];
                    }

                    contactDto = peopleDto;
                }
                else
                {
                    var company = contact as Company;
                    if (company != null)
                    {
                        contactDto = context.Mapper.Map<CompanyDto>(company);

                        if (companiesMembersCount.ContainsKey(contactDto.Id))
                        {
                            ((CompanyDto)contactDto).PersonsCount = companiesMembersCount[contactDto.Id];
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }

                if (contactTags.ContainsKey(contact.ID))
                {
                    contactDto.Tags = contactTags[contact.ID].OrderBy(x => x);
                }

                if (addresses.ContainsKey(contact.ID))
                {
                    contactDto.Addresses = addresses[contact.ID];
                }

                contactDto.CommonData = contactInfos.ContainsKey(contact.ID) ? contactInfos[contact.ID] : new List<ContactInfoDto>();

                if (contactStatus.ContainsKey(contact.StatusID))
                {
                    contactDto.ContactStatus = contactStatus[contact.StatusID];
                }

                contactDto.HaveLateTasks = haveLateTask.ContainsKey(contact.ID) && haveLateTask[contact.ID];

                contactDto.CustomFields = customFields.ContainsKey(contact.ID) ? customFields[contact.ID] : new List<CustomFieldBaseDto>();

                contactDto.TaskCount = taskCount.ContainsKey(contact.ID) ? taskCount[contact.ID] : 0;

                result.Add(contactDto);
            }

            #region CanDelete for main contacts

            if (result.Count > 0)
            {
                var resultListCanDelete = contactDao.CanDelete(result.Select(item => item.Id).ToArray());
                foreach (var contactBaseDtoQuick in result)
                {
                    contactBaseDtoQuick.CanDelete = contactBaseDtoQuick.CanEdit && resultListCanDelete[contactBaseDtoQuick.Id];
                }
            }

            #endregion

            return result;
        }
    }

}