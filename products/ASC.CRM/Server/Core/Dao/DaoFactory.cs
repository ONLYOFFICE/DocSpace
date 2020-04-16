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

using Microsoft.Extensions.DependencyInjection;
using ASC.VoipService.Dao;
using System;
using ASC.Common;
using ASC.Core.Common.Settings;

namespace ASC.CRM.Core.Dao
{
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

        public VoipDao GetVoipDao()
        {
            return ServiceProvider.GetService<VoipDao>();
        }
    }

    public static class DaoFactoryExtention
    {
        public static DIHelper AddDaoFactoryService(this DIHelper services)
        {
            services.TryAddScoped<DaoFactory>();

            //    return services;
            return services.AddTaskDaoService()
                           .AddListItemDaoService()
                           .AddContactDaoService()
                           .AddCustomFieldDaoService()
                           .AddDealDaoService()
                           .AddDealMilestoneDaoService()
                           .AddTagDaoService()
                           .AddSearchDaoService()
                           .AddRelationshipEventDaoService()
                           .AddFileDaoService()
                           .AddCasesDaoService()
                           .AddTaskTemplateDaoService()
                           .AddTaskTemplateContainerDaoService()
                         //  .AddReportDaoService()
                           .AddCurrencyRateDaoService()
                           .AddCurrencyInfoDaoService()
                           .AddContactInfoDaoService()
                           .AddInvoiceDaoService()
                           .AddInvoiceLineDaoService()
                           .AddInvoiceTaxDaoService()
                           .AddInvoiceLineDaoService();
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
//VoipDao