import { ShareAccessRights } from "../../constants/index";

import {
  RoomsActions,
  OwnerRoomsActions,
  RoomAdminRoomsActions,
  EditorRoomsActions,
  FormFillerRoomsActions,
  ReviewerRoomsActions,
  CommentatorRoomsActions,
  ViewerRoomsActions,
} from "./Rooms";

import {
  ArchiveRoomsActions,
  OwnerArchiveRoomsActions,
  RoomAdminArchiveRoomsActions,
  EditorArchiveRoomsActions,
  FormFillerArchiveRoomsActions,
  ReviewerArchiveRoomsActions,
  CommentatorArchiveRoomsActions,
  ViewerArchiveRoomsActions,
} from "./ArchiveRoom";

import {
  FilesActions,
  OwnerFilesActions,
  RoomAdminFilesActions,
  EditorFilesActions,
  FormFillerFilesActions,
  ReviewerFilesActions,
  CommentatorFilesActions,
  ViewerFilesActions,
} from "./Files";

import {
  ArchiveFilesActions,
  OwnerArchiveFilesActions,
  RoomAdminArchiveFilesActions,
  EditorArchiveFilesActions,
  FormFillerArchiveFilesActions,
  ReviewerArchiveFilesActions,
  CommentatorArchiveFilesActions,
  ViewerArchiveFilesActions,
} from "./ArchiveFiles";

import {
  OwnerAccountsActions,
  DocSpaceAdminAccountsActions,
  RoomAdminAccountsActions,
} from "./Accounts";

export const getArchiveRoomRoleActions = (access) => {
  switch (access) {
    case ShareAccessRights.None:
    case ShareAccessRights.FullAccess:
      return OwnerArchiveRoomsActions;
    case ShareAccessRights.RoomManager:
      return RoomAdminArchiveRoomsActions;
    case ShareAccessRights.Editing:
      return EditorArchiveRoomsActions;
    case ShareAccessRights.FormFilling:
      return FormFillerArchiveRoomsActions;
    case ShareAccessRights.Review:
      return ReviewerArchiveRoomsActions;
    case ShareAccessRights.Comment:
      return CommentatorArchiveRoomsActions;
    case ShareAccessRights.ReadOnly:
      return ViewerArchiveRoomsActions;
    default:
      return ArchiveRoomsActions;
  }
};

export const getAccountsTypeActions = (isAdmin, isOwner) => {
  if (isOwner) return OwnerAccountsActions;

  if (isAdmin) return DocSpaceAdminAccountsActions;

  return RoomAdminAccountsActions;
};
