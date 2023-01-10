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

namespace ASC.Data.Storage.Configuration;

public static class StorageConfigExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAddSingleton(r =>
        {
            var config = new ConfigurationExtension(r.GetService<IConfiguration>());
            return config.GetSetting<Storage>($"{RegionSettings.Current}storage");
        });
    }
}


public class Storage
{
    public IEnumerable<Appender> Appender { get; set; }
    public IEnumerable<Handler> Handler { get; set; }
    public IEnumerable<Module> Module { get; set; }

    public Module GetModuleElement(string name)
    {
        return Module?.FirstOrDefault(r => r.Name == name);
    }
    public Handler GetHandler(string name)
    {
        return Handler?.FirstOrDefault(r => r.Name == name);
    }
}

public class Appender
{
    public string Name { get; set; }
    public string Append { get; set; }
    public string AppendSecure { get; set; }
    public string Extensions { get; set; }
}

public class Handler
{
    public string Name { get; set; }
    public string Type { get; set; }
    public IEnumerable<Properties> Property { get; set; }

    public IDictionary<string, string> GetProperties()
    {
        return Property == null || !Property.Any() ? new Dictionary<string, string>()
            : Property.ToDictionary(r => r.Name, r => r.Value);
    }
}
public class Properties
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class Module
{
    public string Name { get; set; }
    public string Data { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public ACL Acl { get; set; } = ACL.Read;
    public string VirtualPath { get; set; }
    public TimeSpan Expires { get; set; }
    public bool Visible { get; set; } = true;
    public bool AppendTenantId { get; set; } = true;
    public bool Public { get; set; }
    public bool DisableMigrate { get; set; }
    public bool Count { get; set; } = true;
    public bool DisabledEncryption { get; set; }
    public IEnumerable<Module> Domain { get; set; }
    public bool Cache { get; set; }
}

