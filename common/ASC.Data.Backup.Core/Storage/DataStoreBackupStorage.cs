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

namespace ASC.Data.Backup.Storage;

[Scope]
public class DataStoreBackupStorage : IBackupStorage
{
    private string _webConfigPath;
    private int _tenant;
    private readonly StorageFactory _storageFactory;

    public DataStoreBackupStorage(StorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    public void Init(int tenant, string webConfigPath)
    {
        _webConfigPath = webConfigPath;
        _tenant = tenant;
    }

    public async Task<string> Upload(string storageBasePath, string localPath, Guid userId)
    {
        using var stream = File.OpenRead(localPath);
        var storagePath = Path.GetFileName(localPath);
        await GetDataStore().SaveAsync("", storagePath, stream);

        return storagePath;
    }

    public async Task Download(string storagePath, string targetLocalPath)
    {
        using var source = await GetDataStore().GetReadStreamAsync("", storagePath);
        using var destination = File.OpenWrite(targetLocalPath);
        source.CopyTo(destination);
    }

    public async Task Delete(string storagePath)
    {
        var dataStore = GetDataStore();
        if (await dataStore.IsFileAsync("", storagePath))
        {
            await dataStore.DeleteAsync("", storagePath);
        }
    }

    public async Task<bool> IsExists(string storagePath)
    {
        return await GetDataStore().IsFileAsync("", storagePath);
    }

    public async Task<string> GetPublicLink(string storagePath)
    {
        return (await GetDataStore().GetPreSignedUriAsync("", storagePath, TimeSpan.FromDays(1), null)).ToString();
    }

    protected virtual IDataStore GetDataStore()
    {
        return _storageFactory.GetStorage(_webConfigPath, _tenant.ToString(), "backup", null);
    }
}
