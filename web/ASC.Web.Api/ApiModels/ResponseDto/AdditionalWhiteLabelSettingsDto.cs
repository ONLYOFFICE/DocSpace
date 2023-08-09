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

namespace ASC.Web.Api.ApiModels.ResponseDto;

/// <summary>
/// </summary>
public class AdditionalWhiteLabelSettingsDto: IMapFrom<AdditionalWhiteLabelSettings>
{
    /// <summary>Specifies if the start document is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool StartDocsEnabled { get; set; }

    /// <summary>Specifies if the help center is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool HelpCenterEnabled { get; set; }

    /// <summary>Specifies if feedback and support are available or not</summary>
    /// <type>System.Boolean, System</type>
    public bool FeedbackAndSupportEnabled { get; set; }

    /// <summary>Feedback and support URL</summary>
    /// <type>System.String, System</type>
    public string FeedbackAndSupportUrl { get; set; }

    /// <summary>Specifies if the user forum is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool UserForumEnabled { get; set; }

    /// <summary>User forum URL</summary>
    /// <type>System.String, System</type>
    public string UserForumUrl { get; set; }

    /// <summary>Specifies if the video guides are enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool VideoGuidesEnabled { get; set; }

    /// <summary>Video guides URL</summary>
    /// <type>System.String, System</type>
    public string VideoGuidesUrl { get; set; }

    /// <summary>Sales email</summary>
    /// <type>System.String, System</type>
    public string SalesEmail { get; set; }

    /// <summary>URL to pay for the portal</summary>
    /// <type>System.String, System</type>
    public string BuyUrl { get; set; }

    /// <summary>Specifies if the license agreements are enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool LicenseAgreementsEnabled { get; set; }

    /// <summary>Specifies if these settings are default or not</summary>
    /// <type>System.Boolean, System</type>
    public bool IsDefault { get; set; }

    /// <summary>License agreements URL</summary>
    /// <type>System.String, System</type>
    public string LicenseAgreementsUrl { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AdditionalWhiteLabelSettings, AdditionalWhiteLabelSettingsDto>()
            .ConvertUsing<AdditionalWhiteLabelSettingsConverter>();
    }
}
