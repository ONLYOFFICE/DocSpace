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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Files.Core;
using ASC.Files.Core.Security;

namespace ASC.Web.Files.Api
{
    [Scope]
    public class FilesIntegration
    {
        private static readonly IDictionary<string, IFileSecurityProvider> providers = new Dictionary<string, IFileSecurityProvider>();

        public IDaoFactory DaoFactory { get; }

        public FilesIntegration(IDaoFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public Task<T> RegisterBunchAsync<T>(string module, string bunch, string data)
        {
            var folderDao = DaoFactory.GetFolderDao<T>();
            return folderDao.GetFolderIDAsync(module, bunch, data, true);
        }

        public Task<IEnumerable<T>> RegisterBunchFoldersAsync<T>(string module, string bunch, IEnumerable<string> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            data = data.ToList();
            if (!data.Any())
            {
                return Task.FromResult<IEnumerable<T>>(new List<T>());
            }

            var folderDao = DaoFactory.GetFolderDao<T>();
            return folderDao.GetFolderIDsAsync(module, bunch, data, true);
        }

        public bool IsRegisteredFileSecurityProvider(string module, string bunch)
        {
            lock (providers)
            {
                return providers.ContainsKey(module + bunch);
            }

        }

        public void RegisterFileSecurityProvider(string module, string bunch, IFileSecurityProvider securityProvider)
        {
            lock (providers)
            {
                providers[module + bunch] = securityProvider;
            }
        }

        internal static IFileSecurity GetFileSecurity(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var parts = path.Split('/');
            if (parts.Length < 3) return null;

            IFileSecurityProvider provider;
            lock (providers)
            {
                providers.TryGetValue(parts[0] + parts[1], out provider);
            }
            return provider?.GetFileSecurity(parts[2]);
        }

        internal static Dictionary<object, IFileSecurity> GetFileSecurity(Dictionary<string, string> paths)
        {
            var result = new Dictionary<object, IFileSecurity>();
            var gropped = paths.GroupBy(r =>
            {
                var parts = r.Value.Split('/');
                if (parts.Length < 3) return "";

                return parts[0] + parts[1];
            }, v =>
            {
                var parts = v.Value.Split('/');
                if (parts.Length < 3) return new KeyValuePair<string, string>(v.Key, "");

                return new KeyValuePair<string, string>(v.Key, parts[2]);
            });

            foreach (var grouping in gropped)
            {
                IFileSecurityProvider provider;
                lock (providers)
                {
                    providers.TryGetValue(grouping.Key, out provider);
                }
                if (provider == null) continue;

                var data = provider.GetFileSecurity(grouping.ToDictionary(r => r.Key, r => r.Value));

                foreach(var d in data)
                {
                    result.Add(d.Key, d.Value);
                }
            }

            return result;
        }
    }
}