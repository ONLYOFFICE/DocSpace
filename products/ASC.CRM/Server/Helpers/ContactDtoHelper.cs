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


using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Web;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using ASC.Web.CRM;
using ASC.Web.CRM.Classes;

using System;
using System.Collections.Generic;
using System.Linq;

using Contact = ASC.CRM.Core.Entities.Contact;

namespace ASC.CRM.ApiModels
{
    [Scope]
    public class ContactDtoHelper
    {
        public ContactDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           CurrencyProvider currencyProvider,
                           PathProvider pathProvider,
                           CurrencyInfoDtoHelper currencyInfoDtoHelper,
                           DaoFactory daoFactory,
                           ContactInfoDtoHelper contactInfoDtoHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeDtoHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            CurrencyProvider = currencyProvider;
            PathProvider = pathProvider;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
            DaoFactory = daoFactory;
            ContactInfoDtoHelper = contactInfoDtoHelper;
        }

        public ContactInfoDtoHelper ContactInfoDtoHelper;
        public DaoFactory DaoFactory;
        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper;
        public CRMSecurity CRMSecurity;
        public ApiDateTimeHelper ApiDateTimeHelper;
        public EmployeeWraperHelper EmployeeDtoHelper;
        public CurrencyProvider CurrencyProvider;
        public PathProvider PathProvider;

        public ContactBaseDto GetContactBaseDtoQuick(Contact contact)
        {
            ContactBaseDto result;

            if (contact is Company)
                result = new CompanyDto();
            else
                result = new PersonDto();

            result.Id = contact.ID;
            result.DisplayName = contact.GetTitle();
            result.IsPrivate = CRMSecurity.IsPrivate(contact);
            result.IsShared = contact.ShareType == ShareType.ReadWrite || contact.ShareType == ShareType.Read;
            result.ShareType = contact.ShareType;
            result.Currency = !String.IsNullOrEmpty(contact.Currency) ?
                                CurrencyInfoDtoHelper.Get(CurrencyProvider.Get(contact.Currency)) : null;
            result.SmallFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=1", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower();
            result.MediumFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=2", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower();
            result.IsCompany = contact is Company;
            result.CanEdit = CRMSecurity.CanEdit(contact);
            //        CanDelete = CRMSecurity.CanDelete(contact),

            if (result.IsPrivate)
            {
                result.AccessList = CRMSecurity.GetAccessSubjectTo(contact)
                                        .Select(item => EmployeeDtoHelper.Get(item.Key));
            }

            return result;
        }

        public ContactDto ToContactDto(Contact contact)
        {
            return ToListContactDto(new List<Contact>
            {
                contact
            }).Single();
        }

        public IEnumerable<ContactDto> ToListContactDto(IReadOnlyList<Contact> itemList)
        {
            if (itemList.Count == 0) return new List<ContactDto>();

            var result = new List<ContactDto>();

            var personsIDs = new List<int>();
            var companyIDs = new List<int>();
            var contactIDs = new int[itemList.Count];

            var peopleCompanyIDs = new List<int>();
            var peopleCompanyList = new Dictionary<int, ContactBaseDto>();


            var contactDao = DaoFactory.GetContactDao();


            for (var index = 0; index < itemList.Count; index++)
            {
                var contact = itemList[index];

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

                contactIDs[index] = itemList[index].ID;
            }

            if (peopleCompanyIDs.Count > 0)
            {
                var tmpList = contactDao.GetContacts(peopleCompanyIDs.ToArray()).ConvertAll(item => GetContactBaseDtoQuick(item));
                var tmpListCanDelete = contactDao.CanDelete(tmpList.Select(item => item.Id).ToArray());

                foreach (var contactBaseDtoQuick in tmpList)
                {
                    contactBaseDtoQuick.CanDelete = contactBaseDtoQuick.CanEdit && tmpListCanDelete[contactBaseDtoQuick.Id];
                    peopleCompanyList.Add(contactBaseDtoQuick.Id, contactBaseDtoQuick);
                }
            }

            var companiesMembersCount = contactDao.GetMembersCount(companyIDs.Distinct().ToArray());

            var contactStatusIDs = itemList.Select(item => item.StatusID).Distinct().ToArray();
            var contactInfos = new Dictionary<int, List<ContactInfoDto>>();

            var haveLateTask = DaoFactory.GetTaskDao().HaveLateTask(contactIDs);
            var contactStatus = DaoFactory.GetListItemDao()
                                          .GetItems(contactStatusIDs)
                                          .ToDictionary(item => item.ID, item => new ContactStatusBaseDto(item));

            var personsCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, personsIDs.ToArray());
            var companyCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, companyIDs.ToArray());

            var customFields = personsCustomFields.Union(companyCustomFields)
                                                  .GroupBy(item => item.EntityID).ToDictionary(item => item.Key, item => item.Select(x => new CustomFieldBaseDto(x)));

            var addresses = new Dictionary<int, List<Address>>();
            var taskCount = DaoFactory.GetTaskDao().GetTasksCount(contactIDs);

            var contactTags = DaoFactory.GetTagDao().GetEntitiesTags(EntityType.Contact);

            DaoFactory.GetContactInfoDao().GetAll(contactIDs).ForEach(
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
                            contactInfos.Add(item.ContactID, new List<ContactInfoDto> { ContactInfoDtoHelper.Get(item) });
                        else
                            contactInfos[item.ContactID].Add(ContactInfoDtoHelper.Get(item));
                    }
                }
                );


            foreach (var contact in itemList)
            {
                ContactDto contactDto;

                var person = contact as Person;
                if (person != null)
                {
                    var people = person;

                    var peopleDto = GetPersonDtoQuick(people);

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
                        contactDto = GetCompanyDtoQuick(company);

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

        public ContactBaseDto GetContactBaseDto(Contact contact)
        {
            var result = GetContactBaseDtoQuick(contact);

            result.CanDelete = CRMSecurity.CanDelete(contact);

            return result;
        }

        public ContactBaseWithPhoneDto GetContactBaseWithPhoneDto(Contact contact)
        {
            if (contact == null) return null;

            var result = (ContactBaseWithPhoneDto)GetContactBaseDto(contact);

            result.Phone = ContactInfoDtoHelper.Get(DaoFactory.GetContactInfoDao().GetList(contact.ID, ContactInfoType.Phone, null, true).FirstOrDefault());

            return result;
        }

        public ContactBaseWithEmailDto GetContactBaseWithEmailDto(Contact contact)
        {
            if (contact == null) return null;

            var result = (ContactBaseWithEmailDto)GetContactBaseDto(contact);

            result.Email = ContactInfoDtoHelper.Get(DaoFactory.GetContactInfoDao().GetList(contact.ID, ContactInfoType.Email, null, true).FirstOrDefault());

            return result;
        }

        public ContactDto GetContactDto(Contact contact)
        {
            ContactDto result;

            var person = contact as Person;

            if (person != null)
            {
                var peopleDto = (PersonDto)GetContactBaseDto(contact);

                peopleDto.FirstName = person.FirstName;
                peopleDto.LastName = person.LastName;
                peopleDto.Title = person.JobTitle;

                if (person.CompanyID > 0)
                {
                    peopleDto.Company = GetContactBaseDto(DaoFactory.GetContactDao().GetByID(person.CompanyID));
                }

                result = peopleDto;
            }
            else
            {
                var company = contact as Company;

                if (company != null)
                {
                    result = (CompanyDto)GetContactBaseDto(company);
                    ((CompanyDto)result).CompanyName = company.CompanyName;
                    ((CompanyDto)result).PersonsCount = DaoFactory.GetContactDao().GetMembersCount(result.Id);
                }
                else throw new ArgumentException();
            }

            if (contact.StatusID > 0)
            {
                var listItem = DaoFactory.GetListItemDao().GetByID(contact.StatusID);
                if (listItem == null) throw new ItemNotFoundException();

                result.ContactStatus = new ContactStatusBaseDto(listItem);
            }

            result.TaskCount = DaoFactory.GetTaskDao().GetTasksCount(contact.ID);
            result.HaveLateTasks = DaoFactory.GetTaskDao().HaveLateTask(contact.ID);

            var contactInfos = new List<ContactInfoDto>();
            var addresses = new List<Address>();

            var data = DaoFactory.GetContactInfoDao().GetList(contact.ID, null, null, null);

            foreach (var contactInfo in data)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    addresses.Add(new Address(contactInfo));
                }
                else
                {
                    contactInfos.Add(ContactInfoDtoHelper.Get(contactInfo));
                }
            }

            result.Addresses = addresses;
            result.CommonData = contactInfos;

            result.CustomFields = DaoFactory.GetCustomFieldDao()
                                            .GetEnityFields(contact is Person ? EntityType.Person : EntityType.Company, contact.ID, false)
                                            .ConvertAll(item => new CustomFieldBaseDto(item));
            return result;
        }


        public CompanyDto GetCompanyDtoQuick(Company company)
        {
            var result = (CompanyDto)GetContactBaseDtoQuick(company);

            result.CompanyName = company.CompanyName;

            result.CreateBy = EmployeeDtoHelper.Get(company.CreateBy);
            result.Created = ApiDateTimeHelper.Get(company.CreateOn);
            result.About = company.About;
            result.Industry = company.Industry;

            return result;
        }

        public PersonDto GetPersonDtoQuick(Person person)
        {
            var result = (PersonDto)GetContactBaseDtoQuick(person);

            result.CreateBy = EmployeeDtoHelper.Get(person.CreateBy);
            result.Created = ApiDateTimeHelper.Get(person.CreateOn);
            result.About = person.About;
            result.Industry = person.Industry;

            result.FirstName = person.FirstName;
            result.LastName = person.LastName;
            result.Title = person.JobTitle;

            return result;
        }
    }

}