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

namespace ASC.Web.Studio.Utility;

[Scope]
public class TenantLogoHelper
{
    private readonly TenantLogoManager _tenantLogoManager;
    private readonly SettingsManager _settingsManager;
    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;

    public TenantLogoHelper(
        TenantLogoManager tenantLogoManager,
        SettingsManager settingsManager,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        TenantInfoSettingsHelper tenantInfoSettingsHelper)
    {
        _tenantLogoManager = tenantLogoManager;
        _settingsManager = settingsManager;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
    }

    public async Task<string> GetLogo(WhiteLabelLogoTypeEnum type, bool isDefIfNoWhiteLabel = false, bool dark = false)
    {
        string imgUrl;
        if (_tenantLogoManager.WhiteLabelEnabled)
        {
            var _tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();
            return await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(_tenantWhiteLabelSettings, type, dark);
        }
        else
        {
            if (isDefIfNoWhiteLabel)
            {
                imgUrl = await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(type, dark);
            }
            else
            {
                if (type == WhiteLabelLogoTypeEnum.LoginPage)
                {
                    /*** simple scheme ***/
                    var _tenantInfoSettings = await _settingsManager.LoadAsync<TenantInfoSettings>();
                    imgUrl = await _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPathAsync(_tenantInfoSettings);
                    /***/
                }
                else
                {
                    imgUrl = await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(type, dark);
                }
            }
        }

        return imgUrl;

    }
}
