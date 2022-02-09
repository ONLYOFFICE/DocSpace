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

using Constants = ASC.Core.Users.Constants;

namespace ASC.Core.Notify;

public class RecipientProviderImpl : IRecipientProvider
{
    private readonly UserManager _userManager;

    public RecipientProviderImpl(UserManager userManager) =>
        _userManager = userManager;

    public virtual IRecipient GetRecipient(string id)
    {
        if (TryParseGuid(id, out var recID))
        {
            var user = _userManager.GetUsers(recID);
            if (user.Id != Constants.LostUser.Id)
            {
                return new DirectRecipient(user.Id.ToString(), user.ToString());
            }

            var group = _userManager.GetGroupInfo(recID);
            if (group.ID != Constants.LostGroupInfo.ID)
            {
                return new RecipientsGroup(group.ID.ToString(), group.Name);
            }
        }

        return null;
    }

    public virtual IRecipient[] GetGroupEntries(IRecipientsGroup group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var result = new List<IRecipient>();
        var groupID = Guid.Empty;
        if (TryParseGuid(group.ID, out groupID))
        {
            var coreGroup = _userManager.GetGroupInfo(groupID);
            if (coreGroup.ID != Constants.LostGroupInfo.ID)
            {
                var users = _userManager.GetUsersByGroup(coreGroup.ID);
                Array.ForEach(users, u => result.Add(new DirectRecipient(u.Id.ToString(), u.ToString())));
            }
        }

        return result.ToArray();
    }

    public virtual IRecipientsGroup[] GetGroups(IRecipient recipient)
    {
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        var result = new List<IRecipientsGroup>();
        if (TryParseGuid(recipient.ID, out var recID))
        {
            if (recipient is IRecipientsGroup)
            {
                var group = _userManager.GetGroupInfo(recID);

                while (group != null && group.Parent != null)
                {
                    result.Add(new RecipientsGroup(group.Parent.ID.ToString(), group.Parent.Name));
                    group = group.Parent;
                }
            }
            else if (recipient is IDirectRecipient)
            {
                foreach (var group in _userManager.GetUserGroups(recID, IncludeType.Distinct))
                {
                    result.Add(new RecipientsGroup(group.ID.ToString(), group.Name));
                }
            }
        }

        return result.ToArray();
    }

    public virtual string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName)
    {
        if (recipient == null)
        {
            throw new ArgumentNullException(nameof(recipient));
        }

        if (TryParseGuid(recipient.ID, out var userID))
        {
            var user = _userManager.GetUsers(userID);
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
    public IDirectRecipient FilterRecipientAddresses(IDirectRecipient recipient)
    {
        //Check activation
        if (recipient.CheckActivation)
        {
            //It's direct email
            if (recipient.Addresses != null && recipient.Addresses.Any())
            {
                //Filtering only missing users and users who activated already
                var filteredAddresses = from address in recipient.Addresses
                                        let user = _userManager.GetUserByEmail(address)
                                        where user.Id == Constants.LostUser.Id || (user.IsActive && (user.Status & EmployeeStatus.Default) == user.Status)
                                        select address;

                return new DirectRecipient(recipient.ID, recipient.Name, filteredAddresses.ToArray(), false);
            }
        }

        return recipient;
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
