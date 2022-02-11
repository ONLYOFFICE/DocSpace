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
using System.Threading.Tasks;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Files.Core;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.Sharpbox
{
    [Transient]
    internal class SharpBoxProviderInfo : IProviderInfo
    {
        public int ID { get; set; }
        public Guid Owner { get; set; }
        public ILog Log { get; private set; }

        private nSupportedCloudConfigurations _providerKey;
        public AuthData AuthData { get; set; }

        public SharpBoxProviderInfo(SharpBoxStorageDisposableWrapper storageDisposableWrapper, IOptionsMonitor<ILog> monitor)
        {
            Wrapper = storageDisposableWrapper;
            Log = monitor.CurrentValue;
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
                if (Wrapper.Storage == null || !Wrapper.Storage.IsOpened)
                {
                    return Wrapper.CreateStorage(AuthData, _providerKey);
                }
                return Wrapper.Storage;
            }
        }

        internal bool StorageOpened
        {
            get => Wrapper.Storage != null && Wrapper.Storage.IsOpened;
        }

        public void UpdateTitle(string newtitle)
        {
            CustomerTitle = newtitle;
        }

        public string CustomerTitle { get; set; }

        public DateTime CreateOn { get; set; }

        public string RootFolderId
        {
            get { return "sbox-" + ID; }
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
                Log.Error("Sharpbox CheckAccess error", ex);
                return Task.FromResult(false);
            }
        }

        public Task InvalidateStorageAsync()
        {
            if (Wrapper != null)
            {
                Wrapper.Dispose();
            }
            return Task.CompletedTask;
        }

        public string ProviderKey
        {
            get { return _providerKey.ToString(); }
            set { _providerKey = (nSupportedCloudConfigurations)Enum.Parse(typeof(nSupportedCloudConfigurations), value, true); }
        }

        public FolderType RootFolderType { get; set; }
        private SharpBoxStorageDisposableWrapper Wrapper { get; set; }
    }

    [Scope]
    class SharpBoxStorageDisposableWrapper : IDisposable
    {
        public CloudStorage Storage { get; private set; }


        public SharpBoxStorageDisposableWrapper()
        {
        }

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
}