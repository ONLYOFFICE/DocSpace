import { makeAutoObservable } from "mobx";

class DialogStore {
  changeName = false;
  changeEmail = false;
  changePassword = false;
  changeOwner = false;
  deleteSelfProfile = false;
  deleteProfileEver = false;
  data = {};

  changeUserTypeDialogVisible = false;

  changeUserStatusDialogVisible = false;
  disableDialogVisible = false;
  sendInviteDialogVisible = false;
  invitationDialogVisible = false;
  deleteDialogVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  setChangeNameDialogVisible = (visible) => {
    this.changeName = visible;
  };

  setChangeEmailDialogVisible = (visible) => {
    this.changeEmail = visible;
  };

  setChangePasswordDialogVisible = (visible) => {
    this.changePassword = visible;
  };

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

  setInvitationDialogVisible = (visible) => {
    this.invitationDialogVisible = visible;
  };

  closeDialogs = () => {
    this.setChangeEmailDialogVisible(false);
    this.setChangePasswordDialogVisible(false);
    this.setChangeOwnerDialogVisible(false);
    this.setDeleteSelfProfileDialogVisible(false);
    this.setDeleteProfileDialogVisible(false);
    this.setDialogData({});

    this.setChangeUserTypeDialogVisible(false);
    this.setChangeUserStatusDialogVisible(false);

    this.setSendInviteDialogVisible(false);
    this.setDeleteDialogVisible(false);
    this.setInvitationDialogVisible(false);
  };
}

export default DialogStore;
