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

namespace ASC.Files.Core;


[DebuggerDisplay("")]
public class EntryProperties
{
    public FormFillingProperties FormFilling { get; set; }

    public static EntryProperties Deserialize(string data, ILogger logger)
    {
        try
        {
            return JsonSerializer.Deserialize<EntryProperties>(data);
        }
        catch (Exception e)
        {
            logger.ErrorWithException("Error parse EntryProperties: " + data, e);
            return null;
        }
    }

    public static string Serialize(EntryProperties entryProperties, ILogger logger)
    {
        try
        {
            return JsonSerializer.Serialize(entryProperties);
        }
        catch (Exception e)
        {
            logger.ErrorWithException("Error serialize EntryProperties", e);
            return null;
        }
    }
}

[Transient]
public class FormFillingProperties
{
    public static readonly string DefaultTitleMask = "{0} - {1} ({2})";
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly TenantUtil _tenantUtil;
    private readonly CustomNamingPeople _customNamingPeople;

    public bool CollectFillForm { get; set; }
    public string ToFolderId { get; set; }
    public string ToFolderPath { get; set; }
    public string CreateFolderTitle { get; set; }
    public string CreateFileMask { get; set; }

    public FormFillingProperties(
        UserManager userManager,
        SecurityContext securityContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        TenantUtil tenantUtil,
        CustomNamingPeople customNamingPeople)
    {
        _userManager = userManager;
        _securityContext = securityContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _tenantUtil = tenantUtil;
        _customNamingPeople = customNamingPeople;
    }

    public void FixFileMask()
    {
        if (string.IsNullOrEmpty(CreateFileMask))
        {
            return;
        }

        var indFileName = CreateFileMask.IndexOf("{0}");
        CreateFileMask = CreateFileMask.Replace("{0}", "");

        var indUserName = CreateFileMask.IndexOf("{1}");
        CreateFileMask = CreateFileMask.Replace("{1}", "");

        var indDate = CreateFileMask.IndexOf("{2}");
        CreateFileMask = CreateFileMask.Replace("{2}", "");

        CreateFileMask = "_" + CreateFileMask + "_";
        CreateFileMask = Global.ReplaceInvalidCharsAndTruncate(CreateFileMask);
        CreateFileMask = CreateFileMask.Substring(1, CreateFileMask.Length - 2);

        if (indDate >= 0)
        {
            CreateFileMask = CreateFileMask.Insert(indDate, "{2}");
        }

        if (indUserName >= 0)
        {
            CreateFileMask = CreateFileMask.Insert(indUserName, "{1}");
        }

        if (indFileName >= 0)
        {
            CreateFileMask = CreateFileMask.Insert(indFileName, "{0}");
        }
    }

    public async Task<string> GetTitleByMaskAsync(string sourceFileName)
    {
        FixFileMask();

        var mask = CreateFileMask;
        if (string.IsNullOrEmpty(mask))
        {
            mask = DefaultTitleMask;
        }

        string userName;
        var userInfo = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        if (userInfo.Equals(Constants.LostUser))
        {
            userName = _customNamingPeople.Substitute<FilesCommonResource>("ProfileRemoved");
        }
        else
        {
            userName = userInfo.DisplayUserName(false, _displayUserSettingsHelper);
        }

        var title = mask
            .Replace("{0}", Path.GetFileNameWithoutExtension(sourceFileName))
            .Replace("{1}", userName)
            .Replace("{2}", _tenantUtil.DateTimeNow().ToString("g"));

        if (FileUtility.GetFileExtension(title) != "docx")
        {
            title += ".docx";
        }

        return title;
    }
}