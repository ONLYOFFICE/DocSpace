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
using ASC.Core.Users;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class CasesDtoTypeConverter : ITypeConverter<Cases, CasesDto>
    {
        private readonly DaoFactory _daoFactory;
        private readonly CrmSecurity _CRMSecurity;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;

        public CasesDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeDtoHelper employeeDtoHelper,
                           CrmSecurity crmSecurity,
                           DaoFactory daoFactory)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _CRMSecurity = crmSecurity;
            _daoFactory = daoFactory;
        }

        public CasesDto Convert(Cases source, CasesDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new CasesDto();

            result.Title = source.Title;
            result.IsClosed = source.IsClosed;
            result.IsPrivate = _CRMSecurity.IsPrivate(source);
            result.Created = _apiDateTimeHelper.Get(source.CreateOn);
            result.CreateBy = _employeeDtoHelper.Get(source.CreateBy);
            result.CanEdit = _CRMSecurity.CanEdit(source);

            if (result.IsPrivate)
            {
                result.AccessList = _CRMSecurity.GetAccessSubjectTo(source)
                                        .SkipWhile(item => item.Key == Constants.GroupEveryone.ID)
                                        .Select(item => _employeeDtoHelper.Get(item.Key));
            }

            result.CustomFields = _daoFactory
            .GetCustomFieldDao()
            .GetEnityFields(EntityType.Case, source.ID, false)
            .ConvertAll(item => context.Mapper.Map<CustomFieldBaseDto>(item));

            result.Members = new List<ContactBaseDto>();

            var memberIDs = _daoFactory.GetCasesDao().GetMembers(source.ID);
            var membersList = _daoFactory.GetContactDao().GetContacts(memberIDs);

            var membersDtoList = new List<ContactBaseDto>();

            foreach (var member in membersList)
            {
                if (member == null) continue;

                membersDtoList.Add(context.Mapper.Map<ContactBaseDto>(member));
            }

            result.Members = membersDtoList;

            return destination;
        }
    }
}


