import { makeAutoObservable } from "mobx";

class DialogStore {
  changeOwner = false;
  deleteSelfProfile = false;
  deleteProfileEver = false;
  data = {};

  changeUserTypeDialogVisible = false;

  changeUserStatusDialogVisible = false;
  disableDialogVisible = false;
  sendInviteDialogVisible = false;
  deleteDialogVisible = false;
  resetAuthDialogVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  setChangeOwnerDialogVisible = (visible) => {
    this.changeOwner = visible;
  };

  setDeleteSelfProfileDialogVisible = (visible) => {
    this.deleteSelfProfile = visible;
  };

  setDeleteProfileDialogVisible = (visible) => {
    this.deleteProfileEver = visible;
  };

  setDialogData = (data) => {
    this.data = data;
  };

  setChangeUserTypeDialogVisible = (visible) => {
    this.changeUserTypeDialogVisible = visible;
  };

  setChangeUserStatusDialogVisible = (visible) => {
    this.changeUserStatusDialogVisible = visible;
  };

  setSendInviteDialogVisible = (visible) => {
    this.sendInviteDialogVisible = visible;
  };

  setDeleteDialogVisible = (visible) => {
    this.deleteDialogVisible = visible;
  };

  setResetAuthDialogVisible = (visible) => {
    this.resetAuthDialogVisible = visible;
  };

  closeDialogs = () => {
    this.setChangeOwnerDialogVisible(false);
    this.setDeleteSelfProfileDialogVisible(false);
    this.setDeleteProfileDialogVisible(false);
    this.setDialogData({});

    this.setChangeUserTypeDialogVisible(false);
    this.setChangeUserStatusDialogVisible(false);

    this.setSendInviteDialogVisible(false);
    this.setDeleteDialogVisible(false);
    this.setResetAuthDialogVisible(false);
  };
}

export default DialogStore;
