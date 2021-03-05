import { makeAutoObservable } from "mobx";
import uploadDataStore from "./UploadDataStore";
import treeFoldersStore from "./TreeFoldersStore";
import filesStore from "./FilesStore";
import selectedFolderStore from "./SelectedFolderStore";
import initFilesStore from "./InitFilesStore";

import {
  removeFiles,
  getProgress,
  copyToFolder,
  deleteFile,
  deleteFolder,
  moveToFolder,
} from "@appserver/common/api/files";
import { FileAction } from "@appserver/common/constants";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";

//import toastr from "@appserver/components/toast";

const { fetchFiles } = filesStore;
const { setTreeFolders } = treeFoldersStore;
const { setIsLoading } = initFilesStore;
const { secondaryProgressDataStore, loopFilesOperations } = uploadDataStore;
const {
  setSecondaryProgressBarData,
  clearSecondaryProgressData,
} = secondaryProgressDataStore;

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

      removeFiles(folderIds, fileIds, deleteAfter, immediately)
        .then((res) => {
          const id = res[0] && res[0].id ? res[0].id : null;
          this.loopDeleteOperation(id, translations);
        })
        .catch((err) => {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          //toastr.error(err);
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
            //toastr.success(successMessage);
          });
        }
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

  getDownloadProgress = (data, label) => {
    const url = data.url;

    getProgress()
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
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  editCompleteAction = (id, isFolder /* selectedItem */) => {
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
          if (isFolder) {
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

          //uncomment if need to select item
          //type === FileAction.Rename && this.onSelectItem(selectedItem);

          // onSelectItem = (item) => {
          //   const { selected, setSelected, setSelection } = this.props;
          //   selected === "close" && setSelected("none");
          //   setSelection([item]);
          // };
        });
    }

    //this.setState({ editingId: null }, () => {
    //  setAction({type: null});
    //});
  };

  copyToAction = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    copyToFolder(
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
    deleteFile(fileId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false, translations);
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

  deleteFolderAction = (folderId, currentFolderId, translations) => {
    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });
    deleteFolder(folderId, currentFolderId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true, translations);
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
            //isFolder
            //  ? toastr.success(translations.folderRemoved)
            //  : toastr.success(translations.fileRemoved);
          })
          .catch((err) => {
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            });
            //toastr.error(err);
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          })
          .finally(() =>
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT)
          );
      }
    });
  };
}

export default new FilesActionStore();
