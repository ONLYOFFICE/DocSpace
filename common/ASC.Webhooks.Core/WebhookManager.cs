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

namespace ASC.Webhooks.Core;

public static class WebhookManager
{
    public static readonly IReadOnlyList<string> MethodList = new List<string>
    {
        "POST",
        "UPDATE",
        "DELETE"
    };

    private static readonly List<Webhook> _webhooks = new List<Webhook>();

    public static void Register(IEnumerable<Webhook> routes)
    {
        _webhooks.AddRange(routes);
    }

    public static bool Contains(string method, string route)
    {
        return _webhooks.Any(r => r.Key == Webhook.GetKey(method, route));
    }

    public static IReadOnlyList<Webhook> GetAll()
    {
        return _webhooks;
    }
}

public class Webhook
{
    public string Key { get => GetKey(Method, Route); }
    public string Name { get => WebHookResource.ResourceManager.GetString(Key) ?? ""; }
    public string Description { get => WebHookResource.ResourceManager.GetString($"{Key}_Description") ?? ""; }
    public string Route { get; set; }
    public string Method { get; set; }
    public bool Disabled { get; set; }

    public static string GetKey(string method, string route) => $"{method}|{route}";
}

public class WebHookDisabledKeysSettings : ISettings<WebHookDisabledKeysSettings>
{
    public List<string> Keys { get; set; }

    [JsonIgnore]
    public Guid ID => new Guid("3B6BA277-EA3C-4B7B-AD67-B2166B15A87C");

    public WebHookDisabledKeysSettings GetDefault()
    {
        return new WebHookDisabledKeysSettings()
        {
            Keys = new List<string> { }
        };
    }
}