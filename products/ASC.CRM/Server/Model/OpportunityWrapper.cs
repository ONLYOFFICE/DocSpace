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

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Opportunity
    /// </summary>
    [DataContract(Name = "opportunity", Namespace = "")]
    public class OpportunityWrapper
    {
   
        public OpportunityWrapper()
        {
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Members { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BidType BidType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal BidValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CurrencyInfoWrapper BidCurrency { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int PerPeriodValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DealMilestoneBaseWrapper Stage { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int SuccessProbability { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime ActualCloseDate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime ExpectedCloseDate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }

        public static OpportunityWrapper GetSample()
        {
            return new OpportunityWrapper
            {
                CreateBy = EmployeeWraper.GetSample(),
                Created = ApiDateTime.GetSample(),
                Responsible = EmployeeWraper.GetSample(),
                Title = "Hotel catalogue",
                Description = "",
                ExpectedCloseDate = ApiDateTime.GetSample(),
                Contact = ContactBaseWrapper.GetSample(),
                IsPrivate = false,
                SuccessProbability = 65,
                BidType = BidType.FixedBid,
                Stage = DealMilestoneBaseWrapper.GetSample()
            };
        }
    }


    public class OpportunityWrapperHelper
    {
        public OpportunityWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           DaoFactory daoFactory,
                           CurrencyProvider currencyProvider,
                           ContactWrapperHelper contactBaseWrapperHelper
                           )
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseWrapperHelper = contactBaseWrapperHelper;
            CurrencyProvider = currencyProvider;
        }

        public CurrencyProvider CurrencyProvider  {get;}
        public ContactWrapperHelper ContactBaseWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }

        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }

        public OpportunityWrapper Get(Deal deal)
        {
            var dealWrapper = new OpportunityWrapper
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
                dealWrapper.Contact = ContactBaseWrapperHelper.GetContactBaseWrapper(DaoFactory.GetContactDao().GetByID(deal.ContactID));

            if (deal.DealMilestoneID > 0)
            {
                var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(deal.DealMilestoneID);

                if (dealMilestone == null)
                    throw new ItemNotFoundException();

                dealWrapper.Stage = new DealMilestoneBaseWrapper(dealMilestone);
            }

            dealWrapper.AccessList = CRMSecurity.GetAccessSubjectTo(deal)
                                                .Select(item => EmployeeWraperHelper.Get(item.Key)).ToItemList();

            dealWrapper.IsPrivate = CRMSecurity.IsPrivate(deal);

            if (!string.IsNullOrEmpty(deal.BidCurrency))
                dealWrapper.BidCurrency = ToCurrencyInfoWrapper(CurrencyProvider.Get(deal.BidCurrency));

            dealWrapper.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, deal.ID, false).ConvertAll(item => new CustomFieldBaseWrapper(item)).ToSmartList();

            dealWrapper.Members = new List<ContactBaseWrapper>();

            var memberIDs = DaoFactory.GetDealDao().GetMembers(deal.ID);
            var membersList = DaoFactory.GetContactDao().GetContacts(memberIDs);
            var membersWrapperList = new List<ContactBaseWrapper>();

            foreach (var member in membersList)
            {
                if (member == null) continue;
                membersWrapperList.Add(ContactBaseWrapperHelper.GetContactBaseWrapper(member));
            }

            dealWrapper.Members = membersWrapperList;

            return dealWrapper;
        }
    }

    public static class OpportunityWrapperHelperExtension
    {
        public static DIHelper AddOpportunityWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<OpportunityWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService()
                           .AddDaoFactoryService()
                           .AddContactWrapperHelperService()
                           .AddCurrencyProviderService();
        }
    }
}