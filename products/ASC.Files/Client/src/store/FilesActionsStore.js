import { makeAutoObservable } from "mobx";
import uploadDataStore from "./UploadDataStore";
import treeFoldersStore from "./TreeFoldersStore";
import filesStore from "./FilesStore";
import selectedFolderStore from "./SelectedFolderStore";
import initFilesStore from "./InitFilesStore";

import { removeFiles, getProgress } from "@appserver/common/api/files";
import { FileAction } from "@appserver/common/constants";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";

const { secondaryProgressDataStore } = uploadDataStore;

class FilesActionStore {
  constructor() {
    makeAutoObservable(this);
  }

  deleteAction = (translations) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
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
    const { filter, fetchFiles } = filesStore;
    const isRecycleBin = treeFoldersStore.isRecycleBinFolder;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const successMessage = isRecycleBin
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
            if (!isRecycleBin) {
              const path = data.selectedFolder.pathParts.slice(0);
              const newTreeFolders = treeFoldersStore.treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              treeFoldersStore.setTreeFolders(newTreeFolders);
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

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

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
    const { fetchFiles, filter, folders, files, fileActionStore } = filesStore;
    const { type, setAction } = fileActionStore;
    const { treeFolders, setTreeFolders } = treeFoldersStore;
    const { setIsLoading } = initFilesStore;

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
}

export default new FilesActionStore();
