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

namespace ASC.ApiSystem.Classes;

[Singletone]
public class CommonConstants
{
    public CommonConstants(IConfiguration configuration)
    {
        DefaultCulture = new CultureInfo(DefaultLanguage);

        RecaptchaRequired = Convert.ToBoolean(configuration["recaptcha:required"] ?? "true");

        var appKeys = configuration["web:app:keys"];

        if (!string.IsNullOrEmpty(appKeys))
        {
            AppSecretKeys = appKeys.Split(',', ';')
                          .Select(x => x.Trim().ToLower())
                          .ToList();
        }
        else
        {
            AppSecretKeys = new List<string>();
        }

        AutotestSecretEmails = (configuration["web:autotest:secret-email"] ?? "").Trim();

        MaxAttemptsCount = Convert.ToInt32(configuration["max-attempts-count"] ?? "10");

        MaxAttemptsTimeInterval = TimeSpan.Parse(configuration["max-attempts-interval"] ?? "00:05:00");

        WebApiBaseUrl = configuration["api:url"] ?? "/api/2.0/";

    }

    public const string BaseDbConnKeyString = "core";

    public const string DefaultLanguage = "en-US";

    public CultureInfo DefaultCulture { get; }

    public bool RecaptchaRequired { get; }

    public List<string> AppSecretKeys { get; }

    public string AutotestSecretEmails { get; }

    public int MaxAttemptsCount { get; }

    public TimeSpan MaxAttemptsTimeInterval { get; }

    public string WebApiBaseUrl { get; }
}
