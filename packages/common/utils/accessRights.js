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

  switch (access) {
    case access === ShareAccessRights.None:
    case access === ShareAccessRights.FullAccess:
      for (key in rights) {
        rights[key] = true;
      }

      return rights;

    case access === ShareAccessRights.RoomManager:
      for (key in rights) {
        rights[key] = true;
      }

      rights.archive = false;
      rights.delete = false;

      return rights;

    default:
      rights.viewHistory = true;
      rights.viewInfo = true;
      rights.viewUsers = true;

      return rights;
  }
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
