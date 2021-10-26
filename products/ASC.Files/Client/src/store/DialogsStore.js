import { getNewFiles } from "@appserver/common/api/files";
import { makeAutoObservable } from "mobx";

class DialogsStore {
  treeFoldersStore;
  filesStore;
  selectedFolderStore;

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
  convertDialogVisible = false;
  isFolderActions = false;

  removeItem = null;
  connectItem = null;
  destFolderId = null;
  newFilesIds = null;
  newFiles = null;
  conflictResolveDialogData = null;
  conflictResolveDialogItems = null;
  removeMediaItem = null;
  unsubscribe = null;
  convertItem = null;

  constructor(treeFoldersStore, filesStore, selectedFolderStore) {
    makeAutoObservable(this);

    this.treeFoldersStore = treeFoldersStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
  }

  setSharingPanelVisible = (sharingPanelVisible) => {
    this.sharingPanelVisible = sharingPanelVisible;
  };

  setIsFolderActions = (isFolderActions) => {
    this.isFolderActions = isFolderActions;
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

  setNewFilesPanelVisible = async (visible, newIds, item) => {
    const id = newIds && newIds[newIds.length - 1];
    let newFilesPanelVisible = visible;

    if (visible) {
      const files = await getNewFiles(id);
      if (files && files.length) {
        this.setNewFiles(files);
        this.setNewFilesIds(newIds);
      } else {
        newFilesPanelVisible = false;
        const {
          getRootFolder,
          updateRootBadge,
          treeFolders,
        } = this.treeFoldersStore;
        const { updateFolderBadge, updateFoldersBadge } = this.filesStore;

        if (item) {
          const { rootFolderType, id } = item;
          const rootFolder = getRootFolder(rootFolderType);
          updateRootBadge(rootFolder.id, item.new);
          updateFolderBadge(id, item.new);
        } else {
          const rootFolder = treeFolders.find((x) => x.id === +newIds[0]);
          updateRootBadge(rootFolder.id, rootFolder.new);
          if (this.selectedFolderStore.id === rootFolder.id)
            updateFoldersBadge();
        }
      }
    } else {
      this.setNewFilesIds(null);
    }

    this.newFilesPanelVisible = newFilesPanelVisible;
  };

  setNewFilesIds = (newFilesIds) => {
    this.newFilesIds = newFilesIds;
  };

  setNewFiles = (files) => {
    this.newFiles = files;
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

  setRemoveMediaItem = (removeMediaItem) => {
    this.removeMediaItem = removeMediaItem;
  };

  setUnsubscribe = (unsubscribe) => {
    this.unsubscribe = unsubscribe;
  };

  setConvertDialogVisible = (visible) => {
    this.convertDialogVisible = visible;
  };

  setConvertItem = (item) => {
    this.convertItem = item;
  };
}

export default DialogsStore;
