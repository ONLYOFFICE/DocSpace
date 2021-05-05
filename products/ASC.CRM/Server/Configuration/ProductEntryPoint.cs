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
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Configuration;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.Files.Api;

using Microsoft.Extensions.Options;


namespace ASC.Web.CRM.Configuration
{
    [Scope]
    public class ProductEntryPoint : Product
    {
        public ProductEntryPoint(SecurityContext securityContext,
                                 UserManager userManager,
                                 PathProvider pathProvider,
                                 FilesIntegration filesIntegration)
        {
            SecurityContext = securityContext;
            UserManager = userManager;
            PathProvider = pathProvider;
            FilesIntegration = filesIntegration;
        }

        public static readonly Guid ID = WebItemManager.CRMProductID;
        private ProductContext _context;
        public FilesIntegration FilesIntegration { get; }
        public PathProvider PathProvider { get; }
        public SecurityContext SecurityContext { get; }
        public UserManager UserManager { get; }
        public override string ApiURL
        {
            get => "api/2.0/crm/info.json";
        }
        public override Guid ProductID { get { return ID; } }
        public override string Name { get { return CRMCommonResource.ProductName; } }
        public override string Description
        {
            get
            {
                var id = SecurityContext.CurrentAccount.ID;

                if (UserManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupAdmin.ID) || UserManager.IsUserInGroup(id, ID))
                    return CRMCommonResource.ProductDescriptionEx;

                return CRMCommonResource.ProductDescription;
            }
        }

        public override string StartURL { get { return PathProvider.StartURL(); } }
        public override string HelpURL { get { return string.Concat(PathProvider.BaseVirtualPath, "help.aspx"); } }
        public override string ProductClassName { get { return "crm"; } }
        public override bool Visible { get { return true; } }
        public override ProductContext Context { get { return _context; } }
        public string ModuleSysName { get; set; }

        public override void Init()
        {
            _context = new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "product_logo.png",
                LargeIconFileName = "product_logolarge.svg",
                DefaultSortOrder = 30,
                //  SubscriptionManager = new ProductSubscriptionManager(),
                //  SpaceUsageStatManager = new CRMSpaceUsageStatManager(),
                AdminOpportunities = () => CRMCommonResource.ProductAdminOpportunities.Split('|').ToList(),
                UserOpportunities = () => CRMCommonResource.ProductUserOpportunities.Split('|').ToList(),
            };

            //if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
            //{
            //    FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", FileSecurityProvider);
            //}
            //if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "opportunity"))
            //{
            //    FilesIntegration.RegisterFileSecurityProvider("crm", "opportunity", FileSecurityProvider);
            //}

            //            SearchHandlerManager.Registry(new SearchHandler());

            //GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            //    name: "Twilio", 
            //    routeTemplate: "twilio/{action}", 
            //    defaults: new {controller = "Twilio", action = "index" });

            //            ClientScriptLocalization = new ClientLocalizationResources();
        }

        //public override void Shutdown()
        //{
        //    if (registered)
        //    {
        //        NotifyClient.Client.UnregisterSendMethod(NotifyClient.SendAutoReminderAboutTask);

        //    }
        //}

        //public static void RegisterSendMethods()
        //{
        //    lock (Locker)
        //    {
        //        if (!registered)
        //        {
        //            registered = true;

        //            NotifyClient.Client.RegisterSendMethod(NotifyClient.SendAutoReminderAboutTask, "0 * * ? * *");

        //        }
        //    }
        //}
    }
}