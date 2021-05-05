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
  thirdPartyDialogVisible = false;
  newFilesPanelVisible = false;
  conflictResolveDialogVisible = false;

  removeItem = null;
  connectItem = null;
  destFolderId = null;
  newFilesIds = null;
  conflictResolveDialogData = null;
  conflictResolveDialogItems = null;

  constructor() {
    makeAutoObservable(this);
  }

  setSharingPanelVisible = (sharingPanelVisible) => {
    this.sharingPanelVisible = sharingPanelVisible;
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

  setConnectItem = (connectItem) => {
    this.connectItem = connectItem;
  };

  setThirdPartyDialogVisible = (thirdPartyDialogVisible) => {
    this.thirdPartyDialogVisible = thirdPartyDialogVisible;
  };

  setDestFolderId = (destFolderId) => {
    this.destFolderId = destFolderId;
  };

  setNewFilesPanelVisible = (newFilesPanelVisible) => {
    if (!newFilesPanelVisible) this.setNewFilesIds(null);
    this.newFilesPanelVisible = newFilesPanelVisible;
  };

  setNewFilesIds = (newFilesIds) => {
    this.newFilesIds = newFilesIds;
  };

  setConflictResolveDialogVisible = (conflictResolveDialogVisible) => {
    this.conflictResolveDialogVisible = conflictResolveDialogVisible;
  };

  setConflictResolveDialogData = (data) => {
    this.conflictResolveDialogData = data;
  };

  setConflictResolveDialogItems = (items) => {
    this.conflictResolveDialogItems = items;
  };
}

export default new DialogsStore();
