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

namespace ASC.Web.Api.ApiModel.ResponseDto;

/// <summary>
/// </summary>
public class SmtpSettingsDto : IMapFrom<SmtpSettings>
{
    /// <summary>Host</summary>
    /// <type>System.String, System</type>
    /// <example>mail.example.com</example>
    public string Host { get; set; }

    /// <summary>Port</summary>
    /// <type>System.Nullable{System.Int32}, System</type>
    /// <example>25</example>
    public int? Port { get; set; }

    /// <summary>Sender address</summary>
    /// <type>System.String, System</type>
    /// <example>notify@example.com</example>
    public string SenderAddress { get; set; }

    /// <summary>Sender display name</summary>
    /// <type>System.String, System</type>
    /// <example>Postman</example>
    public string SenderDisplayName { get; set; }

    /// <summary>Credentials username</summary>
    /// <type>System.String, System</type>
    /// <example>notify@example.com</example>
    public string CredentialsUserName { get; set; }

    /// <summary>Credentials user password</summary>
    /// <type>System.String, System</type>
    /// <example>{password}</example>
    public string CredentialsUserPassword { get; set; }

    /// <summary>Enables SSL or not</summary>
    /// <type>System.Boolean, System</type>
    /// <example>true</example>
    public bool EnableSSL { get; set; }

    /// <summary>Enables authentication or not</summary>
    /// <type>System.Boolean, System</type>
    /// <example>false</example>
    public bool EnableAuth { get; set; }

    /// <summary>Specifies whether to use NTLM or not</summary>
    /// <type>System.Boolean, System</type>
    /// <example>false</example>
    public bool UseNtlm { get; set; }

    /// <summary>Specifies if the current settings are default or not</summary>
    /// <type>System.Boolean, System</type>
    /// <example>false</example>
    public bool IsDefaultSettings { get; set; }

    public static SmtpSettingsDto GetSample()
    {
        return new SmtpSettingsDto
        {
            Host = "mail.example.com",
            Port = 25,
            CredentialsUserName = "notify@example.com",
            CredentialsUserPassword = "{password}",
            EnableAuth = true,
            EnableSSL = false,
            SenderAddress = "notify@example.com",
            SenderDisplayName = "Postman"
        };
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SmtpSettings, SmtpSettingsDto>();
    }
}