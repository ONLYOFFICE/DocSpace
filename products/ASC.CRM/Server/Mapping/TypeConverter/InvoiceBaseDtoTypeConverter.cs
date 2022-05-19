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
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;
using ASC.Web.CRM.Classes;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class InvoiceBaseDtoTypeConverter : ITypeConverter<Invoice, InvoiceBaseDto>
    {
        private readonly EntityDtoHelper _entityDtoHelper;
        private readonly CurrencyProvider _currencyProvider;
        private readonly SettingsManager _settingsManager;
        private readonly CrmSecurity _crmSecurity;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;
        private readonly DaoFactory _daoFactory;

        public InvoiceBaseDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeDtoHelper employeeDtoHelper,
                           CrmSecurity crmSecurity,
                           SettingsManager settingsManager,
                           CurrencyProvider currencyProvider,
                           DaoFactory daoFactory,
                           EntityDtoHelper entityDtoHelper)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _crmSecurity = crmSecurity;
            _settingsManager = settingsManager;
            _currencyProvider = currencyProvider;
            _daoFactory = daoFactory;
            _entityDtoHelper = entityDtoHelper;
        }

        public InvoiceBaseDto Convert(Invoice source, InvoiceBaseDto destination, ResolutionContext context)
        {
            var crmSettings = _settingsManager.Load<CrmSettings>();
            var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency);

            var result = new InvoiceBaseDto
            {
                Id = source.ID,
                Status = context.Mapper.Map<InvoiceStatusDto>(source.Status),
                Number = source.Number,
                IssueDate = _apiDateTimeHelper.Get(source.IssueDate),
                TemplateType = source.TemplateType,
                DueDate = _apiDateTimeHelper.Get(source.DueDate),
                Currency = !String.IsNullOrEmpty(source.Currency) ?
                            context.Mapper.Map<CurrencyInfoDto>(_currencyProvider.Get(source.Currency)) :
                            context.Mapper.Map<CurrencyInfoDto>(defaultCurrency),
                ExchangeRate = source.ExchangeRate,
                Language = source.Language,
                PurchaseOrderNumber = source.PurchaseOrderNumber,
                Terms = source.Terms,
                Description = source.Description,
                FileID = source.FileID,
                CreateOn = _apiDateTimeHelper.Get(source.CreateOn),
                CreateBy = _employeeDtoHelper.Get(source.CreateBy),
                CanEdit = _crmSecurity.CanEdit(source),
                CanDelete = _crmSecurity.CanDelete(source)
            };

            if (source.ContactID > 0)
            {
                result.Contact = context.Mapper.Map<ContactBaseWithEmailDto>(_daoFactory.GetContactDao().GetByID(source.ContactID));
            }

            if (source.ConsigneeID > 0)
            {
                result.Consignee = context.Mapper.Map<ContactBaseWithEmailDto>(_daoFactory.GetContactDao().GetByID(source.ConsigneeID));
            }

            if (source.EntityID > 0)
            {
                result.Entity = _entityDtoHelper.Get(source.EntityType, source.EntityID);
            }

            result.Cost = source.GetInvoiceCost(_daoFactory);

            return result;

        }
    }
}
