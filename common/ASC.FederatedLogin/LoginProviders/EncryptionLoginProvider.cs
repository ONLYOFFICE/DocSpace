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

namespace ASC.Web.Studio.Core;

[Scope]
public class EncryptionLoginProvider
{
    private readonly ILogger<EncryptionLoginProvider> _logger;
    private readonly SecurityContext _securityContext;
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly AccountLinker _accountLinker;

    public EncryptionLoginProvider(
        ILogger<EncryptionLoginProvider> logger,
        SecurityContext securityContext,
        Signature signature,
        InstanceCrypto instanceCrypto,
        AccountLinker accountLinker)
    {
        _logger = logger;
        _securityContext = securityContext;
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _accountLinker = accountLinker;
    }


    public async Task SetKeysAsync(Guid userId, string keys)
    {
        if (string.IsNullOrEmpty(keys))
        {
            return;
        }

        var loginProfile = new LoginProfile(_signature, _instanceCrypto)
        {
            Provider = ProviderConstants.Encryption,
            Name = _instanceCrypto.Encrypt(keys)
        };

        await _accountLinker.AddLinkAsync(userId.ToString(), loginProfile);
    }

    public async Task<string> GetKeysAsync()
    {
        return await GetKeysAsync(_securityContext.CurrentAccount.ID);
    }

    public async Task<string> GetKeysAsync(Guid userId)
    {
        var profile = (await _accountLinker.GetLinkedProfilesAsync(userId.ToString(), ProviderConstants.Encryption)).FirstOrDefault();
        if (profile == null)
        {
            return null;
        }

        try
        {
            return _instanceCrypto.Decrypt(profile.Name);
        }
        catch (Exception ex)
        {
            var message = string.Format("Can not decrypt {0} keys for {1}", ProviderConstants.Encryption, userId);
            _logger.ErrorWithException(message, ex);
            return null;
        }
    }

    public async Task<IDictionary<Guid, string>> GetKeysAsync(IEnumerable<Guid> usrsIds)
    {
        var profiles = await _accountLinker.GetLinkedProfilesAsync(usrsIds.Select(id => id.ToString()), ProviderConstants.Encryption);
        var keys = new Dictionary<Guid, string>(profiles.Count);

        foreach (var profilePair in profiles)
        {
            var userId = new Guid(profilePair.Key);

            try
            {
                var key = _instanceCrypto.Decrypt(profilePair.Value.Name);
                keys.Add(new Guid(profilePair.Key), key);
            }
            catch (Exception ex)
            {
                var message = string.Format("Can not decrypt {0} keys for {1}", ProviderConstants.Encryption, userId);
                _logger.ErrorWithException(message, ex);
            }
        }

        return keys;
    }
}
