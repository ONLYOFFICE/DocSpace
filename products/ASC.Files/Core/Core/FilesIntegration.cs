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

namespace ASC.Web.Files.Api;

[Scope]
public class FilesIntegration
{
    private static readonly IDictionary<string, IFileSecurityProvider> _providers = new Dictionary<string, IFileSecurityProvider>();
    private readonly IDaoFactory _daoFactory;

    public FilesIntegration(IDaoFactory daoFactory)
    {
        _daoFactory = daoFactory;
    }

    public async Task<T> RegisterBunchAsync<T>(string module, string bunch, string data)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();

        return await folderDao.GetFolderIDAsync(module, bunch, data, true);
    }

    public IAsyncEnumerable<T> RegisterBunchFoldersAsync<T>(string module, string bunch, IEnumerable<string> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        data = data.ToList();
        if (!data.Any())
        {
            return AsyncEnumerable.Empty<T>();
        }

        var folderDao = _daoFactory.GetFolderDao<T>();
        return folderDao.GetFolderIDsAsync(module, bunch, data, true);
    }

    public bool IsRegisteredFileSecurityProvider(string module, string bunch)
    {
        lock (_providers)
        {
            return _providers.ContainsKey(module + bunch);
        }

    }

    public void RegisterFileSecurityProvider(string module, string bunch, IFileSecurityProvider securityProvider)
    {
        lock (_providers)
        {
            _providers[module + bunch] = securityProvider;
        }
    }

    internal static IFileSecurity GetFileSecurity(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var parts = path.Split('/');
        if (parts.Length < 3)
        {
            return null;
        }

        IFileSecurityProvider provider;
        lock (_providers)
        {
            _providers.TryGetValue(parts[0] + parts[1], out provider);
        }

        return provider?.GetFileSecurity(parts[2]);
    }

    internal static Dictionary<object, IFileSecurity> GetFileSecurity(Dictionary<string, string> paths)
    {
        var result = new Dictionary<object, IFileSecurity>();
        var gropped = paths.GroupBy(r =>
        {
            var parts = r.Value.Split('/');
            if (parts.Length < 3)
            {
                return string.Empty;
            }

            return parts[0] + parts[1];
        }, v =>
        {
            var parts = v.Value.Split('/');
            if (parts.Length < 3)
            {
                return new KeyValuePair<string, string>(v.Key, "");
            }

            return new KeyValuePair<string, string>(v.Key, parts[2]);
        });

        foreach (var grouping in gropped)
        {
            IFileSecurityProvider provider;
            lock (_providers)
            {
                _providers.TryGetValue(grouping.Key, out provider);
            }

            if (provider == null)
            {
                continue;
            }

            var data = provider.GetFileSecurity(grouping.ToDictionary(r => r.Key, r => r.Value));

            foreach (var d in data)
            {
                result.Add(d.Key, d.Value);
            }
        }

        return result;
    }
}
