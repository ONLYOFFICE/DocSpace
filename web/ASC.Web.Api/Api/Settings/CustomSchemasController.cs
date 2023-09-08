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

namespace ASC.Web.Api.Controllers.Settings;

public class CustomSchemasController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;

    public CustomSchemasController(
        MessageService messageService,
        ApiContext apiContext,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        CustomNamingPeople customNamingPeople,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _messageService = messageService;
        _customNamingPeople = customNamingPeople;
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
    }

    /// <summary>
    /// Returns all portal team templates that allow users to name their organization (or group), add members, and define their activities within the portal.
    /// </summary>
    /// <short>Get team templates</short>
    /// <category>Team templates</category>
    /// <returns type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api">List of team templates with the following parameters</returns>
    /// <path>api/2.0/settings/customschemas</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("customschemas")]
    public async Task<List<SchemaRequestsDto>> PeopleSchemasAsync()
    {
        return await _customNamingPeople
                .GetSchemas().ToAsyncEnumerable()
                .SelectAwait(async r =>
                {
                    var names = await _customNamingPeople.GetPeopleNamesAsync(r.Key);

                    return new SchemaRequestsDto
                    {
                        Id = names.Id,
                        Name = names.SchemaName,
                        UserCaption = names.UserCaption,
                        UsersCaption = names.UsersCaption,
                        GroupCaption = names.GroupCaption,
                        GroupsCaption = names.GroupsCaption,
                        UserPostCaption = names.UserPostCaption,
                        RegDateCaption = names.RegDateCaption,
                        GroupHeadCaption = names.GroupHeadCaption,
                        GuestCaption = names.GuestCaption,
                        GuestsCaption = names.GuestsCaption,
                    };
                })
                .ToListAsync();
    }

    /// <summary>
    /// Saves the names from the team template with the ID specified in the request.
    /// </summary>
    /// <short>Save the naming settings</short>
    /// <category>Team templates</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api" name="inDto">Team template parameters</param>
    /// <returns type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api">Team template with the following parameters</returns>
    /// <path>api/2.0/settings/customschemas</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("customschemas")]
    public async Task<SchemaRequestsDto> SaveNamingSettingsAsync(SchemaRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _customNamingPeople.SetPeopleNamesAsync(inDto.Id);

        await _tenantManager.SaveTenantAsync(await _tenantManager.GetCurrentTenantAsync());

        await _messageService.SendAsync(MessageAction.TeamTemplateChanged);

        return await PeopleSchemaAsync(inDto.Id);
    }

    /// <summary>
    /// Creates a custom team template with the parameters specified in the request.
    /// </summary>
    /// <short>Create a custom team template</short>
    /// <category>Team templates</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api" name="inDto">Team template parameters</param>
    /// <returns type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api">Custom team template with the following parameters</returns>
    /// <path>api/2.0/settings/customschemas</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("customschemas")]
    public async Task<SchemaRequestsDto> SaveCustomNamingSettingsAsync(SchemaRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var usrCaption = (inDto.UserCaption ?? "").Trim();
        var usrsCaption = (inDto.UsersCaption ?? "").Trim();
        var grpCaption = (inDto.GroupCaption ?? "").Trim();
        var grpsCaption = (inDto.GroupsCaption ?? "").Trim();
        var usrStatusCaption = (inDto.UserPostCaption ?? "").Trim();
        var regDateCaption = (inDto.RegDateCaption ?? "").Trim();
        var grpHeadCaption = (inDto.GroupHeadCaption ?? "").Trim();
        var guestCaption = (inDto.GuestCaption ?? "").Trim();
        var guestsCaption = (inDto.GuestsCaption ?? "").Trim();

        if (string.IsNullOrEmpty(usrCaption)
            || string.IsNullOrEmpty(usrsCaption)
            || string.IsNullOrEmpty(grpCaption)
            || string.IsNullOrEmpty(grpsCaption)
            || string.IsNullOrEmpty(usrStatusCaption)
            || string.IsNullOrEmpty(regDateCaption)
            || string.IsNullOrEmpty(grpHeadCaption)
            || string.IsNullOrEmpty(guestCaption)
            || string.IsNullOrEmpty(guestsCaption))
        {
            throw new Exception(Resource.ErrorEmptyFields);
        }

        var names = new PeopleNamesItem
        {
            Id = PeopleNamesItem.CustomID,
            UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
            UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
            GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
            GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
            UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
            RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
            GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
            GuestCaption = guestCaption.Substring(0, Math.Min(30, guestCaption.Length)),
            GuestsCaption = guestsCaption.Substring(0, Math.Min(30, guestsCaption.Length)),
        };

        await _customNamingPeople.SetPeopleNamesAsync(names);

        await _tenantManager.SaveTenantAsync(await _tenantManager.GetCurrentTenantAsync());

        await _messageService.SendAsync(MessageAction.TeamTemplateChanged);

        return await PeopleSchemaAsync(PeopleNamesItem.CustomID);
    }

    /// <summary>
    /// Returns a team template by the ID specified in the request.
    /// </summary>
    /// <short>Get a team template by ID</short>
    /// <category>Team templates</category>
    /// <param type="System.String, System" method="url" name="id">Team template ID</param>
    /// <returns type="ASC.Web.Api.ApiModel.RequestsDto.SchemaRequestsDto, ASC.Web.Api">Team template with the following parameters</returns>
    /// <path>api/2.0/settings/customschemas/{id}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("customschemas/{id}")]
    public async Task<SchemaRequestsDto> PeopleSchemaAsync(string id)
    {
        var names = await _customNamingPeople.GetPeopleNamesAsync(id);
        var schemaItem = new SchemaRequestsDto
        {
            Id = names.Id,
            Name = names.SchemaName,
            UserCaption = names.UserCaption,
            UsersCaption = names.UsersCaption,
            GroupCaption = names.GroupCaption,
            GroupsCaption = names.GroupsCaption,
            UserPostCaption = names.UserPostCaption,
            RegDateCaption = names.RegDateCaption,
            GroupHeadCaption = names.GroupHeadCaption,
            GuestCaption = names.GuestCaption,
            GuestsCaption = names.GuestsCaption,
        };
        return schemaItem;
    }
}