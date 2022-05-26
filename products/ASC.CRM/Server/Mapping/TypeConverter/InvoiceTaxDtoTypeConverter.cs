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

using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class InvoiceTaxDtoTypeConverter : ITypeConverter<InvoiceTax, InvoiceTaxDto>
    {
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;
        private readonly CrmSecurity _crmSecurity;

        public InvoiceTaxDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                                      EmployeeDtoHelper employeeDtoHelper,
                                      CrmSecurity crmSecurity)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _crmSecurity = crmSecurity;
        }

        public InvoiceTaxDto Convert(InvoiceTax source, InvoiceTaxDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new InvoiceTaxDto();

            result.Id = source.ID;
            result.Name = source.Name;
            result.Description = source.Description;
            result.Rate = source.Rate;
            result.CreateOn = _apiDateTimeHelper.Get(source.CreateOn);
            result.CreateBy = _employeeDtoHelper.Get(source.CreateBy);
            result.CanEdit = _crmSecurity.CanEdit(source);
            result.CanDelete = _crmSecurity.CanDelete(source);

            return result;
        }
    }
}
