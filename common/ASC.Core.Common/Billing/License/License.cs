/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Core.Billing;

[Serializable]
[DebuggerDisplay("{DueDate}")]
public class License
{
    public string OriginalLicense { get; set; }

    [JsonPropertyName("affiliate_id")]
    public string AffiliateId { get; set; }

    //[Obsolete]
    public bool WhiteLabel { get; set; }
    public bool Customization { get; set; }
    public bool Branding { get; set; }
    public bool SSBranding { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime DueDate { get; set; }

    [JsonPropertyName("portal_count")]
    public int PortalCount { get; set; }
    public bool Trial { get; set; }

    [JsonPropertyName("user_quota")]
    public int ActiveUsers { get; set; }

    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; }
    public string Signature { get; set; }
    public bool? DiscEncryption { get; set; }

    [JsonPropertyName("users_count")]
    public int DSUsersCount { get; set; }

    [JsonPropertyName("users_expire")]
    public int DSUsersExpire { get; set; }

    [JsonPropertyName("connections")]
    public int DSConnections { get; set; }

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
        if (typeToConvert == typeof(int) && reader.TokenType == JsonTokenType.String)
        {
            var i = reader.GetString();
            if (!int.TryParse(i, out var result))
            {
                return 0;
            }

            return result;
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
