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


using System.Collections.Generic;
using System.Linq;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Web;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using ASC.Web.CRM.Classes;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class OpportunityDtoTypeConverter : ITypeConverter<Deal, OpportunityDto>
    {
        private readonly CurrencyProvider _currencyProvider;
        private readonly DaoFactory _daoFactory;
        private readonly CrmSecurity _crmSecurity;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;

        public OpportunityDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeDtoHelper employeeDtoHelper,
                           CrmSecurity crmSecurity,
                           DaoFactory daoFactory,
                           CurrencyProvider currencyProvider)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _crmSecurity = crmSecurity;
            _daoFactory = daoFactory;
            _currencyProvider = currencyProvider;
        }

        public OpportunityDto Convert(Deal source, OpportunityDto destination, ResolutionContext context)
        {
            var dealDto = new OpportunityDto
            {
                Id = source.ID,
                CreateBy = _employeeDtoHelper.Get(source.CreateBy),
                Created = _apiDateTimeHelper.Get(source.CreateOn),
                Title = source.Title,
                Description = source.Description,
                Responsible = _employeeDtoHelper.Get(source.ResponsibleID),
                BidType = source.BidType,
                BidValue = source.BidValue,
                PerPeriodValue = source.PerPeriodValue,
                SuccessProbability = source.DealMilestoneProbability,
                ActualCloseDate = _apiDateTimeHelper.Get(source.ActualCloseDate),
                ExpectedCloseDate = _apiDateTimeHelper.Get(source.ExpectedCloseDate),
                CanEdit = _crmSecurity.CanEdit(source)
            };

            if (source.ContactID > 0)
                dealDto.Contact = context.Mapper.Map<ContactBaseDto>(_daoFactory.GetContactDao().GetByID(source.ContactID));

            if (source.DealMilestoneID > 0)
            {
                var dealMilestone = _daoFactory.GetDealMilestoneDao().GetByID(source.DealMilestoneID);

                if (dealMilestone == null)
                    throw new ItemNotFoundException();

                dealDto.Stage = new DealMilestoneBaseDto(dealMilestone);
            }

            dealDto.AccessList = _crmSecurity.GetAccessSubjectTo(source)
                                                .Select(item => _employeeDtoHelper.Get(item.Key));

            dealDto.IsPrivate = _crmSecurity.IsPrivate(source);

            if (!string.IsNullOrEmpty(source.BidCurrency))
                dealDto.BidCurrency = context.Mapper.Map<CurrencyInfoDto>(_currencyProvider.Get(source.BidCurrency));

            dealDto.CustomFields = _daoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, source.ID, false).ConvertAll(item => context.Mapper.Map<CustomFieldBaseDto>(item));

            dealDto.Members = new List<ContactBaseDto>();

            var memberIDs = _daoFactory.GetDealDao().GetMembers(source.ID);
            var membersList = _daoFactory.GetContactDao().GetContacts(memberIDs);
            var membersDtoList = new List<ContactBaseDto>();

            foreach (var member in membersList)
            {
                if (member == null) continue;
                membersDtoList.Add(context.Mapper.Map<ContactBaseDto>(member));
            }

            dealDto.Members = membersDtoList;

            return dealDto;
        }
    }
}