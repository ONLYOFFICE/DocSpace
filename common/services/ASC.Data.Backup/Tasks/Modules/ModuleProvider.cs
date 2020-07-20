/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;

using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Tasks.Modules
{
    public class ModuleProvider
    {
        public List<IModuleSpecifics> AllModules;

        public ModuleProvider(IOptionsMonitor<ILog> options, Helpers helpers, CoreSettings coreSettings)
        {
            AllModules = new List<IModuleSpecifics>
            {
                new TenantsModuleSpecifics(coreSettings,helpers),
                new AuditModuleSpecifics(helpers),
                new CommunityModuleSpecifics(helpers),
                new CalendarModuleSpecifics(helpers),
                new ProjectsModuleSpecifics(helpers),
                new CrmModuleSpecifics(helpers),
                new FilesModuleSpecifics(options,helpers),
                new MailModuleSpecifics(options,helpers),
                new CrmModuleSpecifics2(helpers),
                new FilesModuleSpecifics2(helpers),
                new CrmInvoiceModuleSpecifics(helpers),
                new WebStudioModuleSpecifics(helpers),
                new CoreModuleSpecifics(helpers)
            }
            .ToList();
        }
        public IModuleSpecifics GetByStorageModule(string storageModuleName, string storageDomainName = null)
        {
            switch (storageModuleName)
            {
                case "files":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Files);

                case "projects":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Projects);

                case "crm":
                    return AllModules.FirstOrDefault(m => m.ModuleName == (storageDomainName == "mail_messages" ? ModuleName.Crm2 : ModuleName.Crm));

                case "forum":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Community);

                case "mailaggregator":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Mail);

                default:
                    return null;
            }
        }
    }
    public static class ModuleProviderExtension
    {
        public static DIHelper AddModuleProvider(this DIHelper services)
        {
            if (services.TryAddScoped<ModuleProvider>())
            {
                return services
                    .AddCoreSettingsService()
                    .AddHelpers();
            }

            return services;
        }
    }
}
