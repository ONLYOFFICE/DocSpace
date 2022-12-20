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

namespace ASC.Files.Thirdparty.Sharpbox;

[Transient]
internal class SharpBoxProviderInfo : IProviderInfo
{
    public int ID { get; set; }
    public Guid Owner { get; set; }
    private readonly ILogger<SharpBoxProviderInfo> _logger;

    private nSupportedCloudConfigurations _providerKey;
    public AuthData AuthData { get; set; }

    public SharpBoxProviderInfo(SharpBoxStorageDisposableWrapper storageDisposableWrapper, ILogger<SharpBoxProviderInfo> logger)
    {
        _wrapper = storageDisposableWrapper;
        _logger = logger;
    }

    public void Dispose()
    {
        if (StorageOpened)
        {
            Storage.Close();
        }
    }

    internal CloudStorage Storage
    {
        get
        {
            if (_wrapper.Storage == null || !_wrapper.Storage.IsOpened)
            {
                return _wrapper.CreateStorage(AuthData, _providerKey);
            }

            return _wrapper.Storage;
        }
    }

    internal bool StorageOpened => _wrapper.Storage != null && _wrapper.Storage.IsOpened;

    public string CustomerTitle { get; set; }
    public DateTime CreateOn { get; set; }
    public string RootFolderId => "sbox-" + ID;

    public void UpdateTitle(string newtitle)
    {
        CustomerTitle = newtitle;
    }

    public Task<bool> CheckAccessAsync()
    {
        try
        {
            return Task.FromResult(Storage.GetRoot() != null);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(false);
        }
        catch (SharpBoxException ex)
        {
            _logger.ErrorSharpboxCheckAccess(ex);

            return Task.FromResult(false);
        }
    }

    public Task InvalidateStorageAsync()
    {
        if (_wrapper != null)
        {
            _wrapper.Dispose();
        }

        return Task.CompletedTask;
    }

    public string ProviderKey
    {
        get => _providerKey.ToString();
        set => _providerKey = (nSupportedCloudConfigurations)Enum.Parse(typeof(nSupportedCloudConfigurations), value, true);
    }

    public FolderType RootFolderType { get; set; }
    public FolderType FolderType { get; set; }
    public string FolderId { get; set; }
    public bool Private { get; set; }
    public bool HasLogo { get; set; }
    private readonly SharpBoxStorageDisposableWrapper _wrapper;
}

[Scope]
class SharpBoxStorageDisposableWrapper : IDisposable
{
    public CloudStorage Storage { get; private set; }

    public SharpBoxStorageDisposableWrapper() { }

    internal CloudStorage CreateStorage(AuthData _authData, nSupportedCloudConfigurations _providerKey)
    {
        var prms = Array.Empty<object>();
        if (!string.IsNullOrEmpty(_authData.Url))
        {
            var uri = _authData.Url;
            if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri;
            }

            prms = new object[] { new Uri(uri) };
        }

        var storage = new CloudStorage();
        var config = CloudStorage.GetCloudConfigurationEasy(_providerKey, prms);
        if (!string.IsNullOrEmpty(_authData.Token))
        {
            if (_providerKey != nSupportedCloudConfigurations.BoxNet)
            {
                var token = storage.DeserializeSecurityTokenFromBase64(_authData.Token);
                storage.Open(config, token);
            }
        }
        else
        {
            storage.Open(config, new GenericNetworkCredentials { Password = _authData.Password, UserName = _authData.Login });
        }

        return Storage = storage;
    }

    public void Dispose()
    {
        if (Storage != null && Storage.IsOpened)
        {
            Storage.Close();
            Storage = null;
        }
    }
}
