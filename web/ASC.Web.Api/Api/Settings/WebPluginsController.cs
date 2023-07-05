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

using ASC.Core.Common.EF.Model;
using ASC.Data.Storage;

namespace ASC.Web.Api.Controllers.Settings;

public class WebPluginsController : BaseSettingsController
{
    private const string StorageModuleName = "webplugins";

    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly PermissionContext _permissionContext;
    private readonly DbWebPluginService _webPluginService;
    private readonly IConfiguration _configuration;
    private readonly StorageFactory _storageFactory;
    private readonly IMapper _mapper;

    private readonly bool _webPluginsEnabled;
    private readonly string _webPluginExtension;
    private readonly long _webPluginMaxSize;

    public WebPluginsController(
        ApiContext apiContext,
        PermissionContext permissionContext,
        DbWebPluginService webPluginService,
        IConfiguration configuration,
        StorageFactory storageFactory,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _permissionContext = permissionContext;
        _webPluginService = webPluginService;
        _configuration = configuration;
        _storageFactory = storageFactory;
        _mapper = mapper;

        _ = bool.TryParse(_configuration["plugins:enabled"], out _webPluginsEnabled);

        //TODO: think about configuration settings
        _webPluginExtension = ".js";
        _webPluginMaxSize = 1L * 1024 * 1024;
    }

    private void DemandWebPlugins()
    {
        if (!_webPluginsEnabled)
        {
            throw new SecurityException();
        }
    }

    [HttpPost("webplugins")]
    public async Task<WebPluginDto> AddWebPluginFromFile()
    {
        DemandWebPlugins();

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (HttpContext.Request.Form?.Files == null || HttpContext.Request.Form.Files.Count == 0)
        {
            throw new ArgumentException("No input file");
        }

        if (HttpContext.Request.Form.Files.Count > 1)
        {
            throw new ArgumentException("To many input files");
        }

        var file = HttpContext.Request.Form.Files[0] ?? throw new ArgumentException("Input file is null");

        var fileName = Path.GetFileName(file.FileName);

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("Empty file name");
        }

        if (Path.GetExtension(fileName) != _webPluginExtension)
        {
            throw new ArgumentException("Wrong file extension");
        }

        if (file.Length > _webPluginMaxSize)
        {
            throw new ArgumentException("File size exceeds limit");
        }

        var pluginName = Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant(); //TODO: think about special characters

        var storage = await _storageFactory.GetStorageAsync(Tenant.Id, StorageModuleName);

        var uri = await storage.SaveAsync(pluginName, file.OpenReadStream());

        var webPlugin = new DbWebPlugin() {
            TenantId = Tenant.Id,
            Name = pluginName,
            Enabled = true
        };

        var outDto = _mapper.Map<DbWebPlugin, WebPluginDto>(await _webPluginService.SaveAsync(webPlugin));

        outDto.Url = uri.ToString();

        return outDto;
    }

    [HttpGet("webplugins")]
    public async Task<IEnumerable<WebPluginDto>> GetWebPluginsAsync(bool? enabled = null)
    {
        DemandWebPlugins();

        //await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var outDto = _mapper.Map<IEnumerable<DbWebPlugin>, IEnumerable<WebPluginDto>>(await _webPluginService.GetAsync(Tenant.Id, enabled));

        var storage = await _storageFactory.GetStorageAsync(Tenant.Id, StorageModuleName);

        foreach (var dto in outDto)
        {
            var uri = await storage.GetUriAsync(dto.Name);

            dto.Url = uri.ToString();
        }

        return outDto;
    }

    [HttpPut("webplugins/{id}")]
    public async Task<WebPluginDto> GetWebPluginByIdAsync(int id)
    {
        DemandWebPlugins();

        //await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var outDto = _mapper.Map<DbWebPlugin, WebPluginDto>(await _webPluginService.GetByIdAsync(id));

        var storage = await _storageFactory.GetStorageAsync(Tenant.Id, StorageModuleName);

        var uri = await storage.GetUriAsync(outDto.Name);

        outDto.Url = uri.ToString();

        return outDto;
    }

    [HttpPut("webplugins/{id}")]
    public async Task UpdateWebPluginAsync(int id, WebPluginRequestsDto inDto)
    {
        DemandWebPlugins();

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _webPluginService.UpdateAsync(id, inDto.Enabled);
    }

    [HttpDelete("webplugins/{id}")]
    public async Task DeleteWebPluginAsync(int id)
    {
        DemandWebPlugins();

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _webPluginService.DeleteAsync(id);
    }
}
