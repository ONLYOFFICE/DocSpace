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

namespace ASC.Api.Core;

/// <summary>
/// </summary>
public class Module
{
    /// <summary>ID</summary>
    /// <type>System.Guid, System</type>
    public Guid Id { get; set; }

    /// <summary>Product class name</summary>
    /// <type>System.String, System</type>
    public string AppName { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }

    /// <summary>Start link</summary>
    /// <type>System.String, System</type>
    public string Link { get; set; }

    /// <summary>Icon URL</summary>
    /// <type>System.String, System</type>
    public string IconUrl { get; set; }

    /// <summary>Large image URL</summary>
    /// <type>System.String, System</type>
    public string ImageUrl { get; set; }

    /// <summary>Help URL</summary>
    /// <type>System.String, System</type>
    public string HelpUrl { get; set; }

    /// <summary>Description</summary>
    /// <type>System.String, System</type>
    public string Description { get; set; }

    /// <summary>Specifies if the module is primary or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsPrimary { get; set; }

    public Module(Product product)
    {
        Id = product.ProductID;
        AppName = product.ProductClassName;
        Title = product.Name;
        Description = product.Description;
        IconUrl = product.Context.IconFileName;
        ImageUrl = product.Context.LargeIconFileName;
        Link = product.StartURL;
        IsPrimary = product.IsPrimary;
        HelpUrl = product.HelpURL;
    }
}