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

using System;

namespace ASC.Web.Api.Models;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Title { get; set; }
    public string AvatarSmall { get; set; }
    public string ProfileUrl { get; set; }
    public bool HasAvatar { get; set; }

    public static EmployeeDto GetSample()
    {
        return new EmployeeDto
        {
            Id = Guid.Empty,
            DisplayName = "Mike Zanyatski",
            Title = "Manager",
            AvatarSmall = "url to small avatar",
        };
    }
}

[Scope]
public class EmployeeDtoHelper
{
    protected readonly UserPhotoManager _userPhotoManager;
    protected readonly UserManager _userManager;

    private readonly ApiContext _httpContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly Dictionary<Guid, EmployeeDto> _dictionary;

    public EmployeeDtoHelper(
        ApiContext httpContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        UserPhotoManager userPhotoManager,
        CommonLinkUtility commonLinkUtility,
        UserManager userManager)
    {
        _userPhotoManager = userPhotoManager;
        _userManager = userManager;
        _httpContext = httpContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _commonLinkUtility = commonLinkUtility;
        _dictionary = new Dictionary<Guid, EmployeeDto>();
    }

    public async Task<EmployeeDto> GetAsync(UserInfo userInfo)
    {
        if (_dictionary.ContainsKey(userInfo.Id))
        {
            return _dictionary[userInfo.Id];
        }
        var employee = await InitAsync(new EmployeeDto(), userInfo);
        _dictionary.Add(userInfo.Id, employee);

        return employee;
    }

    public async Task<EmployeeDto> GetAsync(Guid userId)
    {
        try
        {
            return await GetAsync(await _userManager.GetUsersAsync(userId));
        }
        catch (Exception)
        {
            return await GetAsync(ASC.Core.Users.Constants.LostUser);
        }
    }

    protected async Task<EmployeeDto> InitAsync(EmployeeDto result, UserInfo userInfo)
    {
        result.Id = userInfo.Id;
        result.DisplayName = _displayUserSettingsHelper.GetFullUserName(userInfo);
        result.HasAvatar = await _userPhotoManager.UserHasAvatar(userInfo.Id);

        if (!string.IsNullOrEmpty(userInfo.Title))
        {
            result.Title = userInfo.Title;
        }

        var cacheKey = Math.Abs(userInfo.LastModified.GetHashCode());

        if (_httpContext.Check("avatarSmall"))
        {
            result.AvatarSmall = await _userPhotoManager.GetSmallPhotoURL(userInfo.Id) + $"?hash={cacheKey}";
        }

        if (result.Id != Guid.Empty)
        {
            var profileUrl = _commonLinkUtility.GetUserProfile(userInfo, false);
            result.ProfileUrl = _commonLinkUtility.GetFullAbsolutePath(profileUrl);
        }

        return result;
    }
}