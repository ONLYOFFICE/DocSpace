import { makeAutoObservable } from "mobx";

class DialogStore {
  changeEmail = false;
  changePassword = false;
  deleteSelfProfile = false;
  deleteProfileEver = false;
  data = {};

  constructor() {
    makeAutoObservable(this);
  }

  setChangeEmailDialogVisible = (visible) => {
    this.changeEmail = visible;
  };

  setChangePasswordDialogVisible = (visible) => {
    this.changePassword = visible;
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

  closeDialogs = () => {
    this.setChangeEmailDialogVisible(false);
    this.setChangePasswordDialogVisible(false);
    this.setDeleteSelfProfileDialogVisible(false);
    this.setDeleteProfileDialogVisible(false);
    this.setDialogData({});
  };
}

export default DialogStore;
