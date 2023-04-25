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

namespace ASC.Files.Core.ApiModels.RequestDto;
/// <summary>
/// </summary>
public class EntryPropertiesRequestDto : IMapFrom<EntryProperties>
{
    /// <summary>Form filling request parameters</summary>
    /// <type>ASC.Files.Core.ApiModels.RequestDto.FormFillingPropertiesRequestDto, ASC.Files.Core</type>
    public FormFillingPropertiesRequestDto FormFilling { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap(typeof(EntryProperties), GetType());
        profile.CreateMap(GetType(), typeof(EntryProperties));
    }
}

/// <summary>
/// </summary>
public class FormFillingPropertiesRequestDto : IMapFrom<FormFillingProperties>
{
    /// <summary>Specifies if the data will be collected from the filled forms or not</summary>
    /// <type>System.Boolean, System</type>
    public bool CollectFillForm { get; set; }

    /// <summary>Folder ID where a file will be saved</summary>
    /// <type>System.String, System</type>
    public string ToFolderId { get; set; }

    /// <summary>Folder path where a file will be saved</summary>
    /// <type>System.String, System</type>
    public string ToFolderPath { get; set; }

    /// <summary>New folder title</summary>
    /// <type>System.String, System</type>
    public string CreateFolderTitle { get; set; }

    /// <summary>File name mask</summary>
    /// <type>System.String, System</type>
    public string CreateFileMask { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap(typeof(FormFillingProperties), GetType());
        profile.CreateMap(GetType(), typeof(FormFillingProperties));
    }
}

/// <summary>
/// </summary>
public class BatchEntryPropertiesRequestDto
{
    /// <summary>List of file IDs</summary>
    /// <type>System.Text.Json.JsonElement[], System.Text.Json</type>
    public JsonElement[] FilesId { get; set; }

    /// <summary>Specifies whether to create a subfolder or not</summary>
    /// <type>System.Boolean, System</type>
    public bool CreateSubfolder { get; set; }

    /// <summary>File properties that are represented as the EntryPropertiesRequestDto object</summary>
    /// <type>ASC.Files.Core.ApiModels.RequestDto.EntryPropertiesRequestDto, ASC.Files.Core</type>
    public EntryPropertiesRequestDto FileProperties { get; set; }
}
