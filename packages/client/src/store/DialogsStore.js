import { getNewFiles } from "@docspace/common/api/files";
import { FileAction } from "@docspace/common/constants";
import { makeAutoObservable } from "mobx";
import { Events } from "@docspace/common/constants";

class DialogsStore {
  authStore;
  treeFoldersStore;
  filesStore;
  selectedFolderStore;
  versionHistoryStore;

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
  selectFileDialogVisible = false;
  convertPasswordDialogVisible = false;
  isFolderActions = false;
  roomCreation = false;
  restoreAllPanelVisible = false;
  eventDialogVisible = false;

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
  formCreationInfo = null;
  saveThirdpartyResponse = null;

  constructor(
    authStore,
    treeFoldersStore,
    filesStore,
    selectedFolderStore,
    versionHistoryStore
  ) {
    makeAutoObservable(this);

    this.treeFoldersStore = treeFoldersStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
    this.authStore = authStore;
    this.versionHistoryStore = versionHistoryStore;
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
    !moveToPanelVisible && this.deselectActiveFiles();
    this.moveToPanelVisible = moveToPanelVisible;
  };

  setRestoreAllPanelVisible = (restoreAllPanelVisible) => {
    this.restoreAllPanelVisible = restoreAllPanelVisible;
  };

  setCopyPanelVisible = (copyPanelVisible) => {
    !copyPanelVisible && this.deselectActiveFiles();
    this.copyPanelVisible = copyPanelVisible;
  };

  setRoomCreation = (roomCreation) => {
    this.roomCreation = roomCreation;
  };

  setSaveThirdpartyResponse = (saveThirdpartyResponse) => {
    this.saveThirdpartyResponse = saveThirdpartyResponse;
  };

  setConnectDialogVisible = (connectDialogVisible) => {
    if (!connectDialogVisible) this.setConnectItem(null);
    this.connectDialogVisible = connectDialogVisible;
    if (!this.connectDialogVisible) this.setRoomCreation(false);
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
    !deleteDialogVisible && this.deselectActiveFiles();
    this.deleteDialogVisible = deleteDialogVisible;
  };

  setEventDialogVisible = (eventDialogVisible) => {
    this.eventDialogVisible = eventDialogVisible;
  };

  setDownloadDialogVisible = (downloadDialogVisible) => {
    !downloadDialogVisible && this.deselectActiveFiles();
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

  setNewFilesPanelVisible = async (visible, newId, item) => {
    const { pathParts } = this.selectedFolderStore;

    const id = visible && !newId ? item.id : newId;
    const newIds = newId ? [newId] : pathParts;
    item && pathParts.push(item.id);

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

  setConvertPasswordDialogVisible = (visible) => {
    this.convertPasswordDialogVisible = visible;
  };

  setFormCreationInfo = (item) => {
    this.formCreationInfo = item;
  };

  setConvertItem = (item) => {
    this.convertItem = item;
  };

  setSelectFileDialogVisible = (visible) => {
    this.selectFileDialogVisible = visible;
  };

  createMasterForm = async (fileInfo) => {
    let newTitle = fileInfo.title;
    newTitle = newTitle.substring(0, newTitle.lastIndexOf("."));

    const event = new Event(Events.CREATE);

    const payload = {
      extension: "docxf",
      id: -1,
      title: `${newTitle}.docxf`,
      templateId: fileInfo.id,
    };

    event.payload = payload;

    window.dispatchEvent(event);
  };

  get someDialogIsOpen() {
    return (
      this.sharingPanelVisible ||
      this.ownerPanelVisible ||
      this.moveToPanelVisible ||
      this.copyPanelVisible ||
      this.deleteThirdPartyDialogVisible ||
      this.connectDialogVisible ||
      this.thirdPartyMoveDialogVisible ||
      this.deleteDialogVisible ||
      this.downloadDialogVisible ||
      this.emptyTrashDialogVisible ||
      this.thirdPartyDialogVisible ||
      this.newFilesPanelVisible ||
      this.conflictResolveDialogVisible ||
      this.convertDialogVisible ||
      this.selectFileDialogVisible ||
      this.authStore.settingsStore.hotkeyPanelVisible ||
      this.versionHistoryStore.isVisible ||
      this.eventDialogVisible
    );
  }

  deselectActiveFiles = () => {
    this.filesStore.setSelected("none");
  };
}

export default DialogsStore;
