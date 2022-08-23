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

namespace ASC.Files.Thirdparty;

internal abstract class RegexDaoSelectorBase<T> : IDaoSelector<T> where T : class, IProviderInfo
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDaoFactory _daoFactory;
    protected internal abstract string Name { get; }
    protected internal abstract string Id { get; }
    public Regex Selector => _selector ??= new Regex(@"^" + Id + @"-(?'id'\d+)(-(?'path'.*)){0,1}$", RegexOptions.Singleline | RegexOptions.Compiled);
    private Regex _selector;

    private Dictionary<string, ThirdPartyProviderDao<T>> Providers { get; set; }

    protected RegexDaoSelectorBase(
        IServiceProvider serviceProvider,
        IDaoFactory daoFactory)
    {
        _serviceProvider = serviceProvider;
        _daoFactory = daoFactory;
        Providers = new Dictionary<string, ThirdPartyProviderDao<T>>();
    }

    public virtual string ConvertId(string id)
    {
        try
        {
            if (id == null)
            {
                return null;
            }

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
        if (Providers.TryGetValue(providerKey, out var provider))
        {
            return (T1)provider;
        }

        var res = _serviceProvider.GetService<T1>();

        res.Init(GetInfo(id), this);

        Providers.Add(providerKey, res);

        return res;
    }

    internal BaseProviderInfo<T> GetInfo(string objectId)
    {
        ArgumentNullException.ThrowIfNull(objectId);

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
        var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
        await dbDao.UpdateProviderInfoAsync(provider.ID, newTitle, null, provider.RootFolderType);
        provider.UpdateTitle(newTitle); //This will update cached version too
    }

    public async Task UpdateProviderFolderId(T provider, string id)
    {
        var dbDao = _serviceProvider.GetService<ProviderAccountDao>();
        await dbDao.UpdateProviderInfoAsync(provider.ID, id, provider.FolderType);
        provider.FolderId = id;
    }

    protected virtual T GetProviderInfo(int linkId)
    {
        var dbDao = _daoFactory.ProviderDao;
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
