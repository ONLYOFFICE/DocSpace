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

export const getAccountsTypeActions = (isAdmin, isOwner) => {
  if (isOwner) return OwnerAccountsActions;

  if (isAdmin) return DocSpaceAdminAccountsActions;

  return RoomAdminAccountsActions;
};
