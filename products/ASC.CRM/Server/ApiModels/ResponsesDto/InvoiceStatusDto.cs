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



using ASC.Common.Mapping;
using ASC.CRM.Classes;
using ASC.CRM.Core.Enums;

using AutoMapper;

namespace ASC.CRM.ApiModels
{
    /// <summary>
    ///  Invoice Status
    /// </summary>
    public class InvoiceStatusDto : IMapFrom<InvoiceStatus>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public static InvoiceStatusDto GetSample()
        {
            return new InvoiceStatusDto
            {
                Id = (int)InvoiceStatus.Draft,
                Title = InvoiceStatus.Draft.ToLocalizedString()
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceStatus, InvoiceStatusDto>()
                    .ForMember(x => x.Title, x => x.MapFrom(y => y.ToLocalizedString()));

        }
    }
}
