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

namespace ASC.Files.Core.Helpers;

[Scope]
public class DocumentServiceLicense
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

    private readonly ICache _cache;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly IHttpClientFactory _clientFactory;


    public DocumentServiceLicense(
        ICache cache,
        CoreBaseSettings coreBaseSettings,
        FilesLinkUtility filesLinkUtility,
        FileUtility fileUtility,
        IHttpClientFactory clientFactory)
    {
        _cache = cache;
        _coreBaseSettings = coreBaseSettings;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _clientFactory = clientFactory;
    }

    private async Task<CommandResponse> GetDocumentServiceLicenseAsync()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return null;
        }

        if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceCommandUrl))
        {
            return null;
        }

        var cacheKey = "DocumentServiceLicense";
        var commandResponse = _cache.Get<CommandResponse>(cacheKey);
        if (commandResponse == null)
        {
            commandResponse = await CommandRequestAsync(
                   _fileUtility,
                   _filesLinkUtility.DocServiceCommandUrl,
                   CommandMethod.License,
                   null,
                   null,
                   null,
                   null,
                   _fileUtility.SignatureSecret,
                   _clientFactory
                   );
            _cache.Insert(cacheKey, commandResponse, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return commandResponse;
    }

    public async Task<Dictionary<string, DateTime>> GetLicenseQuotaAsync()
    {
        var commandResponse = await GetDocumentServiceLicenseAsync();
        if (commandResponse == null
            || commandResponse.Quota == null
            || commandResponse.Quota.Users == null)
        {
            return null;
        }

        var result = new Dictionary<string, DateTime>();
        commandResponse.Quota.Users.ForEach(user => result.Add(user.UserId, user.Expire));
        return result;
    }

    public async Task<License> GetLicenseAsync()
    {
        var commandResponse = await GetDocumentServiceLicenseAsync();
        if (commandResponse == null)
        {
            return null;
        }

        return commandResponse.License;
    }
}
