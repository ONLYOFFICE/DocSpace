namespace ASC.Files.Api;

public class ThirdpartyController : ApiControllerBase
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly EasyBibHelper _easyBibHelper;
    private readonly EntryManager _entryManager;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly FolderWrapperHelper _folderWrapperHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly SecurityContext _securityContext;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;
    private readonly UserManager _userManager;
    private readonly WordpressHelper _wordpressHelper;
    private readonly WordpressToken _wordpressToken;

    public ThirdpartyController(
        FilesControllerHelper<int> filesControllerHelperInt, 
        FilesControllerHelper<string> filesControllerHelperString,
        CoreBaseSettings coreBaseSettings,
        ConsumerFactory consumerFactory,
        EntryManager entryManager,
        FilesSettingsHelper filesSettingsHelper,
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString,
        FolderWrapperHelper folderWrapperHelper,
        GlobalFolderHelper globalFolderHelper,
        SecurityContext securityContext,
        ThirdpartyConfiguration thirdpartyConfiguration,
        UserManager userManager,
        WordpressHelper wordpressHelper,
        WordpressToken wordpressToken
        ) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _userManager = userManager;
        _wordpressHelper = wordpressHelper;
        _wordpressToken = wordpressToken;
        _folderWrapperHelper = folderWrapperHelper;
        _globalFolderHelper = globalFolderHelper;
        _securityContext = securityContext;
        _thirdpartyConfiguration = thirdpartyConfiguration;
        _coreBaseSettings = coreBaseSettings;
        _entryManager = entryManager;
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageServiceString = fileStorageServiceString;
        _filesSettingsHelper = filesSettingsHelper;
        _easyBibHelper = consumerFactory.Get<EasyBibHelper>();
    }

    /// <summary>
    ///   Get a list of available providers
    /// </summary>
    /// <category>Third-Party Integration</category>
    /// <returns>List of provider key</returns>
    /// <remarks>List of provider key: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive</remarks>
    /// <returns></returns>
    [Read("thirdparty/capabilities")]
    public List<List<string>> Capabilities()
    {
        var result = new List<List<string>>();

        if (_userManager.GetUsers(_securityContext.CurrentAccount.ID).IsVisitor(_userManager)
                || (!_filesSettingsHelper.EnableThirdParty
                && !_coreBaseSettings.Personal))
        {
            return result;
        }

        return _thirdpartyConfiguration.GetProviders();
    }

    /// <visible>false</visible>
    [Create("wordpress")]
    public bool CreateWordpressPostFromBody([FromBody] CreateWordpressPostModel model)
    {
        return CreateWordpressPost(model);
    }

    [Create("wordpress")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool CreateWordpressPostFromForm([FromForm] CreateWordpressPostModel model)
    {
        return CreateWordpressPost(model);
    }

    /// <summary>
    ///   Removes the third party file storage service account with the ID specified in the request
    /// </summary>
    /// <param name="providerId">Provider ID. Provider id is part of folder id.
    /// Example, folder id is "sbox-123", then provider id is "123"
    /// </param>
    /// <short>Remove third party account</short>
    /// <category>Third-Party Integration</category>
    /// <returns>Folder id</returns>
    ///<exception cref="ArgumentException"></exception>
    [Delete("thirdparty/{providerId:int}")]
    public Task<object> DeleteThirdPartyAsync(int providerId)
    {
        return _fileStorageServiceString.DeleteThirdPartyAsync(providerId.ToString(CultureInfo.InvariantCulture));

    }

    /// <visible>false</visible>
    [Read("wordpress-delete")]
    public object DeleteWordpressInfo()
    {
        var token = _wordpressToken.GetToken();
        if (token != null)
        {
            _wordpressToken.DeleteToken(token);
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

    /// <visible>false</visible>
    [Create("easybib-citation")]
    public object EasyBibCitationBookFromBody([FromBody] EasyBibCitationBookModel model)
    {
        return EasyBibCitationBook(model);
    }

    [Create("easybib-citation")]
    [Consumes("application/x-www-form-urlencoded")]
    public object EasyBibCitationBookFromForm([FromForm] EasyBibCitationBookModel model)
    {
        return EasyBibCitationBook(model);
    }

    /// <summary>
    ///    Returns the list of third party services connected in the 'Common Documents' section
    /// </summary>
    /// <category>Third-Party Integration</category>
    /// <short>Get third party folder</short>
    /// <returns>Connected providers folder</returns>
    [Read("thirdparty/common")]
    public async Task<IEnumerable<FolderWrapper<string>>> GetCommonThirdPartyFoldersAsync()
    {
        var parent = await _fileStorageServiceInt.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync);
        var thirdpartyFolders = await _entryManager.GetThirpartyFoldersAsync(parent);
        var result = new List<FolderWrapper<string>>();

        foreach (var r in thirdpartyFolders)
        {
            result.Add(await _folderWrapperHelper.GetAsync(r));
        }
        return result;
    }

    /// <visible>false</visible>
    [Read("easybib-citation-list")]
    public object GetEasybibCitationList(int source, string data)
    {
        try
        {
            var citationList = EasyBibHelper.GetEasyBibCitationsList(source, data);
            return new
            {
                success = true,
                citations = citationList
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

    /// <visible>false</visible>
    [Read("easybib-styles")]
    public object GetEasybibStyles()
    {
        try
        {
            var data = EasyBibHelper.GetEasyBibStyles();
            return new
            {
                success = true,
                styles = data
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

    /// <summary>
    ///    Returns the list of all connected third party services
    /// </summary>
    /// <category>Third-Party Integration</category>
    /// <short>Get third party list</short>
    /// <returns>Connected providers</returns>
    [Read("thirdparty")]
    public async Task<IEnumerable<ThirdPartyParams>> GetThirdPartyAccountsAsync()
    {
        return await _fileStorageServiceString.GetThirdPartyAsync();
    }

    /// <visible>false</visible>
    [Read("wordpress-info")]
    public object GetWordpressInfo()
    {
        var token = _wordpressToken.GetToken();
        if (token != null)
        {
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");
            var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

            var blogInfo = RequestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
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
    ///   Saves the third party file storage service account
    /// </summary>
    /// <short>Save third party account</short>
    /// <param name="url">Connection url for SharePoint</param>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    /// <param name="token">Authentication token</param>
    /// <param name="isCorporate"></param>
    /// <param name="customerTitle">Title</param>
    /// <param name="providerKey">Provider Key</param>
    /// <param name="providerId">Provider ID</param>
    /// <category>Third-Party Integration</category>
    /// <returns>Folder contents</returns>
    /// <remarks>List of provider key: DropboxV2, Box, WebDav, Yandex, OneDrive, SharePoint, GoogleDrive</remarks>
    /// <exception cref="ArgumentException"></exception>
    [Create("thirdparty")]
    public Task<FolderWrapper<string>> SaveThirdPartyFromBodyAsync([FromBody] ThirdPartyModel model)
    {
        return SaveThirdPartyAsync(model);
    }

    [Create("thirdparty")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderWrapper<string>> SaveThirdPartyFromFormAsync([FromForm] ThirdPartyModel model)
    {
        return SaveThirdPartyAsync(model);
    }

    /// <visible>false</visible>
    [Create("wordpress-save")]
    public object WordpressSaveFromBody([FromBody] WordpressSaveModel model)
    {
        return WordpressSave(model);
    }

    [Create("wordpress-save")]
    [Consumes("application/x-www-form-urlencoded")]
    public object WordpressSaveFromForm([FromForm] WordpressSaveModel model)
    {
        return WordpressSave(model);
    }

    private bool CreateWordpressPost(CreateWordpressPostModel model)
    {
        try
        {
            var token = _wordpressToken.GetToken();
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var parser = JObject.Parse(meInfo);
            if (parser == null) return false;
            var blogId = parser.Value<string>("token_site_id");

            if (blogId != null)
            {
                var createPost = _wordpressHelper.CreateWordpressPost(model.Title, model.Content, model.Status, blogId, token);
                return createPost;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private object EasyBibCitationBook(EasyBibCitationBookModel model)
    {
        try
        {
            var citat = _easyBibHelper.GetEasyBibCitation(model.CitationData);
            if (citat != null)
            {
                return new
                {
                    success = true,
                    citation = citat
                };
            }
            else
            {
                return new
                {
                    success = false
                };
            }

        }
        catch (Exception)
        {
            return new
            {
                success = false
            };
        }
    }

    private async Task<FolderWrapper<string>> SaveThirdPartyAsync(ThirdPartyModel model)
    {
        var thirdPartyParams = new ThirdPartyParams
        {
            AuthData = new AuthData(model.Url, model.Login, model.Password, model.Token),
            Corporate = model.IsCorporate,
            CustomerTitle = model.CustomerTitle,
            ProviderId = model.ProviderId,
            ProviderKey = model.ProviderKey,
        };

        var folder = await _fileStorageServiceString.SaveThirdPartyAsync(thirdPartyParams);

        return await _folderWrapperHelper.GetAsync(folder);
    }

    private object WordpressSave(WordpressSaveModel model)
    {
        if (model.Code.Length == 0)
        {
            return new
            {
                success = false
            };
        }
        try
        {
            var token = _wordpressToken.SaveTokenFromCode(model.Code);
            var meInfo = _wordpressHelper.GetWordpressMeInfo(token.AccessToken);
            var blogId = JObject.Parse(meInfo).Value<string>("token_site_id");

            var wordpressUserName = JObject.Parse(meInfo).Value<string>("username");

            var blogInfo = RequestHelper.PerformRequest(WordpressLoginProvider.WordpressSites + blogId, "", "GET", "");
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
