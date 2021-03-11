import { makeAutoObservable } from "mobx";

class DialogsStore {
  sharingPanelVisible = false;
  ownerPanelVisible = false;
  moveToPanelVisible = false;
  copyPanelVisible = false;
  deleteThirdPartyDialogVisible = false;
  connectDialogVisible = false;
  thirdPartyMoveDialogVisible = false;
  deleteDialogVisible = false;
  downloadDialogVisible = false;
  emptyTrashDialogVisible = false;

  connectItem = null;
  removeItem = null;
  shareItem = null;

  constructor() {
    makeAutoObservable(this);
  }

  setSharingPanelVisible = (sharingPanelVisible) => {
    this.sharingPanelVisible = sharingPanelVisible;
    !sharingPanelVisible && this.setShareItem(null);
  };

  setChangeOwnerPanelVisible = (ownerPanelVisible) => {
    this.ownerPanelVisible = ownerPanelVisible;
  };

  setMoveToPanelVisible = (moveToPanelVisible) => {
    this.moveToPanelVisible = moveToPanelVisible;
  };

  setCopyPanelVisible = (copyPanelVisible) => {
    this.copyPanelVisible = copyPanelVisible;
  };

  setConnectDialogVisible = (connectDialogVisible) => {
    if (!connectDialogVisible) this.setConnectItem(null);
    this.connectDialogVisible = connectDialogVisible;
  };

  setConnectItem = (connectItem) => {
    this.connectItem = connectItem;
  };

  setRemoveItem = (removeItem) => {
    this.removeItem = removeItem;
  };

  setDeleteThirdPartyDialogVisible = (deleteThirdPartyDialogVisible) => {
    this.deleteThirdPartyDialogVisible = deleteThirdPartyDialogVisible;
  };

  setThirdPartyMoveDialogVisible = (thirdPartyMoveDialogVisible) => {
    this.thirdPartyMoveDialogVisible = thirdPartyMoveDialogVisible;
  };

  setDeleteDialogVisible = (deleteDialogVisible) => {
    this.deleteDialogVisible = deleteDialogVisible;
  };

  setDownloadDialogVisible = (downloadDialogVisible) => {
    this.downloadDialogVisible = downloadDialogVisible;
  };

  setEmptyTrashDialogVisible = (emptyTrashDialogVisible) => {
    this.emptyTrashDialogVisible = emptyTrashDialogVisible;
  };

  setShareItem = (shareItem) => {
    this.shareItem = shareItem;
  };
}

export default new DialogsStore();
