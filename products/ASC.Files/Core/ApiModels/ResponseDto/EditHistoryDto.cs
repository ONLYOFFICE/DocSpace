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

namespace ASC.Files.Core.ApiModels.ResponseDto;

/// <summary>
/// </summary>
public class EditHistoryDto
{
    /// <summary>File ID</summary>
    /// <type>System.Int32, System</type>
    public int ID { get; set; }

    /// <summary>Key</summary>
    /// <type>System.String, System</type>
    public string Key { get; set; }

    /// <summary>File version</summary>
    /// <type>System.Int32, System</type>
    public int Version { get; set; }

    /// <summary>Version group</summary>
    /// <type>System.Int32, System</type>
    public int VersionGroup { get; set; }

    /// <summary>A user who updated a file</summary>
    /// <type>ASC.Files.Core.EditHistoryAuthor, ASC.Files.Core</type>
    public EditHistoryAuthor User { get; set; }

    /// <summary>Creation time</summary>
    /// <type>ASC.Api.Core.ApiDateTime, ASC.Api.Core</type>
    public ApiDateTime Created { get; set; }

    /// <summary>History changes in the string format</summary>
    /// <type>System.String, System</type>
    public string ChangesHistory { get; set; }

    /// <summary>List of history changes</summary>
    /// <type>System.Collections.Generic.List{ASC.Files.Core.ApiModels.ResponseDto.EditHistoryChangesWrapper}, System.Collections.Generic</type>
    public List<EditHistoryChangesWrapper> Changes { get; set; }

    /// <summary>Server version</summary>
    /// <type>System.String, System</type>
    public string ServerVersion { get; set; }

    public EditHistoryDto(EditHistory editHistory, ApiDateTimeHelper apiDateTimeHelper, UserManager userManager, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        ID = editHistory.ID;
        Key = editHistory.Key;
        Version = editHistory.Version;
        VersionGroup = editHistory.VersionGroup;
        Changes = editHistory.Changes.Select(r => new EditHistoryChangesWrapper(r, apiDateTimeHelper)).ToList();
        ChangesHistory = editHistory.ChangesString;
        Created = apiDateTimeHelper.Get(editHistory.ModifiedOn);
        User = new EditHistoryAuthor(userManager, displayUserSettingsHelper) { Id = editHistory.ModifiedBy.ToString() };
        ServerVersion = editHistory.ServerVersion;
    }
}

public class EditHistoryChangesWrapper
{
    public EditHistoryAuthor User { get; set; }
    public ApiDateTime Created { get; set; }

    public EditHistoryChangesWrapper(EditHistoryChanges historyChanges, ApiDateTimeHelper apiDateTimeHelper)
    {
        User = historyChanges.Author;
        Created = apiDateTimeHelper.Get(historyChanges.Date);
    }
}
