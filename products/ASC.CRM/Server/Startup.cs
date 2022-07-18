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

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.Api;
using ASC.CRM.ApiModels;
using ASC.CRM.HttpHandlers;
using ASC.CRM.Mapping;
using ASC.Web.CRM.Core.Search;
using ASC.Web.CRM.HttpHandlers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.CRM
{

    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
             : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.ConfigureServices(services);

            services.AddHttpClient("DownloadCurrencyPage").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 2,
                    UseDefaultCredentials = true
                };
            });

            DIHelper.TryAdd<EntryPointApiController>();
            DIHelper.TryAdd<CasesController>();
            DIHelper.TryAdd<ContactInfosController>();
            DIHelper.TryAdd<ContactsController>();
            DIHelper.TryAdd<CurrencyRatesController>();
            DIHelper.TryAdd<CustomFieldsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<DealsController>();
            DIHelper.TryAdd<InvoicesController>();
            DIHelper.TryAdd<ListItemsController>();
            DIHelper.TryAdd<RelationshipEventsController>();
            DIHelper.TryAdd<ReportsController>();
            DIHelper.TryAdd<TagsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<TaskTemplateController>();
            DIHelper.TryAdd<UtilsController>();

            DIHelper.TryAdd<CasesDtoTypeConverter>();
            DIHelper.TryAdd<ContactDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceBaseDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceItemDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceTaxDtoTypeConverter>();
            DIHelper.TryAdd<OpportunityDtoTypeConverter>();
            DIHelper.TryAdd<RelationshipEventDtoTypeConverter>();
            DIHelper.TryAdd<ListItemDtoTypeConverter>();
            DIHelper.TryAdd<TaskDtoTypeConverter>();
            DIHelper.TryAdd<CustomFieldDtoTypeConverter>();
            DIHelper.TryAdd<DealMilestoneDtoTypeConverter>();

            DIHelper.TryAdd<FactoryIndexerCase>();
            DIHelper.TryAdd<FactoryIndexerContact>();
            DIHelper.TryAdd<FactoryIndexerContactInfo>();
            DIHelper.TryAdd<FactoryIndexerDeal>();
            DIHelper.TryAdd<FactoryIndexerEvents>();
            DIHelper.TryAdd<FactoryIndexerFieldValue>();
            DIHelper.TryAdd<FactoryIndexerInvoice>();
            DIHelper.TryAdd<FactoryIndexerTask>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            app.UseMiddleware<TenantConfigureMiddleware>();

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/contactphotohandler.ashx"),
                appBranch =>
                {
                    appBranch.UseContactPhotoHandler();
                });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/filehandler.ashx"),
              appBranch =>
              {
                  appBranch.UseFileHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/fileuploaderhandler.ashx"),
              appBranch =>
              {
                  appBranch.UseFileUploaderHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/importfilehandler.ashx"),
              appBranch =>
              {
                  appBranch.UseImportFileHandlerHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/organisationlogohandler.ashx"),
              appBranch =>
              {
                  appBranch.UseOrganisationLogoHandler();
              });


            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/webtoleadfromhandler.ashx"),
              appBranch =>
              {
                  appBranch.UseWebToLeadFromHandlerHandler();
              });

        }

        protected override JsonConverter[] Converters
        {
            get
            {
                var jsonConverters = new List<JsonConverter>
                {
                    new ContactDtoJsonConverter()
                };

                return jsonConverters.ToArray();
            }
        }

    }
}


