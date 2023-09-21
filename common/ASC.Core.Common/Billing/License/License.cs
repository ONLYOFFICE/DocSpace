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

namespace ASC.Core.Billing;

/// <summary>
/// </summary>
[DebuggerDisplay("{DueDate}")]
public class License
{
    /// <summary>Original license</summary>
    /// <type>System.String, System</type>
    public string OriginalLicense { get; set; }

    /// <summary>Specifies if the license is customizable or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Customization { get; set; }

    /// <summary>License due date</summary>
    /// <type>System.DateTime, System</type>
    [JsonPropertyName("end_date")]
    public DateTime DueDate { get; set; }

    /// <summary>Specifies if the license is trial or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Trial { get; set; }

    /// <summary>Customer ID</summary>
    /// <type>System.String, System</type>
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; }

    /// <summary>Number of document server users</summary>
    /// <type>System.Int32, System</type>
    [JsonPropertyName("users_count")]
    public int DSUsersCount { get; set; }

    /// <summary>Number of users whose licenses have expired</summary>
    /// <type>System.Int32, System</type>
    [JsonPropertyName("users_expire")]
    public int DSUsersExpire { get; set; }

    /// <summary>Number of document server connections</summary>
    /// <type>System.Int32, System</type>
    [JsonPropertyName("connections")]
    public int DSConnections { get; set; }

    /// <summary>License signature</summary>
    /// <type>System.String, System</type>
    [JsonPropertyName("signature")]
    public string Signature { get; set; }

    public static License Parse(string licenseString)
    {
        if (string.IsNullOrEmpty(licenseString))
        {
            throw new BillingNotFoundException("License file is empty");
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new LicenseConverter());

            var license = JsonSerializer.Deserialize<License>(licenseString, options);

            if (license == null)
            {
                throw new BillingNotFoundException("Can't parse license");
            }

            license.OriginalLicense = licenseString;

            return license;
        }
        catch (Exception)
        {
            throw new BillingNotFoundException("Can't parse license");
        }
    }
}

public class LicenseConverter : System.Text.Json.Serialization.JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(int) == typeToConvert ||
               typeof(bool) == typeToConvert;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(int))
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var i = reader.GetString();
                if (!int.TryParse(i, out var result))
                {
                    return 0;
                }
                return result;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }


        }

        if (typeToConvert == typeof(bool))
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var i = reader.GetString();
                if (!bool.TryParse(i, out var result))
                {
                    return false;
                }

                return result;
            }

            return reader.GetBoolean();
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        return;
    }
}
