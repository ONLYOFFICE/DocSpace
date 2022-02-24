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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Thirdparty
{
    internal abstract class RegexDaoSelectorBase<T> : IDaoSelector<T> where T : class, IProviderInfo
    {
        private IServiceProvider ServiceProvider { get; }
        private IDaoFactory DaoFactory { get; }
        protected internal abstract string Name { get; }
        protected internal abstract string Id { get; }
        public Regex Selector { get => selector ??= new Regex(@"^" + Id + @"-(?'id'\d+)(-(?'path'.*)){0,1}$", RegexOptions.Singleline | RegexOptions.Compiled); }
        private Regex selector;

        private Dictionary<string, ThirdPartyProviderDao<T>> Providers { get; set; }

        protected RegexDaoSelectorBase(
            IServiceProvider serviceProvider,
            IDaoFactory daoFactory)
        {
            ServiceProvider = serviceProvider;
            DaoFactory = daoFactory;
            Providers = new Dictionary<string, ThirdPartyProviderDao<T>>();
        }

        public virtual string ConvertId(string id)
        {
            try
            {
                if (id == null) return null;

                var match = Selector.Match(id);
                if (match.Success)
                {
                    return match.Groups["path"].Value.Replace('|', '/');
                }
                throw new ArgumentException($"Id is not a {Name} id");
            }
            catch (Exception fe)
            {
                throw new FormatException("Can not convert id: " + id, fe);
            }
        }

        public string GetIdCode(string id)
        {
            if (id != null)
            {
                var match = Selector.Match(id);
                if (match.Success)
                {
                    return match.Groups["id"].Value;
                }
            }
            throw new ArgumentException($"Id is not a {Name} id");
        }

        public virtual bool IsMatch(string id)
        {
            return id != null && Selector.IsMatch(id);
        }

        public virtual ISecurityDao<string> GetSecurityDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ISecurityDao<string>
        {
            return GetDao<T1>(id);
        }

        public virtual IFileDao<string> GetFileDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFileDao<string>
        {
            return GetDao<T1>(id);
        }

        public virtual ITagDao<string> GetTagDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ITagDao<string>
        {
            return GetDao<T1>(id);
        }

        public virtual IFolderDao<string> GetFolderDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFolderDao<string>
        {
            return GetDao<T1>(id);
        }

        private T1 GetDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>
        {
            var providerKey = $"{id}{typeof(T1)}";
            if (Providers.TryGetValue(providerKey, out var provider)) return (T1)provider;

            var res = ServiceProvider.GetService<T1>();

            res.Init(GetInfo(id), this);

            Providers.Add(providerKey, res);

            return res;
        }


        internal BaseProviderInfo<T> GetInfo(string objectId)
        {
            if (objectId == null) throw new ArgumentNullException(nameof(objectId));
            var id = objectId;
            var match = Selector.Match(id);
            if (match.Success)
            {
                var providerInfo = GetProviderInfo(Convert.ToInt32(match.Groups["id"].Value));

                return new BaseProviderInfo<T>
                {
                    Path = match.Groups["path"].Value,
                    ProviderInfo = providerInfo,
                    PathPrefix = Id + "-" + match.Groups["id"].Value
                };
            }
            throw new ArgumentException($"Id is not {Name} id");
        }

        public async Task RenameProviderAsync(T provider, string newTitle)
        {
            var dbDao = ServiceProvider.GetService<ProviderAccountDao>();
            await dbDao.UpdateProviderInfoAsync(provider.ID, newTitle, null, provider.RootFolderType);
            provider.UpdateTitle(newTitle); //This will update cached version too
        }

        protected virtual T GetProviderInfo(int linkId)
        {
            var dbDao = DaoFactory.ProviderDao;
            try
            {
                return dbDao.GetProviderInfoAsync(linkId).Result as T;
            }
            catch (InvalidOperationException)
            {
                throw new ProviderInfoArgumentException("Provider id not found or you have no access");
            }
        }

        public void Dispose()
        {
            foreach (var p in Providers)
            {
                p.Value.Dispose();
            }
        }
    }
}