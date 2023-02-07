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

namespace ASC.Common.Utils;

public class ConnectionStringCollection : IEnumerable<ConnectionStringSettings>
{
    private readonly List<ConnectionStringSettings> _data;

    public ConnectionStringSettings this[string name] => _data.FirstOrDefault(r => r.Name == name);

    public ConnectionStringCollection(IEnumerable<ConnectionStringSettings> data)
    {
        _data = data.ToList();
    }

    public IEnumerator<ConnectionStringSettings> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Singletone]
public class ConfigurationExtension
{
    public string this[string key]
    {
        get => _configuration[key];
        set => _configuration[key] = value;
    }

    private readonly IConfiguration _configuration;
    private readonly Lazy<ConnectionStringCollection> _connectionStringSettings;

    public ConfigurationExtension(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionStringSettings = new Lazy<ConnectionStringCollection>(new ConnectionStringCollection(GetSettings<ConnectionStringSettings>($"{RegionSettings.Current}ConnectionStrings")));
    }

    public IEnumerable<T> GetSettings<T>(string section) where T : new()
    {
        var result = new List<T>();

        var sectionSettings = _configuration.GetSection(section);

        foreach (var ch in sectionSettings.GetChildren())
        {
            var cs = new T();
            ch.Bind(cs);
            result.Add(cs);
        }

        return result;
    }

    public T GetSetting<T>(string section) where T : new()
    {
        return GetSetting(section, new T());
    }

    public T GetSetting<T>(string section, T instance)
    {
        var sectionSettings = _configuration.GetSection(section);

        sectionSettings.Bind(instance);

        return instance;
    }

    public ConnectionStringCollection GetConnectionStrings()
    {
        return _connectionStringSettings.Value;
    }

    public ConnectionStringSettings GetConnectionStrings(string key, string region = "current")
    {
        if (region == "current")
        {
            return GetConnectionStrings()[key];
        }
        else
        {
            var connectionStrings = new ConnectionStringCollection(GetSettings<ConnectionStringSettings>($"regions:{region}:ConnectionStrings"));
            return connectionStrings[key];
        }
    }
}
