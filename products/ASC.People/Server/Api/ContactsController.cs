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

namespace ASC.People.Api;

public class ContactsController : PeopleControllerBase
{
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;

    public ContactsController(
        UserManager userManager,
        PermissionContext permissionContext,
        ApiContext apiContext,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        EmployeeFullDtoHelper employeeFullDtoHelper,
        IHttpContextAccessor httpContextAccessor)
        : base(userManager, permissionContext, apiContext, userPhotoManager, httpClientFactory, httpContextAccessor)
    {
        _employeeFullDtoHelper = employeeFullDtoHelper;
    }

    /// <summary>
    /// Deletes the contacts of the user with the ID specified in the request from the portal.
    /// </summary>
    /// <short>
    /// Delete user contacts
    /// </short>
    /// <category>Contacts</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user contacts</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Web.Api">Deleted user profile with the detailed information</returns>
    /// <path>api/2.0/people/{userid}/contacts</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{userid}/contacts")]
    public async Task<EmployeeFullDto> DeleteMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await DeleteContactsAsync(inDto.Contacts, user);
        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Sets the contacts of the user with the ID specified in the request replacing the current portal data with the new data.
    /// </summary>
    /// <short>
    /// Set user contacts
    /// </short>
    /// <category>Contacts</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user contacts</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Web.Api">Updated user profile with the detailed information</returns>
    /// <path>api/2.0/people/{userid}/contacts</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{userid}/contacts")]
    public async Task<EmployeeFullDto> SetMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        user.ContactsList.Clear();
        await UpdateContactsAsync(inDto.Contacts, user);
        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Updates the contact information of the user with the ID specified in the request merging the new data into the current portal data.
    /// </summary>
    /// <short>
    /// Update user contacts
    /// </short>
    /// <category>Contacts</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user contacts</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Web.Api">Updated user profile with the detailed information</returns>
    /// <path>api/2.0/people/{userid}/contacts</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/contacts")]
    public async Task<EmployeeFullDto> UpdateMemberContacts(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await UpdateContactsAsync(inDto.Contacts, user);
        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    private async Task DeleteContactsAsync(IEnumerable<Contact> contacts, UserInfo user)
    {
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (contacts == null)
        {
            return;
        }

        if (user.ContactsList == null)
        {
            user.ContactsList = new List<string>();
        }

        foreach (var contact in contacts)
        {
            var index = user.ContactsList.IndexOf(contact.Type);
            if (index != -1)
            {
                //Remove existing
                user.ContactsList.RemoveRange(index, 2);
            }
        }
    }
}
