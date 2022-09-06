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
    public class InvoiceItemDtoTypeConverter : ITypeConverter<InvoiceItem, InvoiceItemDto>
    {
        private readonly DaoFactory _daoFactory;
        private readonly CurrencyProvider _currencyProvider;
        private readonly SettingsManager _settingsManager;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeWraperHelper;
        private readonly CrmSecurity _crmSecurity;

        public InvoiceItemDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                                   EmployeeDtoHelper employeeWraperHelper,
                                   CrmSecurity crmSecurity,
                                   SettingsManager settingsManager,
                                   CurrencyProvider currencyProvider,
                                   DaoFactory daoFactory)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeWraperHelper = employeeWraperHelper;
            _crmSecurity = crmSecurity;
            _settingsManager = settingsManager;
            _currencyProvider = currencyProvider;
            _daoFactory = daoFactory;
        }

        public InvoiceItemDto Convert(InvoiceItem source, InvoiceItemDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var crmSettings = _settingsManager.Load<CrmSettings>();
            var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency);

            var result = new InvoiceItemDto
            {

                Title = source.Title,
                StockKeepingUnit = source.StockKeepingUnit,
                Description = source.Description,
                Price = source.Price,
                StockQuantity = source.StockQuantity,
                TrackInvenory = source.TrackInventory,
                CreateOn = _apiDateTimeHelper.Get(source.CreateOn),
                CreateBy = _employeeWraperHelper.Get(source.CreateBy),
                Currency = !String.IsNullOrEmpty(source.Currency) ?
                context.Mapper.Map<CurrencyInfoDto>(_currencyProvider.Get(source.Currency)) :
                context.Mapper.Map<CurrencyInfoDto>(defaultCurrency),
                CanEdit = _crmSecurity.CanEdit(source),
                CanDelete = _crmSecurity.CanDelete(source)
            };

            if (source.InvoiceTax1ID > 0)
            {
                var invoiceTax1ID = _daoFactory.GetInvoiceTaxDao().GetByID(source.InvoiceTax1ID);

                result.InvoiceTax1 = context.Mapper.Map<InvoiceTaxDto>(invoiceTax1ID);
            }

            if (source.InvoiceTax2ID > 0)
            {
                var invoiceTax2ID = _daoFactory.GetInvoiceTaxDao().GetByID(source.InvoiceTax2ID);

                result.InvoiceTax2 = context.Mapper.Map<InvoiceTaxDto>(invoiceTax2ID);
            }

            return result;
        }
    }
}
