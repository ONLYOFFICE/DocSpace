import { ShareAccessRights } from "../constants/index";

export const getRoomRights = (access) => {
  const rights = {
    edit: false,
    inviteUsers: false,
    changeUserRole: false,
    viewUsers: false,
    viewHistory: false,
    viewInfo: false,
    deleteUsers: false,
    archive: false,
    delete: false,
  };

  if (
    access === ShareAccessRights.None ||
    access === ShareAccessRights.FullAccess
  ) {
    for (let key in rights) {
      rights[key] = true;
    }

    return rights;
  }

  if (access === ShareAccessRights.RoomManager) {
    for (key in rights) {
      rights[key] = true;
    }

    rights.archive = false;
    rights.delete = false;

    return rights;
  }

  rights.viewHistory = true;
  rights.viewInfo = true;
  rights.viewUsers = true;

  return rights;
};

export const getFilesRights = (access) => {
  const rights = {
    create: false,
    load: false,
    edit: false,
    fillForm: false,
    peerReview: false,
    commenting: false,
    block: false,
    viewVersionHistory: false,
    changeVersionHistory: false,
    viewContent: false,
    viewComments: false,
    copyAtBuffer: false,
    printing: false,
    download: false,
    deleteSelf: false,
    moveSelf: false,
    deleteAlien: false,
    moveAlien: false,
    rename: false,
    copyFromPersonal: false,
  };

  rights.viewContent = true;
  rights.viewComments = true;
  rights.copyAtBuffer = true;
  rights.printing = true;
  rights.download = true;

  if (access === ShareAccessRights.ReadOnly) return rights;

  rights.commenting = true;

  if (access === ShareAccessRights.Comment) return rights;

  rights.peerReview = true;

  if (access === ShareAccessRights.Review) return rights;

  rights.fillForm = true;

  if (access === ShareAccessRights.FormFilling) return rights;

  rights.edit = true;
  rights.viewVersionHistory = true;

  if (access === ShareAccessRights.Editing) return rights;

  rights.create = true;
  rights.load = true;
  rights.block = true;
  rights.changeVersionHistory = true;
  rights.deleteSelf = true;
  rights.moveSelf = true;
  rights.deleteAlien = true;
  rights.moveAlien = true;
  rights.rename = true;
  rights.copyFromPersonal = true;

  return rights;
};

export const getAccountsRights = (isAdmin, isOwner) => {
  const rights = {
    inviteDocspaceAdmin: false,
    inviteRoomAdmin: false,
    inviteUser: false,
    raiseToDocspaceAdmin: false,
    raiseToRoomAdmin: false,
    downgradeToRoomAdmin: false,
    downgradeToUser: false,
    blockDocspaceAdmin: false,
    blockRoomAdmin: false,
    blockUser: false,
    changeDocspaceAdminData: false,
    changeRoomAdminData: false,
    changeUserData: false,
    deleteDocspaceAdmin: false,
    deleteRoomAdmin: false,
    deleteUser: false,
  };

  rights.inviteRoomAdmin = true;
  rights.inviteUser = true;
  rights.raiseToRoomAdmin = true;

  if (!isAdmin && !isOwner) return rights;

  rights.blockRoomAdmin = true;
  rights.blockUser = true;
  rights.changeRoomAdminData = true;
  rights.changeUserData = true;
  rights.deleteRoomAdmin = true;
  rights.deleteUser = true;

  if (isAdmin && !isOwner) return rights;

  rights.inviteDocspaceAdmin = true;
  rights.raiseToDocspaceAdmin = true;
  rights.downgradeToRoomAdmin = true;
  rights.changeDocspaceAdminData = true;
  rights.blockDocspaceAdmin = true;
  rights.deleteDocspaceAdmin = true;

  return rights;
};
