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

using Constants = ASC.Core.Users.Constants;

namespace ASC.Core.Notify;

public class RecipientProviderImpl : IRecipientProvider
{
    private readonly UserManager _userManager;

    public RecipientProviderImpl(UserManager userManager)
    {
        _userManager = userManager;
    }

    public virtual async Task<IRecipient> GetRecipientAsync(string id)
    {
        if (TryParseGuid(id, out var recID))
        {
            var user = await _userManager.GetUsersAsync(recID);
            if (user.Id != Constants.LostUser.Id)
            {
                return new DirectRecipient(user.Id.ToString(), user.ToString());
            }

            var group = await _userManager.GetGroupInfoAsync(recID);
            if (group.ID != Constants.LostGroupInfo.ID)
            {
                return new RecipientsGroup(group.ID.ToString(), group.Name);
            }
        }

        return null;
    }

    public virtual async Task<IRecipient[]> GetGroupEntriesAsync(IRecipientsGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var result = new List<IRecipient>();
        if (TryParseGuid(group.ID, out var groupID))
        {
            var coreGroup = await _userManager.GetGroupInfoAsync(groupID);
            if (coreGroup.ID != Constants.LostGroupInfo.ID)
            {
                var users = await _userManager.GetUsersByGroupAsync(coreGroup.ID);
                Array.ForEach(users, u => result.Add(new DirectRecipient(u.Id.ToString(), u.ToString())));
            }
        }

        return result.ToArray();
    }

    public virtual async Task<IRecipientsGroup[]> GetGroupsAsync(IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        var result = new List<IRecipientsGroup>();
        if (TryParseGuid(recipient.ID, out var recID))
        {
            if (recipient is IRecipientsGroup)
            {
                var group = await _userManager.GetGroupInfoAsync(recID);
                while (group != null && group.Parent != null)
                {
                    result.Add(new RecipientsGroup(group.Parent.ID.ToString(), group.Parent.Name));
                    group = group.Parent;
                }
            }
            else if (recipient is IDirectRecipient)
            {
                foreach (var group in (await _userManager.GetUserGroupsAsync(recID, IncludeType.Distinct)))
                {
                    result.Add(new RecipientsGroup(group.ID.ToString(), group.Name));
                }
            }
        }

        return result.ToArray();
    }

    public virtual async Task<string[]> GetRecipientAddressesAsync(IDirectRecipient recipient, string senderName)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        if (TryParseGuid(recipient.ID, out var userID))
        {
            var user = await _userManager.GetUsersAsync(userID);
            if (user.Id != Constants.LostUser.Id)
            {
                if (senderName == Configuration.Constants.NotifyEMailSenderSysName)
                {
                    return new[] { user.Email };
                }

                if (senderName == Configuration.Constants.NotifyMessengerSenderSysName)
                {
                    return new[] { user.UserName };
                }

                if (senderName == Configuration.Constants.NotifyPushSenderSysName)
                {
                    return new[] { user.UserName };
                }

                if (senderName == Configuration.Constants.NotifyTelegramSenderSysName)
                {
                    return new[] { user.Id.ToString() };
                }
            }
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Check if user with this email is activated
    /// </summary>
    /// <param name="recipient"></param>
    /// <returns></returns>
    public async Task<IDirectRecipient> FilterRecipientAddressesAsync(IDirectRecipient recipient)
    {
        //Check activation
        if (recipient.CheckActivation)
        {
            //It's direct email
            if (recipient.Addresses != null && recipient.Addresses.Length > 0)
            {
                //Filtering only missing users and users who activated already

                var filteredAddresses = await recipient.Addresses.ToAsyncEnumerable().WhereAwait(WhereAsync).ToArrayAsync();

                return new DirectRecipient(recipient.ID, recipient.Name, filteredAddresses.ToArray(), false);
            }
        }

        return recipient;
    }

    private async ValueTask<bool> WhereAsync(string address)
    {
        var user = await _userManager.GetUserByEmailAsync(address);
        return user.Id == Constants.LostUser.Id || (user.IsActive && (user.Status & EmployeeStatus.Default) == user.Status);
    }


    private bool TryParseGuid(string id, out Guid guid)
    {
        guid = Guid.Empty;
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                guid = new Guid(id);

                return true;
            }
            catch (FormatException) { }
            catch (OverflowException) { }
        }

        return false;
    }
}
