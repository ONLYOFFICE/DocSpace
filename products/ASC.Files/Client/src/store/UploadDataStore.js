import { makeAutoObservable } from "mobx";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";
import uniqueid from "lodash/uniqueId";
import throttle from "lodash/throttle";
import sumBy from "lodash/sumBy";
import { ConflictResolveType } from "@appserver/common/constants";
//import toastr from "studio/toastr";
import {
  getFolder,
  getFileInfo,
  getProgress,
  uploadFile,
  convertFile,
  startUploadSession,
  getFileConversationProgress,
  copyToFolder,
  moveToFolder,
} from "@appserver/common/api/files";

const chunkSize = 1024 * 1023; //~0.999mb

class UploadDataStore {
  formatsStore;
  treeFoldersStore;
  selectedFolderStore;
  filesStore;
  secondaryProgressDataStore;
  primaryProgressDataStore;
  dialogsStore;

  files = [];
  filesSize = 0;
  convertFiles = [];
  convertFilesSize = 0;
  uploadStatus = null;
  uploadToFolder = null;
  uploadedFiles = 0;
  percent = 0;
  uploaded = true;

  uploadPanelVisible = false;

  selectedUploadFile = [];

  constructor(
    formatsStore,
    treeFoldersStore,
    selectedFolderStore,
    filesStore,
    secondaryProgressDataStore,
    primaryProgressDataStore,
    dialogsStore
  ) {
    makeAutoObservable(this);
    this.formatsStore = formatsStore;
    this.treeFoldersStore = treeFoldersStore;
    this.selectedFolderStore = selectedFolderStore;
    this.filesStore = filesStore;
    this.secondaryProgressDataStore = secondaryProgressDataStore;
    this.primaryProgressDataStore = primaryProgressDataStore;
    this.dialogsStore = dialogsStore;
  }

  selectUploadedFile = (file) => {
    this.selectedUploadFile = file;
  };

  setUploadPanelVisible = (uploadPanelVisible) => {
    this.uploadPanelVisible = uploadPanelVisible;
  };

  setUploadData = (uploadData) => {
    const uploadDataItems = Object.keys(uploadData);
    for (let key of uploadDataItems) {
      if (key in this) {
        this[key] = uploadData[key];
      }
    }
  };

  updateUploadedFile = (id, info) => {
    const files = this.files.map((file) =>
      file.fileId === id ? { ...file, fileInfo: info } : file
    );
    this.files = files;
  };

  updateUploadedItem = async (id) => {
    const uploadedFileData = await getFileInfo(id);
    this.updateUploadedFile(id, uploadedFileData);
  };

  clearUploadData = () => {
    this.files = [];
    this.filesSize = 0;
    this.uploadStatus = null;
    this.uploadedFiles = 0;
    this.percent = 0;
    this.uploaded = true;
  };

  getUploadedFile = (id) => {
    return this.files.filter((f) => f.uniqueId === id);
  };

  cancelUpload = () => {
    let newFiles = [];

    for (let i = 0; i < this.files.length; i++) {
      if (this.files[i].fileId) {
        newFiles.push(this.files[i]);
      }
    }

    const newUploadData = {
      files: newFiles,
      filesSize: this.filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: 100,
      uploaded: true,
    };

    if (newUploadData.files.length === 0) this.setUploadPanelVisible(false);
    this.setUploadData(newUploadData);
  };

  cancelCurrentUpload = (id) => {
    const newFiles = this.files.filter((el) => el.uniqueId !== id);

    const newUploadData = {
      files: newFiles,
      filesSize: this.filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
      uploaded: false,
    };

    this.setUploadData(newUploadData);
  };

  getNewPercent = (uploadedSize, indexOfFile) => {
    const newTotalSize = sumBy(this.files, (f) => f.file.size);
    const totalUploadedFiles = this.files.filter((_, i) => i < indexOfFile);
    const totalUploadedSize = sumBy(totalUploadedFiles, (f) => f.file.size);
    const newPercent =
      ((uploadedSize + totalUploadedSize) / newTotalSize) * 100;

    /*console.log(
    `newPercent=${newPercent} (newTotalSize=${newTotalSize} totalUploadedSize=${totalUploadedSize} indexOfFile=${indexOfFile})`
  );*/

    return newPercent;
  };

  getConversationProgress = async (fileId) => {
    const promise = new Promise((resolve, reject) => {
      setTimeout(() => {
        try {
          getFileConversationProgress(fileId).then((res) => {
            //console.log(`getFileConversationProgress fileId:${fileId}`, res);
            resolve(res);
          });
        } catch (error) {
          console.error(error);
          reject(error);
        }
      }, 1000);
    });

    return promise;
  };

  convertFile = async (fileId, t, folderId) => {
    const { convertItemId, setConvertItemId } = this.dialogsStore;
    convertItemId && setConvertItemId(null);

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "file",
      //label: t("ConvertingLabel", { file: 0, totalFiles: total }),
      percent: 0,
      visible: true,
    });

    const data = await convertFile(fileId);
    if (data && data[0] && data[0].progress !== 100) {
      let progress = data[0].progress;
      let error = null;
      while (progress < 100) {
        const res = await this.getConversationProgress(fileId);

        progress = res && res[0] && res[0].progress;
        error = res && res[0] && res[0].error;
        if (error.length) {
          setSecondaryProgressBarData({
            icon: "file",
            visible: true,
            alert: true,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          this.refreshFiles(folderId, false);
          return;
        }
        if (progress === 100) {
          this.refreshFiles(folderId, false);
          break;
        } else {
          setSecondaryProgressBarData({
            icon: "file",
            //label: t("ConvertingLabel", { file: index + 1, totalFiles: total }),
            percent: progress,
            visible: true,
          });
        }
      }
    }
  };

  convertUploadedFiles = (t) => {
    this.files = [...this.files, ...this.convertFiles];

    if (this.uploaded) {
      const newUploadData = {
        files: this.convertFiles,
        filesSize: this.convertFilesSize,
        uploadedFiles: this.uploadedFiles,
        percent: this.percent,
        uploaded: false,
      };
      this.convertFiles = [];

      this.setUploadData(newUploadData);
      this.startUploadFiles(t);
    }
    this.convertFiles = [];
  };

  startUpload = (uploadFiles, folderId, t) => {
    const { canConvert } = this.formatsStore.docserviceStore;

    let newFiles = this.files;
    let filesSize = 0;
    let convertSize = 0;

    for (let index of Object.keys(uploadFiles)) {
      const file = uploadFiles[index];

      const parts = file.name.split(".");
      const ext = parts.length > 1 ? "." + parts.pop() : "";
      const needConvert = canConvert(ext);

      const newFile = {
        file: file,
        uniqueId: uniqueid("download_row-key_"),
        fileId: null,
        toFolderId: folderId,
        action: needConvert ? "convert" : "upload",
        error: file.size ? null : t("EmptyFile"),
        fileInfo: null,
        cancel: false,
      };

      needConvert ? this.convertFiles.push(newFile) : newFiles.push(newFile);

      filesSize += file.size;
      convertSize += file.size;
    }

    this.convertFilesSize = convertSize;

    if (this.convertFiles.length && !this.dialogsStore.convertDialogVisible)
      this.dialogsStore.setConvertDialogVisible(true);

    //const showConvertDialog = uploadStatus === "pending";

    const newUploadData = {
      files: newFiles,
      filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
      uploaded: false,
    };

    if (this.uploaded && newFiles.length) {
      this.setUploadData(newUploadData);
      this.startUploadFiles(t);
    }
  };

  refreshFiles = (folderId, needUpdateTree = true) => {
    const { setTreeFolders } = this.treeFoldersStore;
    if (
      this.selectedFolderStore.id === folderId &&
      window.location.pathname.indexOf("/history") === -1
    ) {
      return this.filesStore.fetchFiles(
        this.selectedFolderStore.id,
        this.filesStore.filter.clone(),
        false
      );
    } else if (needUpdateTree) {
      return getFolder(folderId, this.filesStore.filter.clone()).then(
        (data) => {
          const path = data.pathParts;
          const newTreeFolders = this.treeFoldersStore.treeFolders;
          const folders = data.folders;
          const foldersCount = data.count;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);
          setTreeFolders(newTreeFolders);
        }
      );
    }
  };

  throttleRefreshFiles = throttle((toFolderId) => {
    return this.refreshFiles(toFolderId).catch((err) => {
      console.log("RefreshFiles failed", err);
      return Promise.resolve();
    });
  }, 1000);

  uploadFileChunks = async (
    location,
    requestsDataArray,
    fileSize,
    indexOfFile,
    file,
    t
  ) => {
    const length = requestsDataArray.length;
    for (let index = 0; index < length; index++) {
      if (
        this.uploaded ||
        !this.files.some((f) => f.file === file) ||
        this.files[indexOfFile].cancel
      ) {
        return Promise.resolve();
      }

      const res = await uploadFile(location, requestsDataArray[index]);

      //console.log(`Uploaded chunk ${index}/${length}`, res);

      //let isLatestFile = indexOfFile === newFilesLength - 1;
      const fileId = res.data.data.id;

      const { uploaded } = res.data.data;

      const uploadedSize = uploaded ? fileSize : index * chunkSize;

      const newPercent = this.getNewPercent(uploadedSize, indexOfFile);

      const percentCurrentFile = (index / length) * 100;

      this.primaryProgressDataStore.setPrimaryProgressBarData({
        icon: "upload",
        percent: newPercent,
        visible: true,
        loadingFile: {
          uniqueId: this.files[indexOfFile].uniqueId,
          percent: percentCurrentFile,
        },
      });

      if (uploaded) {
        this.files[indexOfFile].fileId = fileId;
        this.files[indexOfFile].fileInfo = await getFileInfo(fileId);
        this.percent = newPercent;
        //setUploadData(uploadData);
      }
    }

    // All chuncks are uploaded

    const currentFile = this.files[indexOfFile];
    if (!currentFile) return Promise.resolve();
    const { fileId, toFolderId } = currentFile;

    if (currentFile.action === "convert") {
      this.convertFile(fileId, t, toFolderId);
      return Promise.resolve();
    } else {
      return this.throttleRefreshFiles(toFolderId);
    }
  };

  startUploadFiles = async (t) => {
    let files = this.files;

    if (files.length === 0 || this.filesSize === 0) {
      return this.finishUploadFiles();
    }

    const progressData = {
      visible: true,
      percent: this.percent,
      icon: "upload",
      alert: false,
    };

    this.primaryProgressDataStore.setPrimaryProgressBarData(progressData);

    let index = 0;
    let len = files.length;
    while (index < len) {
      await this.startSessionFunc(index, t);
      index++;

      files = this.files;
      len = files.length;
    }

    this.finishUploadFiles();
  };

  startSessionFunc = (indexOfFile, t) => {
    //console.log("START UPLOAD SESSION FUNC", uploadData);

    if (!this.uploaded && this.files.length === 0) {
      this.uploaded = true;
      //setUploadData(uploadData);
      return;
    }

    const item = this.files[indexOfFile];

    if (!item) {
      console.error("Empty files");
      return Promise.resolve();
    }

    const { file, toFolderId /*, action*/ } = item;
    const chunks = Math.ceil(file.size / chunkSize, chunkSize);
    const fileName = file.name;
    const fileSize = file.size;
    const relativePath = file.path
      ? file.path.slice(1, -file.name.length)
      : file.webkitRelativePath
      ? file.webkitRelativePath.slice(0, -file.name.length)
      : "";

    return startUploadSession(toFolderId, fileName, fileSize, relativePath)
      .then((res) => {
        const location = res.data.location;

        const requestsDataArray = [];

        let chunk = 0;

        while (chunk < chunks) {
          const offset = chunk * chunkSize;
          const formData = new FormData();
          formData.append("file", file.slice(offset, offset + chunkSize));
          requestsDataArray.push(formData);
          chunk++;
        }

        return { location, requestsDataArray, fileSize };
      })
      .then(({ location, requestsDataArray, fileSize }) => {
        this.primaryProgressDataStore.setPrimaryProgressBarData({
          icon: "upload",
          visible: true,
          percent: this.percent,
          loadingFile: {
            uniqueId: this.files[indexOfFile].uniqueId,
            percent: chunks < 2 ? 50 : 0,
          },
        });

        return this.uploadFileChunks(
          location,
          requestsDataArray,
          fileSize,
          indexOfFile,
          file,
          t
        );
      })
      .catch((err) => {
        if (this.files[indexOfFile] === undefined) {
          this.primaryProgressDataStore.setPrimaryProgressBarData({
            icon: "upload",
            percent: 100,
            visible: true,
            alert: true,
          });
          return Promise.resolve();
        }

        this.files[indexOfFile].error = err;

        //dispatch(setUploadData(uploadData));

        const newPercent = this.getNewPercent(fileSize, indexOfFile);

        this.primaryProgressDataStore.setPrimaryProgressBarData({
          icon: "upload",
          percent: newPercent,
          visible: true,
          alert: true,
        });

        return Promise.resolve();
      });
  };

  finishUploadFiles = () => {
    const totalErrorsCount = sumBy(this.files, (f) => (f.error ? 1 : 0));

    if (totalErrorsCount > 0) console.log("Errors: ", totalErrorsCount);

    const uploadData = {
      filesSize: 0,
      uploadStatus: null,
      uploadedFiles: 0,
      percent: 0,
      uploaded: true,
    };

    setTimeout(() => {
      !this.primaryProgressDataStore.alert &&
        this.primaryProgressDataStore.clearPrimaryProgressData();

      uploadData.files =
        this.uploadPanelVisible || this.primaryProgressDataStore.alert
          ? this.files
          : [];

      this.setUploadData(uploadData);
    }, TIMEOUT);
  };

  copyToAction = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      clearPrimaryProgressData,
    } = this.secondaryProgressDataStore;

    return copyToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopFilesOperations(id, destFolderId, true);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearPrimaryProgressData(), TIMEOUT);
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
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      clearPrimaryProgressData,
    } = this.secondaryProgressDataStore;

    return moveToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopFilesOperations(id, destFolderId, false);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearPrimaryProgressData(), TIMEOUT);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  itemOperationToFolder = (data) => {
    const {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      isCopy,
      translations,
    } = data;
    const conflictResolveType = data.conflictResolveType
      ? data.conflictResolveType
      : ConflictResolveType.Duplicate;

    this.secondaryProgressDataStore.setSecondaryProgressBarData({
      icon: isCopy ? "duplicate" : "move",
      visible: true,
      percent: 0,
      label: isCopy ? translations.copy : translations.move,
      alert: false,
    });

    isCopy
      ? this.copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        )
      : this.moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
  };

  loopFilesOperations = (id, destFolderId, isCopy) => {
    const label = this.secondaryProgressDataStore.label;
    const treeFolders = this.treeFoldersStore.treeFolders;

    const {
      clearSecondaryProgressData,
      setSecondaryProgressBarData,
    } = this.secondaryProgressDataStore;

    const loopOperation = () => {
      getProgress()
        .then((res) => {
          const currentItem = res.find((x) => x.id === id);
          if (currentItem && currentItem.progress !== 100) {
            this.secondaryProgressDataStore.setSecondaryProgressBarData({
              icon: isCopy ? "duplicate" : "move",
              label,
              percent: currentItem.progress,
              visible: true,
              alert: false,
            });

            setTimeout(() => loopOperation(), 1000);
          } else {
            this.secondaryProgressDataStore.setSecondaryProgressBarData({
              icon: isCopy ? "duplicate" : "move",
              label,
              percent: 100,
              visible: true,
              alert: false,
            });

            getFolder(destFolderId).then((data) => {
              let newTreeFolders = treeFolders;
              let path = data.pathParts.slice(0);
              let folders = data.folders;
              let foldersCount = data.current.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);

              if (!isCopy || destFolderId === this.selectedFolderStore.id) {
                this.filesStore
                  .fetchFiles(
                    this.selectedFolderStore.id,
                    this.filesStore.filter
                  )
                  .finally(() => {
                    setTimeout(
                      () =>
                        this.secondaryProgressDataStore.clearSecondaryProgressData(),
                      TIMEOUT
                    );
                  });
              } else {
                this.secondaryProgressDataStore.setSecondaryProgressBarData({
                  icon: "duplicate",
                  label,
                  percent: 100,
                  visible: true,
                  alert: false,
                });

                setTimeout(
                  () =>
                    this.secondaryProgressDataStore.clearSecondaryProgressData(),
                  TIMEOUT
                );
                this.treeFoldersStore.setTreeFolders(newTreeFolders);
              }
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

    loopOperation();
  };
}

export default UploadDataStore;
