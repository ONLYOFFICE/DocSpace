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
using ASC.Web.CRM.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.CRM.ApiModels
{
    /// <summary>
    ///  Opportunity
    /// </summary>
    [DataContract(Name = "opportunity", Namespace = "")]
    public class OpportunityDto
    {
   
        public OpportunityDto()
        {
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }        
        public EmployeeWraper CreateBy { get; set; }        
        public ApiDateTime Created { get; set; }        
        public IEnumerable<ContactBaseDto> Members { get; set; }        
        public ContactBaseDto Contact { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }        
        public String Description { get; set; }        
        public EmployeeWraper Responsible { get; set; }        
        public BidType BidType { get; set; }        
        public decimal BidValue { get; set; }        
        public CurrencyInfoDto BidCurrency { get; set; }        
        public int PerPeriodValue { get; set; }        
        public DealMilestoneBaseDto Stage { get; set; }        
        public int SuccessProbability { get; set; }        
        public ApiDateTime ActualCloseDate { get; set; }        
        public ApiDateTime ExpectedCloseDate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }        
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }        
        public IEnumerable<CustomFieldBaseDto> CustomFields { get; set; }

        public static OpportunityDto GetSample()
        {
            return new OpportunityDto
            {
                CreateBy = EmployeeWraper.GetSample(),
                Created = ApiDateTime.GetSample(),
                Responsible = EmployeeWraper.GetSample(),
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
    }


    [Scope]
    public class OpportunityDtoHelper
    {
        public OpportunityDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           DaoFactory daoFactory,
                           CurrencyProvider currencyProvider,
                           ContactDtoHelper contactBaseDtoHelper,
                           CurrencyInfoDtoHelper currencyInfoDtoHelper
                           )
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseDtoHelper = contactBaseDtoHelper;
            CurrencyProvider = currencyProvider;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
        }

        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper { get; }
        public CurrencyProvider CurrencyProvider  {get;}
        public ContactDtoHelper ContactBaseDtoHelper { get; }
        public DaoFactory DaoFactory { get; }

        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }

        public OpportunityDto Get(Deal deal)
        {
            var dealDto = new OpportunityDto
            {
                Id = deal.ID,
                CreateBy = EmployeeWraperHelper.Get(deal.CreateBy),
                Created = ApiDateTimeHelper.Get(deal.CreateOn),
                Title = deal.Title,
                Description = deal.Description,
                Responsible = EmployeeWraperHelper.Get(deal.ResponsibleID),
                BidType = deal.BidType,
                BidValue = deal.BidValue,
                PerPeriodValue = deal.PerPeriodValue,
                SuccessProbability = deal.DealMilestoneProbability,
                ActualCloseDate = ApiDateTimeHelper.Get(deal.ActualCloseDate),
                ExpectedCloseDate = ApiDateTimeHelper.Get(deal.ExpectedCloseDate),
                CanEdit = CRMSecurity.CanEdit(deal)
            };

            if (deal.ContactID > 0)
                dealDto.Contact = ContactBaseDtoHelper.GetContactBaseDto(DaoFactory.GetContactDao().GetByID(deal.ContactID));

            if (deal.DealMilestoneID > 0)
            {
                var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(deal.DealMilestoneID);

                if (dealMilestone == null)
                    throw new ItemNotFoundException();

                dealDto.Stage = new DealMilestoneBaseDto(dealMilestone);
            }

            dealDto.AccessList = CRMSecurity.GetAccessSubjectTo(deal)
                                                .Select(item => EmployeeWraperHelper.Get(item.Key));

            dealDto.IsPrivate = CRMSecurity.IsPrivate(deal);

            if (!string.IsNullOrEmpty(deal.BidCurrency))
                dealDto.BidCurrency = CurrencyInfoDtoHelper.Get(CurrencyProvider.Get(deal.BidCurrency));

            dealDto.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, deal.ID, false).ConvertAll(item => new CustomFieldBaseDto(item));

            dealDto.Members = new List<ContactBaseDto>();

            var memberIDs = DaoFactory.GetDealDao().GetMembers(deal.ID);
            var membersList = DaoFactory.GetContactDao().GetContacts(memberIDs);
            var membersDtoList = new List<ContactBaseDto>();

            foreach (var member in membersList)
            {
                if (member == null) continue;
                membersDtoList.Add(ContactBaseDtoHelper.GetContactBaseDto(member));
            }

            dealDto.Members = membersDtoList;

            return dealDto;
        }
    }
}