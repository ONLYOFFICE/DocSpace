import { makeAutoObservable } from "mobx";
import store from "studio/store";
import uploadDataStore from "./UploadDataStore";
import treeFoldersStore from "./TreeFoldersStore";
import filesStore from "./FilesStore";
import selectedFolderStore from "./SelectedFolderStore";
import initFilesStore from "./InitFilesStore";
import settingsStore from "./SettingsStore";
import dialogsStore from "./DialogsStore";

import {
  removeFiles,
  getProgress,
  copyToFolder,
  deleteFile,
  deleteFolder,
  moveToFolder,
  finalizeVersion,
  lockFile,
  downloadFiles,
  markAsRead,
} from "@appserver/common/api/files";
import { FileAction } from "@appserver/common/constants";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";

const { auth } = store;

const {
  fetchFiles,
  markItemAsFavorite,
  removeItemFromFavorite,
  fetchFavoritesFolder,
  getFileInfo,
  setSelected,
  selectFile,
  deselectFile,
  setSelection,
  setFile,
} = filesStore;
const { setTreeFolders } = treeFoldersStore;
const { setIsLoading } = initFilesStore;
const { secondaryProgressDataStore, loopFilesOperations } = uploadDataStore;
const {
  setSecondaryProgressBarData,
  clearSecondaryProgressData,
} = secondaryProgressDataStore;
const {
  setConnectDialogVisible,
  setConnectItem,
  setThirdPartyMoveDialogVisible,
  setDestFolderId,
} = dialogsStore;

class FilesActionStore {
  constructor() {
    makeAutoObservable(this);
  }

  deleteAction = (translations) => {
    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
    const { selection } = filesStore;

    const deleteAfter = true; //Delete after finished TODO: get from settings
    const immediately = isRecycleBinFolder || isPrivacyFolder ? true : false; //Don't move to the Recycle Bin

    const folderIds = [];
    const fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst) {
        fileIds.push(selection[i].id);
      } else {
        folderIds.push(selection[i].id);
      }
      i++;
    }

    if (folderIds.length || fileIds.length) {
      setSecondaryProgressBarData({
        icon: "trash",
        visible: true,
        label: translations.deleteOperation,
        percent: 0,
        alert: false,
      });

      return removeFiles(folderIds, fileIds, deleteAfter, immediately)
        .then((res) => {
          const id = res[0] && res[0].id ? res[0].id : null;
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
    const { filter } = filesStore;
    const { isRecycleBinFolder } = treeFoldersStore;
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
          fetchFiles(selectedFolderStore.id, filter).then((data) => {
            if (!isRecycleBinFolder) {
              const path = data.selectedFolder.pathParts.slice(0);
              const newTreeFolders = treeFoldersStore.treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              setTreeFolders(newTreeFolders);
            }
          });
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
    const { selection } = filesStore;
    const fileIds = [];
    const folderIds = [];
    const items = [];

    if (selection.length === 1 && selection[0].fileExst) {
      window.open(selection[0].viewUrl, "_blank");
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

  editCompleteAction = (id, selectedItem) => {
    const { filter, folders, files, fileActionStore } = filesStore;
    const { type, setAction } = fileActionStore;
    const { treeFolders } = treeFoldersStore;

    const items = [...folders, ...files];
    const item = items.find((o) => o.id === id && !o.fileExst); //TODO: maybe need files find and folders find, not at one function?
    if (type === FileAction.Create || type === FileAction.Rename) {
      setIsLoading(true);
      fetchFiles(selectedFolderStore.id, filter)
        .then((data) => {
          const newItem = (item && item.id) === -1 ? null : item; //TODO: not add new folders?
          if (!selectedItem.fileExst) {
            const path = data.selectedFolder.pathParts;
            const newTreeFolders = treeFolders;
            const folders = data.selectedFolder.folders;
            loopTreeFolders(path, newTreeFolders, folders, null, newItem);
            setTreeFolders(newTreeFolders);
          }
        })
        .finally(() => {
          setAction({ type: null, id: null, extension: null });
          setIsLoading(false);
          type === FileAction.Rename && this.onSelectItem(selectedItem);
        });
    }
  };

  onSelectItem = (item) => {
    filesStore.selected === "close" && setSelected("none");
    setSelection([item]);
  };

  copyToAction = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    return copyToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, true);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  moveToAction = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    moveToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, false);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  deleteFileAction = (fileId, currentFolderId, translations) => {
    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });
    return deleteFile(fileId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false, translations);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  deleteFolderAction = (folderId, currentFolderId, translations) => {
    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });
    return deleteFolder(folderId, currentFolderId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true, translations);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  loopDeleteProgress = (id, folderId, isFolder, translations) => {
    const { filter } = filesStore;
    const { treeFolders, isRecycleBinFolder } = treeFoldersStore;

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
        fetchFiles(folderId, filter)
          .then((data) => {
            if (!isRecycleBinFolder && isFolder) {
              const path = data.selectedFolder.pathParts.slice(0);
              const newTreeFolders = treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              setTreeFolders(newTreeFolders);
            }
          })
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
    return lockFile(id, locked).then((res) => setFile(res));
  };

  finalizeVersionAction = (id) => {
    setIsLoading(true);

    return finalizeVersion(id, 0, false)
      .then(() => {
        fetchFiles(selectedFolderStore.id, filesStore.filter);
      })
      .finally(() => setIsLoading(false));
  };

  duplicateAction = (item, label) => {
    const folderIds = [];
    const fileIds = [];
    item.fileExst ? fileIds.push(item.id) : folderIds.push(item.id);
    const conflictResolveType = 2; //Skip = 0, Overwrite = 1, Duplicate = 2 //TODO: get from settings
    const deleteAfter = false; //TODO: get from settings

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    return this.copyToAction(
      selectedFolderStore.id,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );
  };

  setFavoriteAction = (action, id) => {
    //let data = selection.map(item => item.id)
    switch (action) {
      case "mark":
        return markItemAsFavorite([id]).then(() => getFileInfo(id));

      case "remove":
        return removeItemFromFavorite([id])
          .then(() => {
            return treeFoldersStore.isFavoritesFolder
              ? fetchFavoritesFolder(selectedFolderStore.id)
              : getFileInfo(id);
          })
          .then(() => setSelected("close"));
      default:
        return;
    }
  };

  selectRowAction = (checked, file) => {
    filesStore.selected === "close" && setSelected("none");
    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  openLocationAction = (locationId, isFolder) => {
    const locationFilter = isFolder ? filesStore.filter : null;

    return fetchFiles(locationId, locationFilter).then(() =>
      //isFolder ? null : this.selectRowAction(!checked, item)
      isFolder ? null : this.selectRowAction(false, item)
    );
  };

  setThirdpartyInfo = (providerKey) => {
    const { providers, capabilities } = settingsStore.thirdPartyStore;
    const provider = providers.find((x) => x.provider_key === providerKey);
    const capabilityItem = capabilities.find((x) => x[0] === providerKey);
    const capability = {
      title: capabilityItem ? capabilityItem[0] : provider.customer_title,
      link: capabilityItem ? capabilityItem[1] : " ",
    };

    setConnectDialogVisible(true);
    setConnectItem({ ...provider, ...capability });
  };

  markAsRead = (folderIds, fileId) => {
    return markAsRead(folderIds, fileId);
  };

  moveDragItems = (destFolderId, label) => {
    const folderIds = [];
    const fileIds = [];
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2 TODO: get from settings
    const deleteAfter = true;

    const { selection } = filesStore;
    const { isRootFolder } = selectedFolderStore;
    const { isShareFolder, isCommonFolder } = treeFoldersStore;

    for (let item of selection) {
      if (item.providerKey && !isRootFolder) {
        setDestFolderId(destFolderId);
        return setThirdPartyMoveDialogVisible(true);
      }

      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        if (item.providerKey && isRootFolder) continue;
        folderIds.push(item.id);
      }
    }

    if (!folderIds.length && !fileIds.length) return;

    setSecondaryProgressBarData({
      icon: "move",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    if (auth.isAdmin) {
      if (isShareFolder) {
        this.copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        this.moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    } else {
      if (isShareFolder || isCommonFolder) {
        this.copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        this.moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    }
  };
}

export default new FilesActionStore();
