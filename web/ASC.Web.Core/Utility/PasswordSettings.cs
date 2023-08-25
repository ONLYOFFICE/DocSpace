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

namespace ASC.Web.Core.Utility;

/// <summary>
/// </summary>
[Serializable]
public sealed class PasswordSettings : ISettings<PasswordSettings>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("aa93a4d1-012d-4ccd-895a-e094e809c840"); }
    }

    public const int MaxLength = 30;
    private readonly IConfiguration _configuration;

    private bool? _printableASCII;

    private bool PrintableASCII
    {
        get
        {
            if (_printableASCII == null)
            {
                _printableASCII = true;

                if (bool.TryParse(_configuration["web:password:ascii:cut"], out var cut))
                {
                    _printableASCII = !cut;
                }
            }

            return _printableASCII.Value;
        }
    }

    /// <summary>Minimum password length</summary>
    /// <type>System.Int32, System</type>
    public int MinLength { get; set; }

    /// <summary>Allowed characters for the password in the regex string format</summary>
    /// <type>System.String, System</type>
    public string AllowedCharactersRegexStr
    {
        get
        {
            return PrintableASCII ? @"[\x21-\x7E]" : @"[0-9a-zA-Z!""#$%&()*+,.:;<>?@^_{}~]"; // excluding SPACE or (SPACE and '-/=[\]`|)
        }
    }

    /// <summary>Specifies if the password must include the uppercase letters or not</summary>
    /// <type>System.Boolean, System</type>
    public bool UpperCase { get; set; }

    /// <summary>Specifies if the password must include the digits or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Digits { get; set; }

    /// <summary>Allowed digits for the password in the regex string format</summary>
    /// <type>System.String, System</type>
    public string DigitsRegexStr
    {
        get
        {
            return @"(?=.*\d)";
        }
    }

    /// <summary>Allowed uppercase letters for the password in the regex string format</summary>
    /// <type>System.String, System</type>
    public string UpperCaseRegexStr
    {
        get
        {
            return @"(?=.*[A-Z])";
        }
    }

    /// <summary>Specifies if the password must include the special symbols or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SpecSymbols { get; set; }

    /// <summary>Allowed special symbols for the password in the regex string format</summary>
    /// <type>System.String, System</type>
    public string SpecSymbolsRegexStr
    {
        get
        {
            return PrintableASCII ? @"(?=.*[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E])" : @"(?=.*[!""#$%&()*+,.:;<>?@^_{}~])";
        }
    }
    public PasswordSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PasswordSettings()
    {
    }

    public PasswordSettings GetDefault()
    {
        var def = new PasswordSettings(_configuration) { MinLength = 8, UpperCase = false, Digits = false, SpecSymbols = false };

        if (_configuration != null && int.TryParse(_configuration["web:password:min"], out var defaultMinLength))
        {
            def.MinLength = Math.Max(1, Math.Min(MaxLength, defaultMinLength));
        }

        return def;
    }
}
