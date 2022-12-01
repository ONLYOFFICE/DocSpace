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

export const getRoomRoleActions = (access) => {
  switch (access) {
    case ShareAccessRights.None:
    case ShareAccessRights.FullAccess:
      return OwnerRoomsActions;
    case ShareAccessRights.RoomManager:
      return RoomAdminRoomsActions;
    case ShareAccessRights.Editing:
      return EditorRoomsActions;
    case ShareAccessRights.FormFilling:
      return FormFillerRoomsActions;
    case ShareAccessRights.Review:
      return ReviewerRoomsActions;
    case ShareAccessRights.Comment:
      return CommentatorRoomsActions;
    case ShareAccessRights.ReadOnly:
      return ViewerRoomsActions;
    default:
      return RoomsActions;
  }
};

export const getFileRoleActions = (access) => {
  switch (access) {
    case ShareAccessRights.None:
    case ShareAccessRights.FullAccess:
      return OwnerFilesActions;
    case ShareAccessRights.RoomManager:
      return RoomAdminFilesActions;
    case ShareAccessRights.Editing:
      return EditorFilesActions;
    case ShareAccessRights.FormFilling:
      return FormFillerFilesActions;
    case ShareAccessRights.Review:
      return ReviewerFilesActions;
    case ShareAccessRights.Comment:
      return CommentatorFilesActions;
    case ShareAccessRights.ReadOnly:
      return ViewerFilesActions;
    default:
      return FilesActions;
  }
};

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

export const getArchiveFileRoleActions = (access) => {
  switch (access) {
    case ShareAccessRights.None:
    case ShareAccessRights.FullAccess:
      return OwnerArchiveFilesActions;
    case ShareAccessRights.RoomManager:
      return RoomAdminArchiveFilesActions;
    case ShareAccessRights.Editing:
      return EditorArchiveFilesActions;
    case ShareAccessRights.FormFilling:
      return FormFillerArchiveFilesActions;
    case ShareAccessRights.Review:
      return ReviewerArchiveFilesActions;
    case ShareAccessRights.Comment:
      return CommentatorArchiveFilesActions;
    case ShareAccessRights.ReadOnly:
      return ViewerArchiveFilesActions;
    default:
      return ArchiveFilesActions;
  }
};

export const getAccountsTypeActions = (isAdmin, isOwner) => {
  if (isOwner) return OwnerAccountsActions;

  if (isAdmin) return DocSpaceAdminAccountsActions;

  return RoomAdminAccountsActions;
};
