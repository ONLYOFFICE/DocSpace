import { makeAutoObservable } from "mobx";

import {
  removeFiles,
  getProgress,
  deleteFile,
  deleteFolder,
  finalizeVersion,
  lockFile,
  downloadFiles,
  markAsRead,
  checkFileConflicts,
  removeShareFiles,
  getSubfolders,
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

  deleteAction = (translations, newSelection = null) => {
    const { isRecycleBinFolder, isPrivacyFolder } = this.treeFoldersStore;

    const selection = newSelection ? newSelection : this.filesStore.selection;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    const deleteAfter = true; //Delete after finished TODO: get from settings
    const immediately = isRecycleBinFolder || isPrivacyFolder ? true : false; //Don't move to the Recycle Bin

    const folderIds = [];
    const fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst || selection[i].contentLength) {
        fileIds.push(selection[i].id);
      } else {
        folderIds.push(selection[i].id);
      }
      i++;
    }

    if (folderIds.length || fileIds.length) {
      this.isMediaOpen();
      return removeFiles(folderIds, fileIds, deleteAfter, immediately)
        .then((res) => {
          const id = res[0] && res[0].id ? res[0].id : null;
          const currentProcess = res.find((x) => x.id === id);
          setSecondaryProgressBarData({
            icon: "trash",
            visible: true,
            label: translations.deleteOperation,
            percent: currentProcess.progress,
            alert: false,
          });

          this.loopDeleteOperation(id, translations);
        })
        .catch((err) => {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        });
    }
  };

  loopDeleteOperation = (id, translations) => {
    const { filter, fetchFiles } = this.filesStore;
    const { isRecycleBinFolder, setTreeFolders } = this.treeFoldersStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    const successMessage = isRecycleBinFolder
      ? translations.deleteFromTrash
      : translations.deleteSelectedElem;

    getProgress()
      .then((res) => {
        const currentProcess = res.find((x) => x.id === id);
        if (currentProcess && currentProcess.progress !== 100) {
          setSecondaryProgressBarData({
            icon: "trash",
            percent: currentProcess.progress,
            label: translations.deleteOperation,
            visible: true,
            alert: false,
          });
          setTimeout(() => this.loopDeleteOperation(id, translations), 1000);
        } else {
          setSecondaryProgressBarData({
            icon: "trash",
            percent: 100,
            label: translations.deleteOperation,
            visible: true,
            alert: false,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          fetchFiles(this.selectedFolderStore.id, filter, true, true);
        }
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  loopFilesOperations = (id, translations) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    getProgress()
      .then((res) => {
        const currentProcess = res.find((x) => x.id === id);
        if (currentProcess && currentProcess.progress !== 100) {
          setSecondaryProgressBarData({
            icon: "file",
            percent: currentProcess.progress,
            label: "", //TODO: add translation if need "MarkAsRead": "Mark all as read",
            visible: true,
            alert: false,
          });
          setTimeout(() => this.loopFilesOperations(id, translations), 1000);
        } else {
          setSecondaryProgressBarData({
            icon: "file",
            percent: 100,
            label: "", //TODO: add translation if need "MarkAsRead": "Mark all as read",
            visible: true,
            alert: false,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        }
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  getDownloadProgress = (data, label) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;
    const url = data.url;

    return getProgress()
      .then((res) => {
        const currentItem = res.find((x) => x.id === data.id);
        if (!url) {
          setSecondaryProgressBarData({
            icon: "file",
            visible: true,
            percent: currentItem.progress,
            label,
            alert: false,
          });
          setTimeout(() => this.getDownloadProgress(currentItem, label), 1000);
        } else {
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          return (window.location.href = url);
        }
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  downloadAction = (label) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;
    const { selection } = this.filesStore;
    const fileIds = [];
    const folderIds = [];
    const items = [];

    if (selection.length === 1 && selection[0].fileExst) {
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

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    return downloadFiles(fileIds, folderIds)
      .then((res) => {
        this.getDownloadProgress(res[0], label);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
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
    const { setSelection, selected, setSelected } = this.filesStore;
    selected === "close" && setSelected("none");

    if (!id) return;

    const item = this.filesStore[isFolder ? "folders" : "files"].find(
      (elm) => elm.id === id
    );
    setSelection([item]);
  };

  deleteItemAction = (
    itemId,
    currentFolderId,
    translations,
    isFile,
    isThirdParty
  ) => {
    const {
      setSecondaryProgressBarData,
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
      isFile
        ? this.deleteFileAction(itemId, currentFolderId, translations)
        : this.deleteFolderAction(itemId, currentFolderId, translations);
    }
  };

  deleteFileAction = (fileId, currentFolderId, translations) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    this.isMediaOpen();
    return deleteFile(fileId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false, translations);
      })
      .then(() => toastr.success(translations.successRemoveFile))
      .catch((err) => {
        toastr.error(err);
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  unsubscribeAction = async (fileIds, folderIds) => {
    const { setUnsubscribe } = this.dialogsStore;
    const { filter, fetchFiles } = this.filesStore;

    return removeShareFiles(fileIds, folderIds)
      .then(() => setUnsubscribe(false))
      .then(() => fetchFiles(this.selectedFolderStore.id, filter, true, true));
  };

  deleteFolderAction = (folderId, currentFolderId, translations) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    return deleteFolder(folderId, currentFolderId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true, translations);
      })
      .then(() => toastr.success(translations.successRemoveFolder))
      .catch((err) => {
        toastr.error(err);
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  loopDeleteProgress = (id, folderId, isFolder, translations) => {
    const { filter, fetchFiles } = this.filesStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    getProgress().then((res) => {
      const deleteProgress = res.find((x) => x.id === id);
      if (deleteProgress && deleteProgress.progress !== 100) {
        setSecondaryProgressBarData({
          icon: "trash",
          visible: true,
          percent: deleteProgress.progress,
          label: translations.deleteOperation,
          alert: false,
        });
        setTimeout(
          () => this.loopDeleteProgress(id, folderId, isFolder, translations),
          1000
        );
      } else {
        setSecondaryProgressBarData({
          icon: "trash",
          visible: true,
          percent: 100,
          label: translations.deleteOperation,
          alert: false,
        });
        fetchFiles(folderId, filter, true, true)
          .catch((err) => {
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            });
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          })
          .finally(() =>
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT)
          );
      }
    });
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
    const { selected, setSelected, selectFile, deselectFile } = this.filesStore;
    //selected === "close" && setSelected("none");
    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  openLocationAction = (locationId, isFolder) => {
    const locationFilter = isFolder ? this.filesStore.filter : null;
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
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopFilesOperations(id);
      })
      .then(() => item && this.setNewBadgeCount(item))
      .catch((err) => toastr.error(err));
  };

  moveDragItems = (destFolderId, folderTitle, translations) => {
    const folderIds = [];
    const fileIds = [];
    const deleteAfter = true;

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
      return toastr.error(err.message);
    }

    if (conflicts.length) {
      setConflictResolveDialogItems(conflicts);
      setConflictResolveDialogData(operationData);
      setConflictResolveDialogVisible(true);
    } else {
      try {
        await this.uploadDataStore.itemOperationToFolder(operationData);
      } catch (err) {
        return toastr.error(err.message);
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
