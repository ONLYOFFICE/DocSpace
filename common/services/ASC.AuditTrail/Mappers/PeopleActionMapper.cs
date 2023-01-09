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


namespace ASC.AuditTrail.Mappers;

internal class PeopleActionMapper : IProductActionMapper
{
    public List<IModuleActionMapper> Mappers { get; }
    public ProductType Product { get; }

    public PeopleActionMapper()
    {
        Product = ProductType.People;

        Mappers = new List<IModuleActionMapper>()
        {
            new UsersActionMapper(),
            new GroupsActionMapper()
        };
    }
}

internal class UsersActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public UsersActionMapper()
    {
        Module = ModuleType.Users;

        Actions = new MessageMapsDictionary(ProductType.People, Module)
        {
            {
                EntryType.User,
                new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Create,  new[] { MessageAction.UserCreated, MessageAction.GuestCreated, MessageAction.UserCreatedViaInvite, MessageAction.GuestCreatedViaInvite }  },
                    {
                        ActionType.Update,  new[]
                        {
                            MessageAction.UserActivated, MessageAction.GuestActivated, MessageAction.UserUpdated,
                            MessageAction.UserUpdatedMobileNumber, MessageAction.UserUpdatedLanguage, MessageAction.UserAddedAvatar,
                            MessageAction.UserUpdatedAvatarThumbnails, MessageAction.UserUpdatedEmail, MessageAction.UsersUpdatedType,
                            MessageAction.UsersUpdatedStatus, MessageAction.UsersSentActivationInstructions,
                        }
                    },
                    { ActionType.Delete, new[] { MessageAction.UserDeletedAvatar, MessageAction.UserDeleted, MessageAction.UsersDeleted, MessageAction.UserDataRemoving } },
                    { ActionType.Import, new[] { MessageAction.UserImported, MessageAction.GuestImported } },
                    { ActionType.Logout, new[] { MessageAction.UserLogoutActiveConnections, MessageAction.UserLogoutActiveConnection, MessageAction.UserLogoutActiveConnectionsForUser } },
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Reassigns, MessageAction.UserDataReassigns }
                }
            },
            { MessageAction.UserLinkedSocialAccount, ActionType.Link },
            { MessageAction.UserUnlinkedSocialAccount, ActionType.Unlink },
            {
                ActionType.Send,
                new[] { MessageAction.UserSentActivationInstructions, MessageAction.UserSentDeleteInstructions, MessageAction.SentInviteInstructions }
            },
            { MessageAction.UserUpdatedPassword, ActionType.Update },
            { MessageAction.UserSentEmailChangeInstructions, new MessageMaps("UserSentEmailInstructions", ActionType.Send, ProductType.People, Module, EntryType.User) },
            { MessageAction.UserSentPasswordChangeInstructions, new MessageMaps("UserSentPasswordInstructions", ActionType.Send, ProductType.People, Module, EntryType.User) },
            { MessageAction.UserConnectedTfaApp, new MessageMaps("UserTfaGenerateCodes", ActionType.Link, ProductType.People, Module, EntryType.User) },
            { MessageAction.UserDisconnectedTfaApp, new MessageMaps("UserTfaDisconnected", ActionType.Delete, ProductType.People, Module, EntryType.User) }
        };
    }
}

internal class GroupsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public GroupsActionMapper()
    {
        Module = ModuleType.Groups;

        Actions = new MessageMapsDictionary(ProductType.People, Module)
        {
            {
                EntryType.Group, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.GroupCreated },
                    { ActionType.Update, MessageAction.GroupUpdated },
                    { ActionType.Delete, MessageAction.GroupDeleted }
                }
            }
        };
    }
}