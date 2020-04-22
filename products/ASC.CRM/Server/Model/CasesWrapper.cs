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
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Name = "case", Namespace = "")]
    public class CasesWrapper
    {
        public CasesWrapper()
        {
        }


        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Members { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }
        public static CasesWrapper GetSample()
        {
            return new CasesWrapper
            {
                IsClosed = false,
                Title = "Exhibition organization",
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                IsPrivate = false,
                CustomFields = new[] { CustomFieldBaseWrapper.GetSample() },
                CanEdit = true
            };
        }
    }

    public class CasesWrapperHelper
    {
        public CasesWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           DaoFactory daoFactory,
                           ContactWrapperHelper contactBaseWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseWrapperHelper = contactBaseWrapperHelper;
        }

        public ContactWrapperHelper ContactBaseWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }

        public CasesWrapper Get(Cases cases)
        {
            var result = new CasesWrapper
            {
                Title = cases.Title,
                IsClosed = cases.IsClosed,
                IsPrivate = CRMSecurity.IsPrivate(cases),
                Created = ApiDateTimeHelper.Get(cases.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(cases.CreateBy),
                CanEdit = CRMSecurity.CanEdit(cases)
            };
          
            if (result.IsPrivate)
            {
                result.AccessList = CRMSecurity.GetAccessSubjectTo(cases)
                                        .SkipWhile(item => item.Key == Constants.GroupEveryone.ID)
                                        .Select(item => EmployeeWraperHelper.Get(item.Key));
            }

            result.CustomFields = DaoFactory
            .GetCustomFieldDao()
            .GetEnityFields(EntityType.Case, cases.ID, false)
            .ConvertAll(item => new CustomFieldBaseWrapper(item));

            result.Members = new List<ContactBaseWrapper>();

            var memberIDs = DaoFactory.GetCasesDao().GetMembers(cases.ID);
            var membersList = DaoFactory.GetContactDao().GetContacts(memberIDs);

            var membersWrapperList = new List<ContactBaseWrapper>();

            foreach (var member in membersList)
            {
                if (member == null) continue;

                membersWrapperList.Add(ContactBaseWrapperHelper.GetContactBaseWrapper(member));
            }

            result.Members = membersWrapperList;
         
            return result;
        }
    }

    public static class CasesWrapperHelperExtension
    {
        public static DIHelper AddCasesWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<CasesWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService();
        }
    }
}


