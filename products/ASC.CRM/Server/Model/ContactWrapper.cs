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
using ASC.CRM.Classes;
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
using System.Runtime.Serialization;
using Contact = ASC.CRM.Core.Entities.Contact;

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///   Person
    /// </summary>
    [DataContract(Name = "person", Namespace = "")]
    public class PersonWrapper : ContactWrapper
    {
        public PersonWrapper()
        {

        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String FirstName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String LastName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Company { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        public new static PersonWrapper GetSample()
        {
            return new PersonWrapper
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                FirstName = "Tadjeddine",
                LastName = "Bachir",
                Company = CompanyWrapper.GetSample(),
                Title = "Programmer",
                About = "",
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                ShareType = ShareType.None
            };
        }
    }

    public class PersonWrapperHelper
    {
        public PersonWrapperHelper()
        {
        }

        public PersonWrapper Get(Person person)
        {
            return new PersonWrapper
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Title = person.JobTitle
            };
        }

        public PersonWrapper GetQuick(Person person)
        {
            return new PersonWrapper
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Title = person.JobTitle
            };
        }


    }

    public static class PersonWrapperHelperHelperExtension
    {
        public static DIHelper AddPersonWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<PersonWrapperHelper>();

            return services;
        }
    }

    /// <summary>
    ///  Company
    /// </summary>
    [DataContract(Name = "company", Namespace = "")]
    public class CompanyWrapper : ContactWrapper
    {
        public CompanyWrapper()
        {
        }

        //public CompanyWrapper(Company company)
        //    : base(company)
        //{
        //    CompanyName = company.CompanyName;
        //    //  PersonsCount = Global.DaoFactory.ContactDao.GetMembersCount(company.ID);
        //}




        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String CompanyName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Persons { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int PersonsCount { get; set; }

        public new static CompanyWrapper GetSample()
        {
            return new CompanyWrapper
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = true,
                About = "",
                CompanyName = "Food and Culture Project",
                PersonsCount = 0
            };
        }
    }

    [DataContract(Name = "contact", Namespace = "")]
    [KnownType(typeof(PersonWrapper))]
    [KnownType(typeof(CompanyWrapper))]
    public abstract class ContactWrapper : ContactBaseWrapper
    {
        public ContactWrapper()
        {

        }

        //protected ContactWrapper(Contact contact)
        //    : base(contact)
        //{
        //    CreateBy = EmployeeWraper.Get(contact.CreateBy);
        //    Created = (ApiDateTime)contact.CreateOn;
        //    About = contact.About;
        //    Industry = contact.Industry;
        //}

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<Address> Addresses { get; set; }


        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String About { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Industry { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactStatusBaseWrapper ContactStatus { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactTypeBaseWrapper ContactType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactInfoWrapper> CommonData { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<String> Tags { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int TaskCount { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool HaveLateTasks { get; set; }

        public new static ContactWrapper GetSample()
        {
            return new PersonWrapper
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                FirstName = "Tadjeddine",
                LastName = "Bachir",
                Company = CompanyWrapper.GetSample(),
                Title = "Programmer",
                About = "",
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                CommonData = new List<ContactInfoWrapper>() { ContactInfoWrapper.GetSample() },
                CustomFields = new List<CustomFieldBaseWrapper>() { CustomFieldBaseWrapper.GetSample() },
                ShareType = ShareType.None,
                CanDelete = true,
                CanEdit = true,
                TaskCount = 0,
                HaveLateTasks = false
            };
        }
    }

    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWithEmailWrapper : ContactBaseWrapper
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactInfoWrapper Email { get; set; }
    }


    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWithPhoneWrapper : ContactBaseWrapper
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactInfoWrapper Phone { get; set; }
    }







































    /// <summary>
    ///  Contact base information
    /// </summary>
    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWrapper
    {
        public ContactBaseWrapper()
        {

        }


        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String SmallFotoUrl { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String MediumFotoUrl { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public String DisplayName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsCompany { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsShared { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShareType ShareType { get; set; }

        [DataMember]
        public CurrencyInfoWrapper Currency { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanDelete { get; set; }

        public static ContactBaseWrapper GetSample()
        {
            return new ContactBaseWrapper
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                DisplayName = "Tadjeddine Bachir",
                SmallFotoUrl = "url to foto"
            };
        }
    }

    [DataContract(Name = "contact_task", Namespace = "")]
    public class ContactWithTaskWrapper
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskBaseWrapper Task { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactWrapper Contact { get; set; }
    }

    public class ContactWrapperHelper
    {
        public ContactWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           CurrencyProvider currencyProvider,
                           PathProvider pathProvider,
                           CurrencyInfoWrapperHelper currencyInfoWrapperHelper,
                           DaoFactory daoFactory,
                           ContactInfoWrapperHelper contactInfoWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWrapperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            CurrencyProvider = currencyProvider;
            PathProvider = pathProvider;
            CurrencyInfoWrapperHelper = currencyInfoWrapperHelper;
            DaoFactory = daoFactory;
            ContactInfoWrapperHelper = contactInfoWrapperHelper;
        }

        public ContactInfoWrapperHelper ContactInfoWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public CurrencyInfoWrapperHelper CurrencyInfoWrapperHelper { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWrapperHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public PathProvider PathProvider { get; }

        public ContactBaseWrapper GetContactBaseWrapperQuick(Contact contact)
        {
            var result = new ContactBaseWrapper
            {
                Id = contact.ID,
                DisplayName = contact.GetTitle(),
                IsPrivate = CRMSecurity.IsPrivate(contact),
                IsShared = contact.ShareType == ShareType.ReadWrite || contact.ShareType == ShareType.Read,
                ShareType = contact.ShareType,
                Currency = !String.IsNullOrEmpty(contact.Currency) ?
                CurrencyInfoWrapperHelper.Get(CurrencyProvider.Get(contact.Currency)) : null,
                SmallFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=1", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower(),
                MediumFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=2", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower(),
                IsCompany = contact is Company,
                CanEdit = CRMSecurity.CanEdit(contact),
                //        CanDelete = CRMSecurity.CanDelete(contact),
            };

            if (result.IsPrivate)
            {
                result.AccessList = CRMSecurity.GetAccessSubjectTo(contact)
                                        .Select(item => EmployeeWrapperHelper.Get(item.Key));
            }

            return result;
        }


        public ContactBaseWrapper GetContactBaseWrapper(Contact contact)
        {
            var result = GetContactBaseWrapperQuick(contact);

            result.CanDelete = CRMSecurity.CanDelete(contact);

            return result;
        }

        public ContactBaseWithPhoneWrapper GetContactBaseWithPhoneWrapper(Contact contact)
        {
            if (contact == null) return null;

            var result = (ContactBaseWithPhoneWrapper)GetContactBaseWrapper(contact);

            result.Phone = ContactInfoWrapperHelper.Get(DaoFactory.GetContactInfoDao().GetList(contact.ID, ContactInfoType.Phone, null, true).FirstOrDefault());

            return result;
        }

        public ContactBaseWithEmailWrapper GetContactBaseWithEmailWrapper(Contact contact)
        {
            if (contact == null) return null;

            var result = (ContactBaseWithEmailWrapper)GetContactBaseWrapper(contact);

            result.Email = ContactInfoWrapperHelper.Get(DaoFactory.GetContactInfoDao().GetList(contact.ID, ContactInfoType.Email, null, true).FirstOrDefault());

            return result;
        }

        public ContactWrapper GetContactWrapper(Contact contact)
        {
            ContactWrapper result;

            var person = contact as Person;
            if (person != null)
            {
                var peopleWrapper = (PersonWrapper)GetContactBaseWrapper(contact);

                peopleWrapper.FirstName = person.FirstName;
                peopleWrapper.LastName = person.LastName;
                peopleWrapper.Title = person.JobTitle;

                if (person.CompanyID > 0)
                {
                    peopleWrapper.Company = GetContactBaseWrapper(DaoFactory.GetContactDao().GetByID(person.CompanyID));
                }

                result = peopleWrapper;
            }
            else
            {
                var company = contact as Company;

                if (company != null)
                {
                    result = (CompanyWrapper)GetContactBaseWrapper(company);
                    ((CompanyWrapper)result).CompanyName = company.CompanyName;
                    ((CompanyWrapper)result).PersonsCount = DaoFactory.GetContactDao().GetMembersCount(result.Id);
                }
                else throw new ArgumentException();
            }

            if (contact.StatusID > 0)
            {
                var listItem = DaoFactory.GetListItemDao().GetByID(contact.StatusID);
                if (listItem == null) throw new ItemNotFoundException();

                result.ContactStatus = new ContactStatusBaseWrapper(listItem);
            }

            result.TaskCount = DaoFactory.GetTaskDao().GetTasksCount(contact.ID);
            result.HaveLateTasks = DaoFactory.GetTaskDao().HaveLateTask(contact.ID);

            var contactInfos = new List<ContactInfoWrapper>();
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
                    contactInfos.Add(ContactInfoWrapperHelper.Get(contactInfo));
                }
            }

            result.Addresses = addresses;
            result.CommonData = contactInfos;

            result.CustomFields = DaoFactory.GetCustomFieldDao()
                                            .GetEnityFields(contact is Person ? EntityType.Person : EntityType.Company, contact.ID, false)
                                            .ConvertAll(item => new CustomFieldBaseWrapper(item));
            return result;
        }


        public CompanyWrapper GetCompanyWrapperQuick(Company company)
        {
            var result = (CompanyWrapper)GetContactBaseWrapperQuick(company);

            result.CompanyName = company.CompanyName;

            result.CreateBy = EmployeeWrapperHelper.Get(company.CreateBy);
            result.Created = ApiDateTimeHelper.Get(company.CreateOn);
            result.About = company.About;
            result.Industry = company.Industry;

            return result;
        }

        public PersonWrapper GetPersonWrapperQuick(Person person)
        {
            var result = (PersonWrapper)GetContactBaseWrapperQuick(person);

            result.CreateBy = EmployeeWrapperHelper.Get(person.CreateBy);
            result.Created = ApiDateTimeHelper.Get(person.CreateOn);
            result.About = person.About;
            result.Industry = person.Industry;

            result.FirstName = person.FirstName;
            result.LastName = person.LastName;
            result.Title = person.JobTitle;

            return result;
        }
    }

    public static class ContactWrapperHelperExtension
    {
        public static DIHelper AddContactWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<ContactWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService()
                           .AddCurrencyProviderService()
                           .AddCRMPathProviderService();
        }
    }

}