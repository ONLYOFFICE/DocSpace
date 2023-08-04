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

internal class DocumentsActionMapper : IProductActionMapper
{
    public List<IModuleActionMapper> Mappers { get; }
    public ProductType Product { get; }
    public DocumentsActionMapper()
    {
        Product = ProductType.Documents;

        Mappers = new List<IModuleActionMapper>()
        {
            new FilesActionMapper(),
            new FoldersActionMapper(),
            new RoomsActionMapper(),
            new SettingsActionMapper()
        };
    }
}
internal class FilesActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public FilesActionMapper()
    {
        Module = ModuleType.Files;
        Actions = new MessageMapsDictionary(ProductType.Documents, Module)
        {
            {
                EntryType.File, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Create, new[] { MessageAction.FileCreated, MessageAction.FileCreatedVersion, MessageAction.FileRestoreVersion, MessageAction.FileConverted } },
                    {
                        ActionType.Update, new[]
                        {
                            MessageAction.FileRenamed, MessageAction.FileUpdated, MessageAction.UserFileUpdated, MessageAction.FileUpdatedRevisionComment,
                            MessageAction.FileLocked, MessageAction.FileUnlocked, MessageAction.FileOpenedForChange, MessageAction.FileMarkedAsFavorite,
                            MessageAction.FileRemovedFromFavorite, MessageAction.FileMarkedAsRead, MessageAction.FileReaded
                        }
                    },
                    { ActionType.Delete, new[] { MessageAction.FileDeletedVersion, MessageAction.FileDeleted, MessageAction.TrashEmptied } },
                    { ActionType.UpdateAccess, new[] { MessageAction.FileUpdatedAccess, MessageAction.FileUpdatedAccessFor, MessageAction.FileRemovedFromList, MessageAction.FileExternalLinkAccessUpdated } },
                    { ActionType.Download, new[] {  MessageAction.FileDownloaded, MessageAction.FileDownloadedAs, MessageAction.FileRevisionDownloaded } },
                    { ActionType.Send, new[] { MessageAction.FileSendAccessLink, MessageAction.FileChangeOwner } },
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Upload, MessageAction.FileUploaded },
                    { ActionType.Import, MessageAction.FileImported },
                    { ActionType.Move, MessageAction.FileMovedToTrash }
                }
            },
            {
                EntryType.File, EntryType.Folder, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Copy, new[] { MessageAction.FileCopied, MessageAction.FileCopiedWithOverwriting } },
                    { ActionType.Move, new[] { MessageAction.FileMoved, MessageAction.FileMovedWithOverwriting } },
                }
            },
        };

        Actions.Add(MessageAction.DocumentSignComplete, new MessageMaps("FilesDocumentSigned", ActionType.Send, ProductType.Documents, Module, EntryType.File));
        Actions.Add(MessageAction.DocumentSendToSign, new MessageMaps("FilesRequestSign", ActionType.Send, ProductType.Documents, Module, EntryType.File));
    }
}

internal class FoldersActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public FoldersActionMapper()
    {
        Module = ModuleType.Folders;
        Actions = new MessageMapsDictionary(ProductType.Documents, Module)
        {
            {
                EntryType.Folder, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Update, new[] { MessageAction.FolderRenamed, MessageAction.FolderMarkedAsRead } },
                    { ActionType.UpdateAccess, new[] { MessageAction.FolderUpdatedAccess, MessageAction.FolderUpdatedAccessFor, MessageAction.FolderRemovedFromList } }
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.FolderCreated },
                    { ActionType.Move, MessageAction.FolderMovedToTrash },
                    { ActionType.Delete, MessageAction.FolderDeleted },
                    { ActionType.Download, MessageAction.FolderDownloaded },
                }
            },
            {
                EntryType.Folder, EntryType.Folder, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Copy, new[] { MessageAction.FolderCopied, MessageAction.FolderCopiedWithOverwriting } },
                    { ActionType.Move, new[] { MessageAction.FolderMoved, MessageAction.FolderMovedWithOverwriting } },
                }
            },
        };
    }
}

internal class RoomsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public RoomsActionMapper()
    {
        Module = ModuleType.Rooms;
        Actions = new MessageMapsDictionary(ProductType.Documents, Module)
        {
            {
                EntryType.Room, new Dictionary<ActionType, MessageAction[]>
                {
                    { ActionType.Create, new[] { MessageAction.RoomCreated } },
                    {
                        ActionType.Update, new[]
                        {
                            MessageAction.RoomArchived,
                            MessageAction.RoomUnarchived,
                            MessageAction.RoomRenamed,
                            MessageAction.AddedRoomTags,
                            MessageAction.DeletedRoomTags,
                            MessageAction.RoomLogoCreated,
                            MessageAction.RoomLogoDeleted,
                            MessageAction.RoomCreateUser,
                            MessageAction.RoomUpdateAccessForUser,
                            MessageAction.RoomRemoveUser,
                            MessageAction.RoomInvitationLinkCreated,
                            MessageAction.RoomInvitationLinkUpdated,
                            MessageAction.RoomInvitationLinkDeleted
                        }
                    },
                    {
                        ActionType.Delete, new [] { MessageAction.RoomDeleted }
                    }
                }
            },
            {
                EntryType.Tag, new Dictionary<ActionType, MessageAction>
                {
                    { ActionType.Create, MessageAction.TagCreated }, 
                    { ActionType.Delete, MessageAction.TagsDeleted }
                }
            }
        };
    }
}

internal class SettingsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public SettingsActionMapper()
    {
        Module = ModuleType.DocumentsSettings;
        Actions = new MessageMapsDictionary(ProductType.Documents, Module)
        {
            {
                EntryType.Folder, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create,  MessageAction.ThirdPartyCreated  },
                    { ActionType.Update, MessageAction.ThirdPartyUpdated },
                    { ActionType.Delete, MessageAction.ThirdPartyDeleted },
                }
            },
            {
                ActionType.Update, new []
                {
                    MessageAction.DocumentsThirdPartySettingsUpdated, MessageAction.DocumentsOverwritingSettingsUpdated,
                    MessageAction.DocumentsForcesave, MessageAction.DocumentsStoreForcesave, MessageAction.DocumentsUploadingFormatsSettingsUpdated,
                    MessageAction.DocumentsExternalShareSettingsUpdated
                }
            },
        };
    }
}