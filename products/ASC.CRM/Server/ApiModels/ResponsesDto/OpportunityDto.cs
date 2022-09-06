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


using ASC.Api.Core;
using ASC.Common.Mapping;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Mapping;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.ApiModels
{
    /// <summary>
    ///  Opportunity
    /// </summary>
    public class OpportunityDto : IMapFrom<Deal>
    {

        public OpportunityDto()
        {
        }

        public int Id { get; set; }
        public EmployeeDto CreateBy { get; set; }
        public ApiDateTime Created { get; set; }
        public IEnumerable<ContactBaseDto> Members { get; set; }
        public ContactBaseDto Contact { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public EmployeeDto Responsible { get; set; }
        public BidType BidType { get; set; }
        public decimal BidValue { get; set; }
        public CurrencyInfoDto BidCurrency { get; set; }
        public int PerPeriodValue { get; set; }
        public DealMilestoneBaseDto Stage { get; set; }
        public int SuccessProbability { get; set; }
        public ApiDateTime ActualCloseDate { get; set; }
        public ApiDateTime ExpectedCloseDate { get; set; }

        public bool IsPrivate { get; set; }
        public IEnumerable<EmployeeDto> AccessList { get; set; }
        public bool CanEdit { get; set; }
        public IEnumerable<CustomFieldBaseDto> CustomFields { get; set; }

        public static OpportunityDto GetSample()
        {
            return new OpportunityDto
            {
                CreateBy = EmployeeDto.GetSample(),
                Created = ApiDateTime.GetSample(),
                Responsible = EmployeeDto.GetSample(),
                Title = "Hotel catalogue",
                Description = "",
                ExpectedCloseDate = ApiDateTime.GetSample(),
                Contact = ContactBaseDto.GetSample(),
                IsPrivate = false,
                SuccessProbability = 65,
                BidType = BidType.FixedBid,
                Stage = DealMilestoneBaseDto.GetSample()
            };
        }


        public void Mapping(Profile profile)
        {
            profile.CreateMap<Deal, OpportunityDto>().ConvertUsing<OpportunityDtoTypeConverter>();
        }
    }
}