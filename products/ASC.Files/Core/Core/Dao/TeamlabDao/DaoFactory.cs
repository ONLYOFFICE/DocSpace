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
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.htm
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

namespace ASC.Files.Core.Data;

[Scope]
public class DaoFactory : IDaoFactory
{
    private readonly IServiceProvider _serviceProvider;
    public IProviderDao ProviderDao { get; }

    public DaoFactory(IServiceProvider serviceProvider, IProviderDao providerDao)
    {
        _serviceProvider = serviceProvider;
        ProviderDao = providerDao;
    }

    public IFileDao<T> GetFileDao<T>()
    {
        return _serviceProvider.GetService<IFileDao<T>>();
    }

    public IFolderDao<T> GetFolderDao<T>()
    {
        return _serviceProvider.GetService<IFolderDao<T>>();
    }

    public ITagDao<T> GetTagDao<T>()
    {
        return _serviceProvider.GetService<ITagDao<T>>();
    }

    public ISecurityDao<T> GetSecurityDao<T>()
    {
        return _serviceProvider.GetService<ISecurityDao<T>>();
    }

    public ILinkDao GetLinkDao()
    {
        return _serviceProvider.GetService<ILinkDao>();
    }
}

public static class DaoFactoryExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<TenantDateTimeConverter>();
        services.TryAdd<FilesMappingAction>();

        services.TryAdd<File<int>>();
        services.TryAdd<IFileDao<int>, FileDao>();

        services.TryAdd<File<string>>();
        services.TryAdd<IFileDao<string>, ProviderFileDao>();

        services.TryAdd<Folder<int>>();
        services.TryAdd<IFolderDao<int>, FolderDao>();

        services.TryAdd<Folder<string>>();
        services.TryAdd<IFolderDao<string>, ProviderFolderDao>();

        services.TryAdd<ISecurityDao<int>, SecurityDao>();
        services.TryAdd<ISecurityDao<string>, ThirdPartySecurityDao>();

        services.TryAdd<ITagDao<int>, TagDao>();
        services.TryAdd<ITagDao<string>, ThirdPartyTagDao>();

        services.TryAdd<ILinkDao, LinkDao>();

        services.TryAdd<EditHistory>();
    }
}
