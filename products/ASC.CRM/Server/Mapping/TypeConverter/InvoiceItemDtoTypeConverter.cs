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
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;
using ASC.Web.CRM.Classes;

using AutoMapper;

using System;

namespace ASC.CRM.Mapping
{
    public class InvoiceItemDtoTypeConverter : ITypeConverter<InvoiceItem, InvoiceItemDto>
    {
        private CurrencyInfoDtoHelper _currencyInfoDtoHelper;
        private DaoFactory _daoFactory;
        private CurrencyProvider _currencyProvider;
        private SettingsManager _settingsManager;
        private ApiDateTimeHelper _apiDateTimeHelper;
        private EmployeeWraperHelper _employeeWraperHelper;
        private CRMSecurity _CRMSecurity;

        public InvoiceItemDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                                    EmployeeWraperHelper employeeWraperHelper,
                                   CRMSecurity cRMSecurity,
                                   SettingsManager settingsManager,
                                   CurrencyProvider currencyProvider,
                                   DaoFactory daoFactory,
                                   CurrencyInfoDtoHelper currencyInfoDtoHelper)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeWraperHelper = employeeWraperHelper;
            _CRMSecurity = cRMSecurity;
            _settingsManager = settingsManager;
            _currencyProvider = currencyProvider;
            _daoFactory = daoFactory;
            _currencyInfoDtoHelper = currencyInfoDtoHelper;
        }

        public InvoiceItemDto Convert(InvoiceItem source, InvoiceItemDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

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
                _currencyInfoDtoHelper.Get(_currencyProvider.Get(source.Currency)) :
                _currencyInfoDtoHelper.Get(_settingsManager.Load<CRMSettings>().DefaultCurrency),
                CanEdit = _CRMSecurity.CanEdit(source),
                CanDelete = _CRMSecurity.CanDelete(source)
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
