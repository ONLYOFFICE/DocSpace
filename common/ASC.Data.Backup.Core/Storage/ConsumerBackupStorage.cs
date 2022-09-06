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
public class ConsumerBackupStorage : IBackupStorage
{
    private const string Domain = "backup";

    private IDataStore _store;
    private readonly StorageSettingsHelper _storageSettingsHelper;

    public ConsumerBackupStorage(StorageSettingsHelper storageSettingsHelper)
    {
        _storageSettingsHelper = storageSettingsHelper;
    }

    public void Init(IReadOnlyDictionary<string, string> storageParams)
    {
        var settings = new StorageSettings { Module = storageParams["module"], Props = storageParams.Where(r => r.Key != "module").ToDictionary(r => r.Key, r => r.Value) };
        _store = _storageSettingsHelper.DataStore(settings);
    }

    public string Upload(string storageBasePath, string localPath, Guid userId)
    {
        using var stream = File.OpenRead(localPath);
        var storagePath = Path.GetFileName(localPath);
        _store.SaveAsync(Domain, storagePath, stream, ACL.Private).Wait();
        return storagePath;
    }

    public void Download(string storagePath, string targetLocalPath)
    {
        using var source = _store.GetReadStreamAsync(Domain, storagePath).Result;
        using var destination = File.OpenWrite(targetLocalPath);
        source.CopyTo(destination);
    }

    public void Delete(string storagePath)
    {
        if (_store.IsFileAsync(Domain, storagePath).Result)
        {
            _store.DeleteAsync(Domain, storagePath).Wait();
        }
    }

    public bool IsExists(string storagePath)
    {
        if (_store != null)
        {
            return _store.IsFileAsync(Domain, storagePath).Result;
        }
        else
        {
            return false;
        }
    }

    public string GetPublicLink(string storagePath)
    {
        return _store.GetInternalUriAsync(Domain, storagePath, TimeSpan.FromDays(1), null).Result.AbsoluteUri;
    }
}
