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

namespace ASC.Web.Core.Users;

[Serializable]
public class DisplayUserSettings : ISettings<DisplayUserSettings>
{
    [JsonIgnore]
    public Guid ID => new Guid("2EF59652-E1A7-4814-BF71-FEB990149428");

    public bool IsDisableGettingStarted { get; set; }

    public DisplayUserSettings GetDefault()
    {
        return new DisplayUserSettings
        {
            IsDisableGettingStarted = false,
        };
    }
}

[Scope]
public class DisplayUserSettingsHelper
{
    private readonly string _removedProfileName;
    public DisplayUserSettingsHelper(UserManager userManager, UserFormatter userFormatter, IConfiguration configuration)
    {
        _userManager = userManager;
        _userFormatter = userFormatter;
        _removedProfileName = configuration["web:removed-profile-name"] ?? "profile removed";
    }

    private readonly UserManager _userManager;
    private readonly UserFormatter _userFormatter;

    public async Task<string> GetFullUserNameAsync(Guid userID, bool withHtmlEncode = true)
    {
        return GetFullUserName(await _userManager.GetUsersAsync(userID), withHtmlEncode);
    }

    public string GetFullUserName(UserInfo userInfo, bool withHtmlEncode = true)
    {
        return GetFullUserName(userInfo, DisplayUserNameFormat.Default, withHtmlEncode);
    }

    public string GetFullUserName(UserInfo userInfo, DisplayUserNameFormat format, bool withHtmlEncode)
    {
        if (userInfo == null)
        {
            return string.Empty;
        }
        if (!userInfo.Id.Equals(Guid.Empty) && !_userManager.UserExists(userInfo))
        {
            try
            {
                var resourceType = Type.GetType("ASC.Web.Core.PublicResources.Resource, ASC.Web.Core");
                var resourceProperty = resourceType.GetProperty("ProfileRemoved", BindingFlags.Static | BindingFlags.Public);

                var resourceValue = "";

                if (resourceProperty != null)
                {
                    resourceValue = (string)resourceProperty.GetValue(null);
                }

                return string.IsNullOrEmpty(resourceValue) ? _removedProfileName : resourceValue;
            }
            catch (Exception)
            {
                return _removedProfileName;
            }
        }
        var result = _userFormatter.GetUserName(userInfo, format);

        if (string.IsNullOrWhiteSpace(result))
        {
            result = userInfo.Email;
        }

        return withHtmlEncode ? HtmlEncode(result) : result;
    }
    public string HtmlEncode(string str)
    {
        return !string.IsNullOrEmpty(str) ? HttpUtility.HtmlEncode(str) : str;
    }
}
