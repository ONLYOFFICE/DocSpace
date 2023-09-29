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
public class FileShareDto
{
    public FileShareDto() { }

    /// <summary>Sharing rights</summary>
    /// <type>ASC.Files.Core.Security.FileShare, ASC.Files.Core</type>
    public FileShare Access { get; set; }

    /// <summary>A user who has the access to the specified file</summary>
    /// <type>System.Object, System</type>
    public object SharedTo { get; set; }

    /// <summary>Specifies if the file is locked by this user or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsLocked { get; set; }

    /// <summary>Specifies if this user is an owner of the specified file or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsOwner { get; set; }

    /// <summary>Spceifies if this user can edit the access to the specified file or not</summary>
    /// <type>System.Boolean, System</type>
    public bool CanEditAccess { get; set; }

    public static FileShareDto GetSample()
    {
        return new FileShareDto
        {
            Access = FileShare.ReadWrite,
            IsLocked = false,
            IsOwner = true,
            //SharedTo = EmployeeWraper.GetSample()
        };
    }
}

public class FileShareLink
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ShareLink { get; set; }
    public ApiDateTime ExpirationDate { get; set; }
    public LinkType LinkType { get; set; }
    public string Password { get; set; }
    public bool? Disabled { get; set; }
    public bool? DenyDownload { get; set; }
    public bool? IsExpired { get; set; }
}

public enum LinkType
{
    Invitation,
    External
}

[Scope]
public class FileShareDtoHelper
{
    private readonly UserManager _userManager;
    private readonly EmployeeFullDtoHelper _employeeWraperFullHelper;
    private readonly ApiDateTimeHelper _apiDateTimeHelper;

    public FileShareDtoHelper(
        UserManager userManager,
        EmployeeFullDtoHelper employeeWraperFullHelper,
        ApiDateTimeHelper apiDateTimeHelper)
    {
        _userManager = userManager;
        _employeeWraperFullHelper = employeeWraperFullHelper;
        _apiDateTimeHelper = apiDateTimeHelper;
    }

    public async Task<FileShareDto> Get(AceWrapper aceWrapper)
    {
        var result = new FileShareDto
        {
            IsOwner = aceWrapper.Owner,
            IsLocked = aceWrapper.LockedRights,
            CanEditAccess = aceWrapper.CanEditAccess,
        };

        if (aceWrapper.SubjectGroup)
        {
            if (!string.IsNullOrEmpty(aceWrapper.Link))
            {
                var date = aceWrapper.FileShareOptions?.ExpirationDate;
                var expired = aceWrapper.FileShareOptions?.IsExpired;

                result.SharedTo = new FileShareLink
                {
                    Id = aceWrapper.Id,
                    Title = aceWrapper.FileShareOptions?.Title,
                    ShareLink = aceWrapper.Link,
                    ExpirationDate = date.HasValue && date.Value != default ? _apiDateTimeHelper.Get(date) : null,
                    Password = aceWrapper.FileShareOptions?.Password,
                    Disabled = aceWrapper.FileShareOptions?.Disabled is true ? true : expired,
                    DenyDownload = aceWrapper.FileShareOptions?.DenyDownload,
                    LinkType = aceWrapper.SubjectType switch
                    {
                        SubjectType.InvitationLink => LinkType.Invitation,
                        SubjectType.ExternalLink => LinkType.External,
                        _ => LinkType.Invitation
                    },
                    IsExpired = expired
                };
            }
            else
            {
                //Shared to group
                result.SharedTo = new GroupSummaryDto(await _userManager.GetGroupInfoAsync(aceWrapper.Id), _userManager);
            }
        }
        else
        {
            result.SharedTo = await _employeeWraperFullHelper.GetFullAsync(await _userManager.GetUsersAsync(aceWrapper.Id));
        }

        result.Access = aceWrapper.Access;

        return result;
    }
}
