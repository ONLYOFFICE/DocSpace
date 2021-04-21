import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import { TIMEOUT } from "../helpers/constants";
import { loopTreeFolders } from "../helpers/files-helpers";
import uniqueid from "lodash/uniqueId";
import throttle from "lodash/throttle";
import sumBy from "lodash/sumBy";
import { ConflictResolveType } from "@appserver/common/constants";

import { copyToFolder, moveToFolder } from "@appserver/common/api/files";

const chunkSize = 1024 * 1023; //~0.999mb

class UploadDataStore {
  formatsStore;
  treeFoldersStore;
  selectedFolderStore;
  filesStore;
  secondaryProgressDataStore;
  primaryProgressDataStore;

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
  convertDialogVisible = false;

  selectedUploadFile = [];

  constructor(
    formatsStore,
    treeFoldersStore,
    selectedFolderStore,
    filesStore,
    secondaryProgressDataStore,
    primaryProgressDataStore
  ) {
    makeAutoObservable(this);
    this.formatsStore = formatsStore;
    this.treeFoldersStore = treeFoldersStore;
    this.selectedFolderStore = selectedFolderStore;
    this.filesStore = filesStore;
    this.secondaryProgressDataStore = secondaryProgressDataStore;
    this.primaryProgressDataStore = primaryProgressDataStore;
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
    const uploadedFileData = await api.files.getFileInfo(id);
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

  setConvertDialogVisible = (convertDialogVisible) => {
    this.convertDialogVisible = convertDialogVisible;
  };

  getFilesToConvert = (files) => {
    const filesToConvert = files.filter((f) => f.action === "convert");
    return filesToConvert;
  };

  setDialogVisible = (t) => {
    this.setConvertDialogVisible(false);
    const label = t("UploadingLabel", {
      file: this.uploadedFiles,
      totalFiles: this.files.length,
    });

    if (this.uploadStatus === null) {
      this.primaryProgressDataStore.setPrimaryProgressBarData({
        icon: "upload",
        label,
        percent: 100,
        visible: true,
        alert: false,
      });
      this.uploadData.uploadedFiles = 0;
      this.uploadData.percent = 0;
      //setUploadData(uploadData);
    } else if (!this.files.length) {
      this.primaryProgressDataStore.clearPrimaryProgressData();
    } else {
      this.primaryProgressDataStore.setPrimaryProgressBarData({
        icon: "upload",
        label,
        percent: this.percent,
        visible: true,
        alert: false,
      });
      this.uploadData.uploadStatus = "cancel";
      //setUploadData(uploadData);
    }
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
          api.files.getFileConversationProgress(fileId).then((res) => {
            console.log(res);
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

  startConvertFiles = async (files, t) => {
    const total = files.length;
    this.setConvertDialogVisible(false);

    this.primaryProgressDataStore.setPrimaryProgressBarData({
      icon: "file",
      label: t("ConvertingLabel", {
        file: 0,
        totalFiles: total,
      }),
      percent: 0,
      visible: true,
    });

    for (let index = 0; index < total; index++) {
      const fileId = this.files[index].fileId;

      const data = await api.files.convertFile(fileId);

      if (data && data[0] && data[0].progress !== 100) {
        let progress = data[0].progress;
        let error = null;
        while (progress < 100) {
          const res = await this.getConversationProgress(fileId);

          progress = res && res[0] && res[0].progress;
          error = res && res[0] && res[0].error;
          if (error.length) {
            this.primaryProgressDataStore.setPrimaryProgressBarData({
              icon: "file",
              visible: true,
              alert: true,
            });
            return;
          }
          if (progress === 100) {
            break;
          } else {
            //TODO: calculate local progress
            // const percent = (progress) + (index / total) * 100;
            // dispatch(
            //   this.primaryProgressDataStore.setPrimaryProgressBarData({
            //     icon: "file",
            //     label: t("ConvertingLabel", {
            //       file: index + 1,
            //       totalFiles: total,
            //     }),
            //     percent: newPercent,
            //     visible: true,
            //   })
            // );
          }

          //setTimeout(() => { console.log("Wait for a second...") }, 1000);
        }
      }

      const newPercent = (index + 1 / total) * 100;

      this.primaryProgressDataStore.setPrimaryProgressBarData({
        icon: "file",
        label: t("ConvertingLabel", {
          file: index + 1,
          totalFiles: total,
        }),
        percent: newPercent,
        visible: true,
      });
    }
  };

  convertUploadedFiles = (t) => {
    const filesToConvert = this.getFilesToConvert(this.files);

    if (filesToConvert.length > 0) {
      this.startConvertFiles(filesToConvert, t).then(() =>
        this.finishUploadFiles()
      );
    } else {
      this.finishUploadFiles();
    }
  };

  startUpload = (uploadFiles, folderId, t) => {
    const { canConvert } = this.formatsStore.docserviceStore;

    let newFiles = this.files;
    let filesSize = 0;

    for (let index of Object.keys(uploadFiles)) {
      const file = uploadFiles[index];

      const parts = file.name.split(".");
      const ext = parts.length > 1 ? "." + parts.pop() : "";
      const needConvert = canConvert(ext);

      newFiles.push({
        file: file,
        uniqueId: uniqueid("download_row-key_"),
        fileId: null,
        toFolderId: folderId,
        action: needConvert ? "convert" : "upload",
        error: file.size ? null : t("EmptyFile"),
        fileInfo: null,
        cancel: false,
      });

      filesSize += file.size;
    }

    //const showConvertDialog = uploadStatus === "pending";

    const newUploadData = {
      files: newFiles,
      filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
      uploaded: false,
    };

    if (this.uploaded) {
      this.setUploadData(newUploadData);
      this.startUploadFiles(t);
    }
  };

  refreshFiles = (folderId) => {
    const { setTreeFolders } = this.treeFoldersStore;
    if (
      this.selectedFolderStore.id === folderId &&
      window.location.pathname.indexOf("/history") === -1
    ) {
      return this.filesStore
        .fetchFiles(
          this.selectedFolderStore.id,
          this.filesStore.filter.clone(),
          false
        )
        .then((data) => {
          const path = data.selectedFolder.pathParts;
          const newTreeFolders = this.treeFoldersStore.treeFolders;
          const folders = data.selectedFolder.folders;
          const foldersCount = data.selectedFolder.foldersCount;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);
          setTreeFolders(newTreeFolders);
        });
    } else {
      return api.files
        .getFolder(folderId, this.filesStore.filter.clone())
        .then((data) => {
          const path = data.pathParts;
          const newTreeFolders = this.treeFoldersStore.treeFolders;
          const folders = data.folders;
          const foldersCount = data.count;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);
          setTreeFolders(newTreeFolders);
        });
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

      const res = await api.files.uploadFile(
        location,
        requestsDataArray[index]
      );

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
        this.files[indexOfFile].fileInfo = await api.files.getFileInfo(fileId);
        this.percent = newPercent;
        //setUploadData(uploadData);
      }
    }

    // All chuncks are uploaded

    const currentFile = this.files[indexOfFile];

    if (!currentFile) return Promise.resolve();

    const { toFolderId } = currentFile;

    return this.throttleRefreshFiles(toFolderId);
  };

  startUploadFiles = async (t) => {
    let files = this.files;

    if (files.length === 0 || this.filesSize === 0)
      return this.finishUploadFiles();

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

    //TODO: Uncomment after fix conversation
    /*const filesToConvert = this.getFilesToConvert(files);
    if (filesToConvert.length > 0) {
      // Ask to convert options
      return dispatch(setConvertDialogVisible(true));
    }*/

    // All files has been uploaded and nothing to convert
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

    return api.files
      .startUploadSession(toFolderId, fileName, fileSize, relativePath)
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
      api.files
        .getProgress()
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

            api.files.getFolder(destFolderId).then((data) => {
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
                  .then((data) => {
                    if (!this.treeFoldersStore.isRecycleBinFolder) {
                      newTreeFolders = treeFolders;
                      path = data.selectedFolder.pathParts.slice(0);
                      folders = data.selectedFolder.folders;
                      foldersCount = data.selectedFolder.foldersCount;
                      loopTreeFolders(
                        path,
                        newTreeFolders,
                        folders,
                        foldersCount
                      );
                      this.treeFoldersStore.setTreeFolders(newTreeFolders);
                    }
                  })
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
