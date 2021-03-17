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
using ASC.VoipService;
using ASC.Web.Api.Models;

using System.Linq;

namespace ASC.CRM.ApiModels
{
    [Scope]
    public class VoipCallDtoHelper
    {
        private ApiDateTimeHelper _apiDateTimeHelper;
        private EmployeeWraperHelper _employeeWraperHelper;

        public VoipCallDtoHelper(EmployeeWraperHelper employeeWraper,
                                     ApiDateTimeHelper apiDateTimeHelper)
        {
            _employeeWraperHelper = employeeWraper;
            _apiDateTimeHelper = apiDateTimeHelper;
        }

        public VoipCallDto Get(VoipCall call, ContactDto contact = null)
        {
            var result = new VoipCallDto
            {
                Id = call.Id,
                From = call.From,
                To = call.To,
                Status = call.Status,
                AnsweredBy = _employeeWraperHelper.Get(call.AnsweredBy),
                DialDate = _apiDateTimeHelper.Get(call.DialDate),
                DialDuration = call.DialDuration,
                Cost = call.Price + call.ChildCalls.Sum(r => r.Price) + call.VoipRecord.Price,
                Contact = contact,
                RecordUrl = call.VoipRecord.Uri,
                RecordDuration = call.VoipRecord.Duration
            };

            if (call.ChildCalls.Any())
            {
                result.Calls = call.ChildCalls.Select(childCall => Get(childCall));
            }

            return result;
        }
    }
}