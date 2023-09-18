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
  resetAuthDialogVisible = false;
  dataReassignmentDialogVisible = false;
  dataReassignmentDeleteProfile = false;
  isDeletingUserWithReassignment = false;

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

  setDataReassignmentDeleteProfile = (dataReassignmentDeleteProfile) => {
    this.dataReassignmentDeleteProfile = dataReassignmentDeleteProfile;
  };

  setIsDeletingUserWithReassignment = (isDeletingUserWithReassignment) => {
    this.isDeletingUserWithReassignment = isDeletingUserWithReassignment;
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

  setResetAuthDialogVisible = (visible) => {
    this.resetAuthDialogVisible = visible;
  };

  setDataReassignmentDialogVisible = (visible) => {
    this.dataReassignmentDialogVisible = visible;
  };

  closeDialogs = () => {
    this.setChangeOwnerDialogVisible(false);
    this.setDeleteSelfProfileDialogVisible(false);
    this.setDeleteProfileDialogVisible(false);
    this.setDialogData({});

    this.setChangeUserTypeDialogVisible(false);
    this.setChangeUserStatusDialogVisible(false);

    this.setSendInviteDialogVisible(false);
    this.setResetAuthDialogVisible(false);
  };
}

export default DialogStore;
