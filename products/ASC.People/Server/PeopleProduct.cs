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

namespace ASC.People;

public class PeopleProduct : Product
{
    internal const string ProductPath = "/products/people/";

    private ProductContext _context;
    public static Guid ID => new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}");
    public override bool Visible => true;
    public override ProductContext Context => _context;
    public override string Name => PeopleResource.ProductName;
    public override string Description => PeopleResource.ProductDescription;
    public override string ExtendedDescription => PeopleResource.ProductDescription;
    public override Guid ProductID => ID;
    public override string StartURL => ProductPath;
    public override string HelpURL => string.Concat(ProductPath, "help.aspx");
    public override string ProductClassName => "people";
    public override string ApiURL => "api/2.0/people/info.json";
    public override bool IsPrimary { get => false; }

    public override void Init()
    {
        _context = new ProductContext
        {
            DisabledIconFileName = "product_disabled_logo.png",
            IconFileName = "images/people.menu.svg",
            LargeIconFileName = "images/people.svg",
            DefaultSortOrder = 50,
            AdminOpportunities = () => PeopleResource.ProductAdminOpportunities.Split('|').ToList(),
            UserOpportunities = () => PeopleResource.ProductUserOpportunities.Split('|').ToList()
        };

        //SearchHandlerManager.Registry(new SearchHandler());
    }
}
