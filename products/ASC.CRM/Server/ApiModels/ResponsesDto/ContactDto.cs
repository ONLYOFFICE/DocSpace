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

using System.Text.Json;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Mapping;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Mapping;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.ApiModels
{
    public class PersonDto : ContactDto
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public ContactBaseDto Company { get; set; }
        public String Title { get; set; }
        public new static PersonDto GetSample()
        {
            return new PersonDto
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                FirstName = "Tadjeddine",
                LastName = "Bachir",
                Company = CompanyDto.GetSample(),
                Title = "Programmer",
                About = "",
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeDto.GetSample(),
                ShareType = ShareType.None
            };
        }
    }

    [Scope]
    public class PersonDtoHelper
    {
        public PersonDto Get(Person person)
        {
            return new PersonDto
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Title = person.JobTitle
            };
        }

        public PersonDto GetQuick(Person person)
        {
            return new PersonDto
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Title = person.JobTitle
            };
        }
    }

    public class CompanyDto : ContactDto
    {
        public String CompanyName { get; set; }
        public IEnumerable<ContactBaseDto> Persons { get; set; }
        public int PersonsCount { get; set; }
        public new static CompanyDto GetSample()
        {
            return new CompanyDto
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

    public class ContactDto : ContactBaseDto, IMapFrom<ASC.CRM.Core.Entities.Contact>
    {
        public ContactDto()
        {

        }

        public IEnumerable<Address> Addresses { get; set; }
        public EmployeeDto CreateBy { get; set; }
        public ApiDateTime Created { get; set; }
        public String About { get; set; }
        public String Industry { get; set; }
        public ContactStatusBaseDto ContactStatus { get; set; }
        public ContactTypeBaseDto ContactType { get; set; }
        public IEnumerable<ContactInfoDto> CommonData { get; set; }
        public IEnumerable<CustomFieldBaseDto> CustomFields { get; set; }
        public IEnumerable<String> Tags { get; set; }
        public int TaskCount { get; set; }
        public bool HaveLateTasks { get; set; }
        public new static ContactDto GetSample()
        {
            return new PersonDto
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                FirstName = "Tadjeddine",
                LastName = "Bachir",
                Company = CompanyDto.GetSample(),
                Title = "Programmer",
                About = "",
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeDto.GetSample(),
                CommonData = new List<ContactInfoDto>() { ContactInfoDto.GetSample() },
                CustomFields = new List<CustomFieldBaseDto>() { CustomFieldBaseDto.GetSample() },
                ShareType = ShareType.None,
                CanDelete = true,
                CanEdit = true,
                TaskCount = 0,
                HaveLateTasks = false
            };
        }

        public void Mapping(Profile profile)
        {

            profile.CreateMap<Core.Entities.Contact, ContactDto>().ConvertUsing<ContactDtoTypeConverter>();
            profile.CreateMap<List<Core.Entities.Contact>, List<ContactDto>>().ConvertUsing<ContactDtoTypeConverter>();
            profile.CreateMap<Core.Entities.Contact, ContactBaseDto>().ConvertUsing<ContactDtoTypeConverter>();
            profile.CreateMap<Core.Entities.Person, PersonDto>().ConvertUsing<ContactDtoTypeConverter>();
            profile.CreateMap<Core.Entities.Company, CompanyDto>().ConvertUsing<ContactDtoTypeConverter>();

            profile.CreateMap<Core.Entities.Contact, ContactBaseWithPhoneDto>().ConvertUsing<ContactDtoTypeConverter>();
            profile.CreateMap<Core.Entities.Contact, ContactBaseWithEmailDto>().ConvertUsing<ContactDtoTypeConverter>();
        }
    }

    public class ContactBaseWithEmailDto : ContactBaseDto
    {
        public ContactInfoDto Email { get; set; }
    }

    public class ContactBaseWithPhoneDto : ContactBaseDto
    {
        public ContactInfoDto Phone { get; set; }
    }

    public class ContactDtoJsonConverter : JsonConverter<ContactDto>
    {
        public override ContactDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ContactDto value, JsonSerializerOptions options)
        {
            if (value is PersonDto)
            {
                JsonSerializer.Serialize(writer, (PersonDto)value!, options);

                return;
            }

            if (value is CompanyDto)
            {
                JsonSerializer.Serialize(writer, (CompanyDto)value!, options);

                return;
            }

            if (value is ContactDto)
            {
                JsonSerializer.Serialize(writer, value!, options);

                return;
            }

            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///  Contact base information
    /// </summary>
    public class ContactBaseDto
    {
        public ContactBaseDto()
        {

        }

        public int Id { get; set; }
        public String SmallFotoUrl { get; set; }
        public String MediumFotoUrl { get; set; }
        public String DisplayName { get; set; }
        public bool IsCompany { get; set; }
        public IEnumerable<EmployeeDto> AccessList { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsShared { get; set; }
        public ShareType ShareType { get; set; }
        public CurrencyInfoDto Currency { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public static ContactBaseDto GetSample()
        {
            return new ContactBaseDto
            {
                IsPrivate = true,
                IsShared = false,
                IsCompany = false,
                DisplayName = "Tadjeddine Bachir",
                SmallFotoUrl = "url to foto"
            };
        }
    }

    public class ContactWithTaskDto
    {
        public TaskBaseDto Task { get; set; }
        public ContactDto Contact { get; set; }
    }
}