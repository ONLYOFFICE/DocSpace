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

namespace ASC.Files.Api;

public class ThirdpartyController : ApiControllerBase
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly EntryManager _entryManager;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FileStorageService _fileStorageService;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly SecurityContext _securityContext;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;
    private readonly UserManager _userManager;
    private readonly WordpressHelper _wordpressHelper;
    private readonly WordpressToken _wordpressToken;
    private readonly RequestHelper _requestHelper;
    private readonly FileSecurityCommon _fileSecurityCommon;

    public ThirdpartyController(
        CoreBaseSettings coreBaseSettings,
        EntryManager entryManager,
        FilesSettingsHelper filesSettingsHelper,
        FileStorageService fileStorageService,
        GlobalFolderHelper globalFolderHelper,
        SecurityContext securityContext,
        ThirdpartyConfiguration thirdpartyConfiguration,
        UserManager userManager,
        WordpressHelper wordpressHelper,
        WordpressToken wordpressToken,
        RequestHelper requestHelper,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        FileSecurityCommon fileSecurityCommon) : base(folderDtoHelper, fileDtoHelper)
    {
        _coreBaseSettings = coreBaseSettings;
        _entryManager = entryManager;
        _filesSettingsHelper = filesSettingsHelper;
        _fileStorageService = fileStorageService;
        _globalFolderHelper = globalFolderHelper;
        _securityContext = securityContext;
        _thirdpartyConfiguration = thirdpartyConfiguration;
        _userManager = userManager;
        _wordpressHelper = wordpressHelper;
        _wordpressToken = wordpressToken;
        _requestHelper = requestHelper;
        _fileSecurityCommon = fileSecurityCommon;
    }

    /// <summary>
    /// Returns a list of the available providers.
    /// </summary>
    /// <short>Get providers</short>
    /// <category>Third-party integration</category>
    /// <returns type="System.Collections.Generic.List{System.String}, System.Collections.Generic">List of provider keys</returns>
    /// <remarks>Available provider keys: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive, kDrive.</remarks>
    /// <path>api/2.0/files/thirdparty/capabilities</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("thirdparty/capabilities")]
    public async Task<List<List<string>>> CapabilitiesAsync()
    {
        var result = new List<List<string>>();

        if (await _userManager.IsUserAsync(_securityContext.CurrentAccount.ID)
                || (!_filesSettingsHelper.EnableThirdParty
                && !_coreBaseSettings.Personal))
        {
            return result;
        }

        return _thirdpartyConfiguration.GetProviders();
    }

    /// <summary>
    /// Creates a WordPress post with the parameters specified in the request.
    /// </summary>
    /// <short>Create a WordPress post</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.CreateWordpressPostRequestDto, ASC.Files.Core" name="inDto">Request parameters for creating a WordPress post</param>
    /// <category>WordPress</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/wordpress</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("wordpress")]
    public async Task<bool> CreateWordpressPostAsync(CreateWordpressPostRequestDto inDto)
    {
        try
        {
            var token = await _wordpressToken.GetTokenAsync();
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var parser = JObject.Parse(meInfo);
            if (parser == null)
            {
                return false;
            }

            var blogId = parser.Value<string>("token_site_id");

            if (blogId != null)
            {
                var createPost = _wordpressHelper.CreateWordpressPost(inDto.Title, inDto.Content, inDto.Status, blogId, token);

                return createPost;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the third-party storage service account with the ID specified in the request.
    /// </summary>
    /// <param type="System.Int32, System" method="url" name="providerId">Provider ID. It is a part of the folder ID. Example: folder ID is "sbox-123", then provider ID is "123"</param>
    /// <short>Remove a third-party account</short>
    /// <category>Third-party integration</category>
    /// <returns type="System.Object, System">Third-party folder ID</returns>
    /// <path>api/2.0/files/thirdparty/{providerId}</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <exception cref="ArgumentException"></exception>
    [HttpDelete("thirdparty/{providerId:int}")]
    public async Task<object> DeleteThirdPartyAsync(int providerId)
    {
        return await _fileStorageService.DeleteThirdPartyAsync(providerId.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Deletes the WordPress plugin information.
    /// </summary>
    /// <short>Delete the WordPress information</short>
    /// <category>WordPress</category>
    /// <returns type="System.Object, System">Object with the "success" field: true if the operation is successful</returns>
    /// <path>api/2.0/files/wordpress-delete</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("wordpress-delete")]
    public async Task<object> DeleteWordpressInfoAsync()
    {
        var token = await _wordpressToken.GetTokenAsync();
        if (token != null)
        {
            await _wordpressToken.DeleteTokenAsync(token);
            return new
            {
                success = true
            };
        }
        return new
        {
            success = false
        };
    }

    /// <summary>
    /// Returns a list of the third-party services connected to the "Common" section.
    /// </summary>
    /// <category>Third-party integration</category>
    /// <short>Get common third-party services</short>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">List of common third-party folderst</returns>
    /// <path>api/2.0/files/thirdparty/common</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("thirdparty/common")]
    public async IAsyncEnumerable<FolderDto<string>> GetCommonThirdPartyFoldersAsync()
    {
        var parent = await _fileStorageService.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync);
        var thirdpartyFolders = _entryManager.GetThirpartyFoldersAsync(parent);

        await foreach (var r in thirdpartyFolders)
        {
            yield return await _folderDtoHelper.GetAsync(r);
        }
    }

    /// <summary>
    /// Returns a list of all the connected third-party accounts.
    /// </summary>
    /// <category>Third-party integration</category>
    /// <short>Get third-party accounts</short>
    /// <returns type="ASC.Web.Files.Services.WCFService.ThirdPartyParams, ASC.Files.Core">List of connected providers information</returns>
    /// <path>api/2.0/files/thirdparty</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("thirdparty")]
    public IAsyncEnumerable<ThirdPartyParams> GetThirdPartyAccountsAsync()
    {
        return _fileStorageService.GetThirdPartyAsync();
    }

    /// <summary>
    /// Return a backup of the connected third-party account.
    /// </summary>
    /// <category>Third-party integration</category>
    /// <short>Get a third-party account backup</short>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Folder for the third-party account backup</returns>
    /// <path>api/2.0/files/thirdparty/backup</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("thirdparty/backup")]
    public async Task<FolderDto<string>> GetBackupThirdPartyAccountAsync()
    {
        var folder = await _fileStorageService.GetBackupThirdPartyAsync();
        if (folder != null)
        {

            return await _folderDtoHelper.GetAsync(folder);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the WordPress plugin information.
    /// </summary>
    /// <short>Get the WordPress information</short>
    /// <category>WordPress</category>
    /// <returns type="System.Object, System">Object with the following parameters: "success" - specifies if the operation is successful or not, "data" - blog information</returns>
    /// <path>api/2.0/files/wordpress-info</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("wordpress-info")]
    public async Task<object> GetWordpressInfoAsync()
    {
        var token = await _wordpressToken.GetTokenAsync();
        if (token != null)
        {
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");
            var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

            var blogInfo = _requestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
            var jsonBlogInfo = JObject.Parse(blogInfo);
            jsonBlogInfo.Add("username", wordpressUserName);

            blogInfo = jsonBlogInfo.ToString();
            return new
            {
                success = true,
                data = blogInfo
            };
        }
        return new
        {
            success = false
        };
    }

    /// <summary>
    /// Saves the third-party storage service account. For WebDav, Yandex, kDrive and SharePoint, the login and password are used for authentication. For other providers, the authentication is performed using a token received via OAuth 2.0.
    /// </summary>
    /// <short>Save a third-party account</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ThirdPartyRequestDto, ASC.Files.Core" name="inDto">Third-party request parameters</param>
    /// <category>Third-party integration</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Connected provider folder</returns>
    /// <remarks>List of provider keys: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive, kDrive.</remarks>
    /// <path>api/2.0/files/thirdparty</path>
    /// <httpMethod>POST</httpMethod>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("thirdparty")]
    public async Task<FolderDto<string>> SaveThirdPartyAsync(ThirdPartyRequestDto inDto)
    {
        var thirdPartyParams = new ThirdPartyParams
        {
            AuthData = new AuthData(inDto.Url, inDto.Login, inDto.Password, inDto.Token),
            Corporate = inDto.IsRoomsStorage ? false : inDto.IsCorporate,
            RoomsStorage = inDto.IsCorporate ? false : inDto.IsRoomsStorage,
            CustomerTitle = inDto.CustomerTitle,
            ProviderId = inDto.ProviderId,
            ProviderKey = inDto.ProviderKey,
        };

        var folder = await _fileStorageService.SaveThirdPartyAsync(thirdPartyParams);

        return await _folderDtoHelper.GetAsync(folder);
    }

    /// <summary>
    /// Saves a backup of the connected third-party account.
    /// </summary>
    /// <short>Save a third-party account backup</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.ThirdPartyBackupRequestDto, ASC.Files.Core" name="inDto">Third-party backup request parameters</param>
    /// <category>Third-party integration</category>
    /// <returns type="ASC.Files.Core.ApiModels.ResponseDto.FolderDto, ASC.Files.Core">Folder for the third-party account backup</returns>
    /// <remarks>List of provider key: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive, kDrive</remarks>
    /// <path>api/2.0/files/thirdparty/backup</path>
    /// <httpMethod>POST</httpMethod>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("thirdparty/backup")]
    public async Task<FolderDto<string>> SaveThirdPartyBackupAsync(ThirdPartyBackupRequestDto inDto)
    {
        if (!await _fileSecurityCommon.IsDocSpaceAdministratorAsync(_securityContext.CurrentAccount.ID))
        {
            throw new InvalidOperationException(FilesCommonResource.ErrorMassage_SecurityException_Create);
        }

        var thirdPartyParams = new ThirdPartyParams
        {
            AuthData = new AuthData(inDto.Url, inDto.Login, inDto.Password, inDto.Token),
            CustomerTitle = inDto.CustomerTitle,
            ProviderKey = inDto.ProviderKey,
        };

        var folder = await _fileStorageService.SaveThirdPartyBackupAsync(thirdPartyParams);

        return await _folderDtoHelper.GetAsync(folder);
    }

    /// <summary>
    /// Saves the user WordPress information when logging in.
    /// </summary>
    /// <short>Save the user WordPress information</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.WordpressSaveRequestDto, ASC.Files.Core" name="inDto">Request parameters for saving WordPress information</param>
    /// <category>WordPress</category>
    /// <returns type="System.Object, System">Object with the following parameters: "success" - specifies if the operation is successful or not, "data" - blog information</returns>
    /// <path>api/2.0/files/wordpress-save</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("wordpress-save")]
    public async Task<object> WordpressSaveAsync(WordpressSaveRequestDto inDto)
    {
        if (inDto.Code.Length == 0)
        {
            return new
            {
                success = false
            };
        }
        try
        {
            var token = await _wordpressToken.SaveTokenFromCodeAsync(inDto.Code);
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");

            var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

            var blogInfo = _requestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
            var jsonBlogInfo = JObject.Parse(blogInfo);
            jsonBlogInfo.Add("username", wordpressUserName);

            blogInfo = jsonBlogInfo.ToString();
            return new
            {
                success = true,
                data = blogInfo
            };
        }
        catch (Exception)
        {
            return new
            {
                success = false
            };
        }
    }
}
