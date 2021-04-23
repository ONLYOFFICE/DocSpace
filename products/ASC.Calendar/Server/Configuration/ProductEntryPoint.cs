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

using ASC.Calendar.Resources;
using ASC.Common;
using ASC.Core;
using ASC.Web.Calendar.Classes;
using ASC.Web.Core;

namespace ASC.Calendar.Configuration
{
    [Scope]
    public class ProductEntryPoint : Product
    {
        private ProductContext context;

        public static readonly Guid ID = WebItemManager.CalendarProductID;

        private AuthContext AuthContext { get; }
        private UserManager UserManager { get; }
        private PathProvider PathProvider { get; }

        public ProductEntryPoint(
            AuthContext authContext, 
            UserManager userManager,
            PathProvider pathProvider)
        {
            AuthContext = authContext;
            UserManager = userManager;
            PathProvider = pathProvider;
        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return CalendarAddonResource.AddonName; }
        }

        public override string Description
        {
            get { return CalendarAddonResource.AddonDescription; }
        }

        public override string StartURL
        {
            get { return PathProvider.BaseAbsolutePath; }
        }

        public override string HelpURL
        {
            get { return "https://helpcenter.onlyoffice.com/userguides/calendar.aspx"; }
        }

        public override string ProductClassName
        {
            get { return "calendar"; }
        }

        public override bool Visible { get { return true; } }

        public override ProductContext Context
        {
            get { return context; }
        }

        public override string ApiURL => "api/2.0/calendar/info.json";

        public override void Init()
        {
            context = new ProductContext
            {
                //MasterPageFile = String.Concat(PathProvider.BaseVirtualPath, "Masters/BasicTemplate.Master"),
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "images/calendar.menu.svg",
                LargeIconFileName = "images/calendar.svg",
                //SubscriptionManager = new ProductSubscriptionManager(),
                DefaultSortOrder = 20,
                //SpaceUsageStatManager = new ProjectsSpaceUsageStatManager(),
                HasComplexHierarchyOfAccessRights = true,
            };
        }
    }
}