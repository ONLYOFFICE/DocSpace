// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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

using ASC.Common;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.CRM.Core.Dao
{
    [Scope(Additional = typeof(DaoFactoryExtension))]
    public class DaoFactory
    {
        public DaoFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public TaskDao GetTaskDao()
        {
            return ServiceProvider.GetService<TaskDao>();
        }

        public ListItemDao GetCachedListItem()
        {
            return ServiceProvider.GetService<ListItemDao>();
        }

        public ContactDao GetContactDao()
        {
            return ServiceProvider.GetService<ContactDao>();
        }

        public CustomFieldDao GetCustomFieldDao()
        {
            return ServiceProvider.GetService<CustomFieldDao>();
        }

        public DealDao GetDealDao()
        {
            return ServiceProvider.GetService<DealDao>();
        }

        public DealMilestoneDao GetDealMilestoneDao()
        {
            return ServiceProvider.GetService<DealMilestoneDao>();

        }

        public ListItemDao GetListItemDao()
        {
            return ServiceProvider.GetService<ListItemDao>();
        }

        public TagDao GetTagDao()
        {
            return ServiceProvider.GetService<TagDao>();
        }

        public SearchDao GetSearchDao()
        {
            return ServiceProvider.GetService<SearchDao>();
        }

        public RelationshipEventDao GetRelationshipEventDao()
        {
            return ServiceProvider.GetService<RelationshipEventDao>();
        }

        public FileDao GetFileDao()
        {
            return ServiceProvider.GetService<FileDao>();
        }

        public CasesDao GetCasesDao()
        {
            return ServiceProvider.GetService<CasesDao>();
        }

        public TaskTemplateContainerDao GetTaskTemplateContainerDao()
        {
            return ServiceProvider.GetService<TaskTemplateContainerDao>();
        }

        public TaskTemplateDao GetTaskTemplateDao()
        {
            return ServiceProvider.GetService<TaskTemplateDao>();
        }

        public ReportDao GetReportDao()
        {
            return ServiceProvider.GetService<ReportDao>();
        }

        public CurrencyRateDao GetCurrencyRateDao()
        {
            return ServiceProvider.GetService<CurrencyRateDao>();
        }

        public CurrencyInfoDao GetCurrencyInfoDao()
        {
            return ServiceProvider.GetService<CurrencyInfoDao>();
        }

        public ContactInfoDao GetContactInfoDao()
        {
            return ServiceProvider.GetService<ContactInfoDao>();
        }

        public InvoiceDao GetInvoiceDao()
        {
            return ServiceProvider.GetService<InvoiceDao>();
        }

        public InvoiceItemDao GetInvoiceItemDao()
        {
            return ServiceProvider.GetService<InvoiceItemDao>();
        }

        public InvoiceTaxDao GetInvoiceTaxDao()
        {
            return ServiceProvider.GetService<InvoiceTaxDao>();
        }

        public InvoiceLineDao GetInvoiceLineDao()
        {
            return ServiceProvider.GetService<InvoiceLineDao>();
        }
    }

    public class DaoFactoryExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<TaskDao>();
            services.TryAdd<ListItemDao>();
            services.TryAdd<ContactDao>();
            services.TryAdd<CustomFieldDao>();
            services.TryAdd<DealDao>();
            services.TryAdd<DealMilestoneDao>();
            services.TryAdd<TagDao>();
            services.TryAdd<SearchDao>();
            services.TryAdd<RelationshipEventDao>();
            services.TryAdd<FileDao>();
            services.TryAdd<CasesDao>();
            services.TryAdd<TaskTemplateContainerDao>();
            services.TryAdd<TaskTemplateDao>();

            services.TryAdd<ReportDao>();
            services.TryAdd<CurrencyRateDao>();

            services.TryAdd<CurrencyInfoDao>();
            services.TryAdd<ContactInfoDao>();


            services.TryAdd<InvoiceDao>();
            services.TryAdd<InvoiceItemDao>();
            services.TryAdd<InvoiceTaxDao>();
            services.TryAdd<InvoiceLineDao>();
        }
    }

}

//TaskDao
//ListItemDao
//ContactDao
//CustomFieldDao
//DealDao
//DealMilestoneDao

//TagDao
//SearchDao
//RelationshipEventDao
//FileDao
//CasesDao
//TaskTemplateContainerDao
//TaskTemplateDao


//ReportDao
//CurrencyRateDao
//CurrencyInfoDao
//ContactInfoDao


//InvoiceDao
//InvoiceItemDao
//InvoiceTaxDao
//InvoiceLineDao