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
using ASC.Core;
using ASC.CRM.Resources;
using ASC.Web.Core;
using ASC.Web.Files.Api;


namespace ASC.Web.CRM.Configuration
{
    [Scope]
    public class ProductEntryPoint : Product
    {
        private ProductContext _context;
        private FilesIntegration _filesIntegration;
        private PathProvider _pathProvider;
        private SecurityContext _securityContext;
        private UserManager _userManager;

        public ProductEntryPoint(SecurityContext securityContext,
                                 UserManager userManager,
                                 PathProvider pathProvider,
                                 FilesIntegration filesIntegration)
        {
            _securityContext = securityContext;
            _userManager = userManager;
            _pathProvider = pathProvider;
            _filesIntegration = filesIntegration;
        }

        public static readonly Guid ID = WebItemManager.CRMProductID;
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
                var id = _securityContext.CurrentAccount.ID;

                if (_userManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupAdmin.ID) || _userManager.IsUserInGroup(id, ID))
                    return CRMCommonResource.ProductDescriptionEx;

                return CRMCommonResource.ProductDescription;
            }
        }

        public override string StartURL { get { return _pathProvider.StartURL(); } }
        public override string HelpURL { get { return string.Concat(_pathProvider.BaseVirtualPath, "help.aspx"); } }
        public override string ProductClassName { get { return "crm"; } }
        public override bool Visible { get { return true; } }
        public override ProductContext Context { get { return _context; } }
        public string ModuleSysName { get; set; }

        public override void Init()
        {
            _context = new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "images/crm.menu.svg",
                LargeIconFileName = "images/crm.svg",
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