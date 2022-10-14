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

namespace ASC.Web.Files.Configuration;

[Scope]
public class ProductEntryPoint : Product
{
    internal const string ProductPath = "/products/files/";

    //public FilesSpaceUsageStatManager FilesSpaceUsageStatManager { get; }
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly NotifyConfiguration _notifyConfiguration;

    //public SubscriptionManager SubscriptionManager { get; }

    public ProductEntryPoint() { }

    public ProductEntryPoint(
        //            FilesSpaceUsageStatManager filesSpaceUsageStatManager,
        CoreBaseSettings coreBaseSettings,
        AuthContext authContext,
        UserManager userManager,
        NotifyConfiguration notifyConfiguration
        //            SubscriptionManager subscriptionManager
        )
    {
        //            FilesSpaceUsageStatManager = filesSpaceUsageStatManager;
        _coreBaseSettings = coreBaseSettings;
        _authContext = authContext;
        _userManager = userManager;
        _notifyConfiguration = notifyConfiguration;
        //SubscriptionManager = subscriptionManager;
    }

    public static readonly Guid ID = WebItemManager.DocumentsProductID;

    private ProductContext _productContext;

    public override bool Visible => true;
    public override bool IsPrimary => true;

    public override void Init()
    {
        List<string> adminOpportunities() => (_coreBaseSettings.CustomMode
                                                           ? CustomModeResource.ProductAdminOpportunitiesCustomMode
                                                           : FilesCommonResource.ProductAdminOpportunities).Split('|').ToList();

        List<string> userOpportunities() => (_coreBaseSettings.CustomMode
                                     ? CustomModeResource.ProductUserOpportunitiesCustomMode
                                     : FilesCommonResource.ProductUserOpportunities).Split('|').ToList();

        _productContext =
            new ProductContext
            {
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "images/files.menu.svg",
                LargeIconFileName = "images/files.svg",
                DefaultSortOrder = 10,
                //SubscriptionManager = SubscriptionManager,
                //SpaceUsageStatManager = FilesSpaceUsageStatManager,
                AdminOpportunities = adminOpportunities,
                UserOpportunities = userOpportunities,
                CanNotBeDisabled = true,
            };

        if (_notifyConfiguration != null)
        {
            _notifyConfiguration.Configure();
        }
        //SearchHandlerManager.Registry(new SearchHandler());
    }

    public string GetModuleResource(string ResourceClassTypeName, string ResourseKey)
    {
        if (string.IsNullOrEmpty(ResourseKey))
        {
            return string.Empty;
        }

        try
        {
            return (string)Type.GetType(ResourceClassTypeName).GetProperty(ResourseKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public override Guid ProductID => ID;
    public override string Name => FilesCommonResource.ProductName;

    public override string Description
    {
        get
        {
            var id = _authContext.CurrentAccount.ID;

            if (_userManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupUser.ID))
            {
                return FilesCommonResource.ProductDescriptionShort;
            }

            if (_userManager.IsUserInGroup(id, ASC.Core.Users.Constants.GroupAdmin.ID) || _userManager.IsUserInGroup(id, ID))
            {
                return FilesCommonResource.ProductDescriptionEx;
            }

            return FilesCommonResource.ProductDescription;
        }
    }

    public override string StartURL => ProductPath;
    public override string HelpURL => PathProvider.StartURL;
    public override string ProductClassName => "files";
    public override ProductContext Context => _productContext;
    public override string ApiURL => string.Empty;
}
