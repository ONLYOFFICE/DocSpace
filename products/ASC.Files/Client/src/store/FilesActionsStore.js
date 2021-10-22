import { makeAutoObservable } from "mobx";

import {
  removeFiles,
  deleteFile,
  deleteFolder,
  finalizeVersion,
  lockFile,
  downloadFiles,
  markAsRead,
  checkFileConflicts,
  removeShareFiles,
  getSubfolders,
  emptyTrash,
} from "@appserver/common/api/files";
import { ConflictResolveType, FileAction } from "@appserver/common/constants";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";
import toastr from "studio/toastr";

class FilesActionStore {
  authStore;
  uploadDataStore;
  treeFoldersStore;
  filesStore;
  selectedFolderStore;
  settingsStore;
  dialogsStore;
  mediaViewerDataStore;

  constructor(
    authStore,
    uploadDataStore,
    treeFoldersStore,
    filesStore,
    selectedFolderStore,
    settingsStore,
    dialogsStore,
    mediaViewerDataStore
  ) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.uploadDataStore = uploadDataStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
    this.settingsStore = settingsStore;
    this.dialogsStore = dialogsStore;
    this.mediaViewerDataStore = mediaViewerDataStore;
  }

  isMediaOpen = () => {
    const { visible, setMediaViewerData, playlist } = this.mediaViewerDataStore;
    if (visible && playlist.length === 1) {
      setMediaViewerData({ visible: false, id: null });
    }
  };

  updateCurrentFolder = () => {
    const {
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    let updatedFolder = this.selectedFolderStore.id;

    if (this.dialogsStore.isFolderActions) {
      updatedFolder = this.selectedFolderStore.parentId;
    }

    const { filter, fetchFiles } = this.filesStore;
    fetchFiles(updatedFolder, filter, true, true).finally(() => {
      this.dialogsStore.setIsFolderActions(false);
      return setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
    });
  };

  deleteAction = async (
    translations,
    newSelection = null,
    withoutDialog = false
  ) => {
    const { isRecycleBinFolder, isPrivacyFolder } = this.treeFoldersStore;

    const selection = newSelection ? newSelection : this.filesStore.selection;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });

    const deleteAfter = false; //Delete after finished TODO: get from settings
    const immediately = isRecycleBinFolder || isPrivacyFolder ? true : false; //Don't move to the Recycle Bin

    let folderIds = [];
    let fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst || selection[i].contentLength) {
        fileIds.push(selection[i].id);
      } else {
        folderIds.push(selection[i].id);
      }
      i++;
    }

    if (this.dialogsStore.isFolderActions && withoutDialog) {
      folderIds = [];
      fileIds = [];

      folderIds.push(selection[0]);
    }

    if (folderIds.length || fileIds.length) {
      this.isMediaOpen();

      try {
        await removeFiles(folderIds, fileIds, deleteAfter, immediately).then(
          async (res) => {
            const data = res[0] ? res[0] : null;
            const pbData = {
              icon: "trash",
              label: translations.deleteOperation,
            };
            await this.uploadDataStore.loopFilesOperations(data, pbData);
            this.updateCurrentFolder();
          }
        );
      } catch (err) {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  emptyTrash = async (translations) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });

    try {
      await emptyTrash().then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "trash",
          label: translations.deleteOperation,
        };
        await this.uploadDataStore.loopFilesOperations(data, pbData);
        this.updateCurrentFolder();
      });
    } catch (err) {
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    }
  };

  downloadFilesOperation = () => {};

  downloadFiles = async (fileConvertIds, folderIds, label) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    try {
      await downloadFiles(fileConvertIds, folderIds).then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "file",
          label,
        };

        const item =
          data?.finished && data?.url
            ? data
            : await this.uploadDataStore.loopFilesOperations(data, pbData);

        if (item.url) {
          window.location.href = item.url;
        } else {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
        }

        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
    } catch (err) {
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    }
  };

  downloadAction = (label, folderId) => {
    const { bufferSelection } = this.filesStore;

    const selection = this.filesStore.selection.length
      ? this.filesStore.selection
      : [bufferSelection];

    let fileIds = [];
    let folderIds = [];
    const items = [];

    if (selection.length === 1 && selection[0].fileExst && !folderId) {
      window.open(selection[0].viewUrl, "_self");
      return Promise.resolve();
    }

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
        items.push({ id: item.id, fileExst: item.fileExst });
      } else {
        folderIds.push(item.id);
        items.push({ id: item.id });
      }
    }

    if (this.dialogsStore.isFolderActions) {
      fileIds = [];
      folderIds = [];

      folderIds.push(bufferSelection);
      this.dialogsStore.setIsFolderActions(false);
    }

    return this.downloadFiles(fileIds, folderIds, label);
  };

  editCompleteAction = async (id, selectedItem, isCancelled = false) => {
    const {
      filter,
      folders,
      files,
      fileActionStore,
      fetchFiles,
      setIsLoading,
    } = this.filesStore;
    const { type, setAction } = fileActionStore;
    const { treeFolders, setTreeFolders } = this.treeFoldersStore;

    const items = [...folders, ...files];
    const item = items.find((o) => o.id === id && !o.fileExst); //TODO: maybe need files find and folders find, not at one function?
    if (type === FileAction.Create || type === FileAction.Rename) {
      setIsLoading(true);

      if (!isCancelled) {
        const data = await fetchFiles(this.selectedFolderStore.id, filter);
        const newItem = (item && item.id) === -1 ? null : item; //TODO: not add new folders?
        if (!selectedItem.fileExst && !selectedItem.contentLength) {
          const path = data.selectedFolder.pathParts;
          const folders = await getSubfolders(this.selectedFolderStore.id);
          loopTreeFolders(path, treeFolders, folders, null, newItem);
          setTreeFolders(treeFolders);
        }
      }
      setAction({ type: null, id: null, extension: null });
      setIsLoading(false);
      type === FileAction.Rename &&
        this.onSelectItem({
          id: selectedItem.id,
          isFolder: selectedItem.isFolder,
        });
    }
  };

  onSelectItem = ({ id, isFolder }) => {
    const { setBufferSelection, selected, setSelected } = this.filesStore;
    /* selected === "close" &&  */ setSelected("none");

    if (!id) return;

    const item = this.filesStore[isFolder ? "folders" : "files"].find(
      (elm) => elm.id === id
    );

    if (item) {
      item.isFolder = isFolder;
      setBufferSelection(item);
    }
  };

  deleteItemAction = async (itemId, translations, isFile, isThirdParty) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;
    if (
      this.settingsStore.confirmDelete ||
      this.treeFoldersStore.isPrivacyFolder ||
      isThirdParty
    ) {
      this.dialogsStore.setDeleteDialogVisible(true);
    } else {
      setSecondaryProgressBarData({
        icon: "trash",
        visible: true,
        percent: 0,
        label: translations.deleteOperation,
        alert: false,
      });

      try {
        await this.deleteItemOperation(isFile, itemId, translations);
      } catch (err) {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  deleteItemOperation = (isFile, itemId, translations) => {
    const pbData = {
      icon: "trash",
      label: translations.deleteOperation,
    };

    if (isFile) {
      this.isMediaOpen();
      return deleteFile(itemId)
        .then(async (res) => {
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);
          this.updateCurrentFolder();
        })
        .then(() => toastr.success(translations.successRemoveFile));
    } else {
      return deleteFolder(itemId)
        .then(async (res) => {
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);
          this.updateCurrentFolder();
        })
        .then(() => toastr.success(translations.successRemoveFolder));
    }
  };

  unsubscribeAction = async (fileIds, folderIds) => {
    const { setUnsubscribe } = this.dialogsStore;
    const { filter, fetchFiles } = this.filesStore;

    return removeShareFiles(fileIds, folderIds)
      .then(() => setUnsubscribe(false))
      .then(() => fetchFiles(this.selectedFolderStore.id, filter, true, true));
  };

  lockFileAction = (id, locked) => {
    const { setFile } = this.filesStore;
    return lockFile(id, locked).then((res) => setFile(res));
  };

  finalizeVersionAction = (id) => {
    const { fetchFiles, setIsLoading } = this.filesStore;

    setIsLoading(true);

    return finalizeVersion(id, 0, false)
      .then(() => {
        fetchFiles(this.selectedFolderStore.id, this.filesStore.filter);
      })
      .finally(() => setIsLoading(false));
  };

  duplicateAction = (item, label) => {
    const {
      setSecondaryProgressBarData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    const folderIds = [];
    const fileIds = [];
    item.fileExst ? fileIds.push(item.id) : folderIds.push(item.id);
    const conflictResolveType = ConflictResolveType.Duplicate;
    const deleteAfter = false; //TODO: get from settings

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    return this.uploadDataStore.copyToAction(
      this.selectedFolderStore.id,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );
  };

  setFavoriteAction = (action, id) => {
    const {
      markItemAsFavorite,
      removeItemFromFavorite,
      fetchFavoritesFolder,
      getFileInfo,
      setSelected,
    } = this.filesStore;

    const items = Array.isArray(id) ? id : [id];

    //let data = selection.map(item => item.id)
    switch (action) {
      case "mark":
        return markItemAsFavorite([id]).then(() => getFileInfo(id));

      case "remove":
        return removeItemFromFavorite(items)
          .then(() => {
            return this.treeFoldersStore.isFavoritesFolder
              ? fetchFavoritesFolder(this.selectedFolderStore.id)
              : getFileInfo(id);
          })
          .then(() => setSelected("close"));
      default:
        return;
    }
  };

  selectRowAction = (checked, file) => {
    const {
      selected,
      setSelected,
      selectFile,
      deselectFile,
      setBufferSelection,
    } = this.filesStore;
    //selected === "close" && setSelected("none");
    setBufferSelection(null);
    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  openLocationAction = (locationId, isFolder) => {
    const locationFilter = isFolder ? this.filesStore.filter : null;
    this.filesStore.setBufferSelection(null);
    return this.filesStore.fetchFiles(locationId, locationFilter);
    /*.then(() =>
      //isFolder ? null : this.selectRowAction(!checked, item)
    );*/
  };

  setThirdpartyInfo = (providerKey) => {
    const { setConnectDialogVisible, setConnectItem } = this.dialogsStore;
    const { providers, capabilities } = this.settingsStore.thirdPartyStore;
    const provider = providers.find((x) => x.provider_key === providerKey);
    const capabilityItem = capabilities.find((x) => x[0] === providerKey);
    const capability = {
      title: capabilityItem ? capabilityItem[0] : provider.customer_title,
      link: capabilityItem ? capabilityItem[1] : " ",
    };

    setConnectDialogVisible(true);
    setConnectItem({ ...provider, ...capability });
  };

  setNewBadgeCount = (item) => {
    const { getRootFolder, updateRootBadge } = this.treeFoldersStore;
    const { updateFileBadge, updateFolderBadge } = this.filesStore;
    const { rootFolderType, fileExst, id } = item;

    const count = item.new ? item.new : 1;
    const rootFolder = getRootFolder(rootFolderType);
    updateRootBadge(rootFolder.id, count);

    if (fileExst) updateFileBadge(id);
    else updateFolderBadge(id, item.new);
  };

  markAsRead = (folderIds, fileId, item) => {
    const {
      setSecondaryProgressBarData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "file",
      label: "", //TODO: add translation if need "MarkAsRead": "Mark all as read",
      percent: 0,
      visible: true,
    });

    return markAsRead(folderIds, fileId)
      .then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = { icon: "file" };
        await this.uploadDataStore.loopFilesOperations(data, pbData);
      })
      .then(() => item && this.setNewBadgeCount(item))
      .catch((err) => toastr.error(err));
  };

  moveDragItems = (destFolderId, folderTitle, translations) => {
    const folderIds = [];
    const fileIds = [];
    const deleteAfter = false;

    const { selection } = this.filesStore;
    const { isRootFolder } = this.selectedFolderStore;
    const {
      isShareFolder,
      isCommonFolder,
      isFavoritesFolder,
      isRecentFolder,
    } = this.treeFoldersStore;
    const isCopy =
      isShareFolder ||
      isFavoritesFolder ||
      isRecentFolder ||
      (!this.authStore.isAdmin && isCommonFolder);

    const operationData = {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      translations,
      folderTitle,
      isCopy,
    };

    const {
      setThirdPartyMoveDialogVisible,
      setDestFolderId,
    } = this.dialogsStore;

    for (let item of selection) {
      if (item.providerKey && !isRootFolder) {
        setDestFolderId(destFolderId);
        return setThirdPartyMoveDialogVisible(true);
      }

      if (!item.isFolder) {
        fileIds.push(item.id);
      } else {
        if (item.providerKey && isRootFolder) continue;
        folderIds.push(item.id);
      }
    }

    if (!folderIds.length && !fileIds.length) return;
    this.checkOperationConflict(operationData);
  };

  checkOperationConflict = async (operationData) => {
    const { destFolderId, folderIds, fileIds } = operationData;
    const {
      setConflictResolveDialogData,
      setConflictResolveDialogVisible,
      setConflictResolveDialogItems,
    } = this.dialogsStore;

    let conflicts;

    try {
      conflicts = await checkFileConflicts(destFolderId, folderIds, fileIds);
    } catch (err) {
      this.filesStore.setBufferSelection(null);
      return toastr.error(err.message ? err.message : err);
    }

    if (conflicts.length) {
      setConflictResolveDialogItems(conflicts);
      setConflictResolveDialogData(operationData);
      setConflictResolveDialogVisible(true);
    } else {
      try {
        await this.uploadDataStore.itemOperationToFolder(operationData);
      } catch (err) {
        this.filesStore.setBufferSelection(null);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  isAvailableOption = (option) => {
    const {
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
    } = this.treeFoldersStore;
    const {
      isAccessedSelected,
      isWebEditSelected,
      isThirdPartyRootSelection,
      hasSelection,
    } = this.filesStore;
    const { personal } = this.authStore.settingsStore;
    const { userAccess } = this.filesStore;

    switch (option) {
      case "share":
        return isAccessedSelected && !personal; //isFavoritesFolder ||isRecentFolder
      case "copy":
      case "download":
        return hasSelection;
      case "downloadAs":
        return isWebEditSelected && hasSelection;
      case "moveTo":
        return (
          !isThirdPartyRootSelection &&
          hasSelection &&
          isAccessedSelected &&
          !isRecentFolder &&
          !isFavoritesFolder
        );

      case "delete":
        const deleteCondition =
          !isThirdPartyRootSelection && hasSelection && isAccessedSelected;

        return isCommonFolder ? userAccess && deleteCondition : deleteCondition;
    }
  };

  convertToArray = (itemsCollection) => {
    const result = Array.from(itemsCollection.values()).filter((item) => {
      return item != null;
    });

    itemsCollection.clear();

    return result;
  };

  getOption = (option, t) => {
    const {
      setSharingPanelVisible,
      setDownloadDialogVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
    } = this.dialogsStore;

    switch (option) {
      case "share":
        if (!this.isAvailableOption("share")) return null;
        else
          return {
            label: t("Share"),
            onClick: () => setSharingPanelVisible(true),
          };

      case "copy":
        if (!this.isAvailableOption("copy")) return null;
        else
          return {
            label: t("Translations:Copy"),
            onClick: () => setCopyPanelVisible(true),
          };

      case "download":
        if (!this.isAvailableOption("download")) return null;
        else
          return {
            label: t("Common:Download"),
            onClick: () =>
              this.downloadAction(
                t("Translations:ArchivingData")
              ).catch((err) => toastr.error(err)),
          };

      case "downloadAs":
        if (!this.isAvailableOption("downloadAs")) return null;
        else
          return {
            label: t("Translations:DownloadAs"),
            onClick: () => setDownloadDialogVisible(true),
          };

      case "moveTo":
        if (!this.isAvailableOption("moveTo")) return null;
        else
          return {
            label: t("MoveTo"),
            onClick: () => setMoveToPanelVisible(true),
          };

      case "delete":
        if (!this.isAvailableOption("delete")) return null;
        else
          return {
            label: t("Common:Delete"),
            onClick: () => {
              if (this.settingsStore.confirmDelete) {
                setDeleteDialogVisible(true);
              } else {
                const translations = {
                  deleteOperation: t("Translations:DeleteOperation"),
                  deleteFromTrash: t("Translations:DeleteFromTrash"),
                  deleteSelectedElem: t("Translations:DeleteSelectedElem"),
                };

                this.deleteAction(translations).catch((err) =>
                  toastr.error(err)
                );
              }
            },
          };
    }
  };

  getAnotherFolderOptions = (itemsCollection, t) => {
    const share = this.getOption("share", t);
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const moveTo = this.getOption("moveTo", t);
    const copy = this.getOption("copy", t);
    const deleteOption = this.getOption("delete", t);

    itemsCollection
      .set("share", share)
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("moveTo", moveTo)
      .set("copy", copy)
      .set("delete", deleteOption);

    return this.convertToArray(itemsCollection);
  };

  getRecentFolderOptions = (itemsCollection, t) => {
    const share = this.getOption("share", t);
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);

    itemsCollection
      .set("share", share)
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy);
    return this.convertToArray(itemsCollection);
  };

  getShareFolderOptions = (itemsCollection, t) => {
    const { setDeleteDialogVisible, setUnsubscribe } = this.dialogsStore;

    const share = this.getOption("share", t);
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);

    itemsCollection
      .set("share", share)
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy)
      .set("delete", {
        label: t("RemoveFromList"),
        onClick: () => {
          setUnsubscribe(true);
          setDeleteDialogVisible(true);
        },
      });
    return this.convertToArray(itemsCollection);
  };

  getPrivacyFolderOption = (itemsCollection, t) => {
    const moveTo = this.getOption("moveTo", t);
    const deleteOption = this.getOption("delete", t);
    const download = this.getOption("download", t);

    itemsCollection
      .set("download", download)
      .set("moveTo", moveTo)

      .set("delete", deleteOption);
    return this.convertToArray(itemsCollection);
  };

  getFavoritesFolderOptions = (itemsCollection, t) => {
    const { selection } = this.filesStore;

    const share = this.getOption("share", t);
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);

    itemsCollection
      .set("share", share)
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy)
      .set("delete", {
        label: t("Common:Delete"),
        alt: t("RemoveFromFavorites"),
        onClick: () => {
          const items = selection.map((item) => item.id);
          this.setFavoriteAction("remove", items)
            .then(() => toastr.success(t("RemovedFromFavorites")))
            .catch((err) => toastr.error(err));
        },
      });
    return this.convertToArray(itemsCollection);
  };

  getRecycleBinFolderOptions = (itemsCollection, t) => {
    const {
      setEmptyTrashDialogVisible,
      setMoveToPanelVisible,
    } = this.dialogsStore;

    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const deleteOption = this.getOption("delete", t);

    itemsCollection
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("restore", {
        label: t("Translations:Restore"),
        onClick: () => setMoveToPanelVisible(true),
      })
      .set("delete", deleteOption)
      .set("emptyRecycleBin", {
        label: t("EmptyRecycleBin"),
        onClick: () => setEmptyTrashDialogVisible(true),
      });
    return this.convertToArray(itemsCollection);
  };
  getHeaderMenu = (t) => {
    const {
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,
      isPrivacyFolder,
      isShareFolder,
    } = this.treeFoldersStore;

    let itemsCollection = new Map();

    if (isRecycleBinFolder)
      return this.getRecycleBinFolderOptions(itemsCollection, t);

    if (isFavoritesFolder)
      return this.getFavoritesFolderOptions(itemsCollection, t);

    if (isPrivacyFolder) return this.getPrivacyFolderOption(itemsCollection, t);

    if (isShareFolder) return this.getShareFolderOptions(itemsCollection, t);

    if (isRecentFolder) return this.getRecentFolderOptions(itemsCollection, t);

    return this.getAnotherFolderOptions(itemsCollection, t);
  };
}

export default FilesActionStore;
