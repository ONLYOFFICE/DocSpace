import { makeAutoObservable, runInAction } from "mobx";
import { TIMEOUT } from "@docspace/client/src/helpers/filesConstants";
import uniqueid from "lodash/uniqueId";
import sumBy from "lodash/sumBy";
import { ConflictResolveType } from "@docspace/common/constants";
import {
  getFolder,
  getFileInfo,
  getFolderInfo,
  getProgress,
  uploadFile,
  convertFile,
  startUploadSession,
  getFileConversationProgress,
  copyToFolder,
  moveToFolder,
  fileCopyAs,
} from "@docspace/common/api/files";
import toastr from "@docspace/components/toast/toastr";
class UploadDataStore {
  authStore;
  treeFoldersStore;
  selectedFolderStore;
  filesStore;
  secondaryProgressDataStore;
  primaryProgressDataStore;
  dialogsStore;
  settingsStore;

  files = [];
  uploadedFilesHistory = [];
  filesSize = 0;
  tempConversionFiles = [];
  filesToConversion = [];
  convertFilesSize = 0;
  uploadToFolder = null;
  uploadedFiles = 0;
  percent = 0;
  conversionPercent = 0;
  uploaded = true;
  converted = true;
  uploadPanelVisible = false;
  selectedUploadFile = [];
  errors = 0;

  isUploading = false;
  isUploadingAndConversion = false;

  constructor(
    authStore,
    treeFoldersStore,
    selectedFolderStore,
    filesStore,
    secondaryProgressDataStore,
    primaryProgressDataStore,
    dialogsStore,
    settingsStore
  ) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.treeFoldersStore = treeFoldersStore;
    this.selectedFolderStore = selectedFolderStore;
    this.filesStore = filesStore;
    this.secondaryProgressDataStore = secondaryProgressDataStore;
    this.primaryProgressDataStore = primaryProgressDataStore;
    this.dialogsStore = dialogsStore;
    this.settingsStore = settingsStore;
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
    this.filesToConversion = [];
    this.uploadedFilesHistory = [];
    this.filesSize = 0;
    this.uploadedFiles = 0;
    this.percent = 0;
    this.conversionPercent = 0;
    this.uploaded = true;
    this.converted = true;
    this.errors = 0;

    this.isUploadingAndConversion = false;
    this.isUploading = false;
  };

  removeFileFromList = (id) => {
    this.files = this.files.filter((obj) => {
      return obj.fileId !== id;
    });
  };

  clearUploadedFiles = () => {
    const uploadData = {
      filesSize: 0,
      uploadedFiles: 0,
      percent: 0,
      files: this.files.filter((x) => x.action !== "uploaded"),
    };

    this.isUploadingAndConversion = false;
    this.isUploading = false;

    this.setUploadData(uploadData);
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
      converted: true,
    };

    const newHistory = this.uploadedFilesHistory.filter(
      (el) => el.action === "uploaded"
    );

    if (newUploadData.files.length === 0) this.setUploadPanelVisible(false);
    this.setUploadData(newUploadData);
    this.uploadedFilesHistory = newHistory;
  };

  cancelConversion = () => {
    let newFiles = [];

    for (let i = 0; i < this.files.length; i++) {
      const file = this.files[i];
      if (file.action === "converted" || file.error || file.inConversion) {
        newFiles.push(this.files[i]);
      }
    }

    const newUploadData = {
      files: newFiles,
      filesToConversion: [],
      filesSize: this.filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: 100,
      uploaded: true,
      converted: true,
    };

    if (newUploadData.files.length === 0) this.setUploadPanelVisible(false);
    this.setUploadData(newUploadData);
  };

  cancelCurrentUpload = (id) => {
    const newFiles = this.files.filter((el) => el.uniqueId !== id);
    const uploadedFilesHistory = this.uploadedFilesHistory.filter(
      (el) => el.uniqueId !== id
    );

    const newUploadData = {
      files: newFiles,
      uploadedFilesHistory,
      filesSize: this.filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
      uploaded: false,
    };

    this.setUploadData(newUploadData);
  };

  cancelCurrentFileConversion = (fileId) => {
    const { convertItem, setConvertItem } = this.dialogsStore;
    convertItem && setConvertItem(null);

    const files = this.files.filter((el) => el.fileId + "" !== fileId);
    const filesToConversion = this.filesToConversion.filter(
      (el) => el.fileId + "" !== fileId
    );

    const newUploadData = {
      files,
      filesToConversion,
      filesSize: this.filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
    };

    this.setUploadData(newUploadData);
  };

  convertFile = (file, t) => {
    this.dialogsStore.setConvertItem(null);

    const secondConvertingWithPassword = file.hasOwnProperty("password");
    const conversionPositionIndex = file.hasOwnProperty("index");

    const alreadyConverting = this.files.some(
      (item) => item.fileId === file.fileId
    );

    if (this.converted && !alreadyConverting) {
      this.filesToConversion = [];
      this.convertFilesSize = 0;
      if (!secondConvertingWithPassword)
        this.files = this.files.filter((f) => f.action === "converted");

      this.primaryProgressDataStore.clearPrimaryProgressData();
    }

    if (!alreadyConverting) {
      if (secondConvertingWithPassword && conversionPositionIndex) {
        this.files.splice(file.index, 0, file);
      } else {
        this.files.push(file);
      }

      if (!this.filesToConversion.length) {
        this.filesToConversion.push(file);
        if (!secondConvertingWithPassword && !conversionPositionIndex)
          this.uploadedFilesHistory.push(file);
        this.startConversion(t);
      } else {
        this.filesToConversion.push(file);
        if (!secondConvertingWithPassword && !conversionPositionIndex)
          this.uploadedFilesHistory.push(file);
      }
    }
  };

  getNewPercent = (uploadedSize, indexOfFile) => {
    const newTotalSize = sumBy(this.files, (f) =>
      f.file && !this.uploaded ? f.file.size : 0
    );
    const totalUploadedFiles = this.files.filter((_, i) => i < indexOfFile);
    const totalUploadedSize = sumBy(totalUploadedFiles, (f) =>
      f.file && !this.uploaded ? f.file.size : 0
    );
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

  setConversionPercent = (percent, alert) => {
    const data = { icon: "file", percent, visible: true };

    if (this.uploaded) {
      this.primaryProgressDataStore.setPrimaryProgressBarData(
        alert ? { ...data, ...{ alert } } : data
      );
    }
  };

  getConversationPercent = (fileIndex) => {
    const length = this.files.filter((f) => f.needConvert).length;
    return (fileIndex / length) * 100;
  };

  startConversion = async (t) => {
    const {
      isRecentFolder,
      isFavoritesFolder,
      isShareFolder,
    } = this.treeFoldersStore;

    const { storeOriginalFiles } = this.settingsStore;

    const isSortedFolder = isRecentFolder || isFavoritesFolder || isShareFolder;
    const needToRefreshFilesList = !isSortedFolder || !storeOriginalFiles;

    runInAction(() => (this.converted = false));
    this.setConversionPercent(0);

    let index = 0;
    let len = this.filesToConversion.length;
    let filesToConversion = this.filesToConversion;

    while (index < len) {
      const conversionItem = filesToConversion[index];
      const { fileId, toFolderId, password } = conversionItem;
      const itemPassword = password ? password : null;
      const file = this.files.find((f) => f.fileId === fileId);
      if (file) runInAction(() => (file.inConversion = true));

      const historyFile = this.uploadedFilesHistory.find(
        (f) => f.fileId === fileId
      );
      if (historyFile) runInAction(() => (historyFile.inConversion = true));

      const data = await convertFile(fileId, itemPassword);

      if (data && data[0]) {
        let progress = data[0].progress;
        let fileInfo = null;
        let error = null;

        while (progress < 100) {
          const res = await this.getConversationProgress(fileId);
          progress = res && res[0] && res[0].progress;
          fileInfo = res && res[0].result;

          runInAction(() => {
            const file = this.files.find((file) => file.fileId === fileId);
            if (file) file.convertProgress = progress;

            const historyFile = this.uploadedFilesHistory.find(
              (file) => file.fileId === fileId
            );
            if (historyFile) historyFile.convertProgress = progress;
          });

          error = res && res[0] && res[0].error;
          if (error.length) {
            const percent = this.getConversationPercent(index + 1);
            this.setConversionPercent(percent, !!error);

            runInAction(() => {
              const file = this.files.find((file) => file.fileId === fileId);
              if (file) {
                file.error = error;
                file.inConversion = false;
                if (fileInfo === "password") file.needPassword = true;
              }

              const historyFile = this.uploadedFilesHistory.find(
                (file) => file.fileId === fileId
              );

              if (historyFile) {
                historyFile.error = error;
                historyFile.inConversion = false;
                if (fileInfo === "password") historyFile.needPassword = true;
              }
            });

            //this.refreshFiles(toFolderId, false);
            break;
          }

          const percent = this.getConversationPercent(index + 1);
          this.setConversionPercent(percent);
        }

        if (progress === 100) {
          if (!error) error = data[0].error;

          runInAction(() => {
            const file = this.files.find((file) => file.fileId === fileId);

            if (file) {
              file.error = error;
              file.convertProgress = progress;
              file.inConversion = false;

              if (error.indexOf("password") !== -1) {
                file.needPassword = true;
              } else file.action = "converted";
            }

            const historyFile = this.uploadedFilesHistory.find(
              (file) => file.fileId === fileId
            );

            if (historyFile) {
              historyFile.error = error;
              historyFile.convertProgress = progress;
              historyFile.inConversion = false;

              if (error.indexOf("password") !== -1) {
                historyFile.needPassword = true;
              } else historyFile.action = "converted";
            }
          });

          storeOriginalFiles && this.refreshFiles(file);

          if (fileInfo && fileInfo !== "password") {
            file.fileInfo = fileInfo;
            historyFile.fileInfo = fileInfo;
            needToRefreshFilesList && this.refreshFiles(file);
          }

          if (file && isSortedFolder) {
            const folderId = file.fileInfo?.folderId;
            const fileTitle = file.fileInfo?.title;

            folderId &&
              getFolderInfo(folderId)
                .then((folderInfo) =>
                  toastr.success(
                    t("InfoCreateFileIn", {
                      fileTitle,
                      folderTitle: folderInfo.title,
                    })
                  )
                )
                .catch((error) => toastr.error(error));
          }
          const percent = this.getConversationPercent(index + 1);
          this.setConversionPercent(percent, !!error);
        }
      }

      index++;
      filesToConversion = this.filesToConversion;
      len = filesToConversion.length;
    }

    if (this.uploaded) {
      this.setConversionPercent(100);
      this.finishUploadFiles();
    } else {
      runInAction(() => {
        this.converted = true;
        this.filesToConversion = [];
        this.conversionPercent = 0;
      });
    }
  };

  convertUploadedFiles = (t) => {
    this.files = [...this.files, ...this.tempConversionFiles];
    this.uploadedFilesHistory = [
      ...this.uploadedFilesHistory,
      ...this.tempConversionFiles,
    ];

    if (this.uploaded) {
      const newUploadData = {
        files: this.files,
        filesSize: this.convertFilesSize,
        uploadedFiles: this.uploadedFiles,
        percent: this.percent,
        uploaded: false,
        converted: false,
      };
      this.tempConversionFiles = [];

      this.setUploadData(newUploadData);
      this.startUploadFiles(t);
    }
    this.tempConversionFiles = [];
  };

  startUpload = (uploadFiles, folderId, t) => {
    const { canConvert } = this.settingsStore;

    const toFolderId = folderId ? folderId : this.selectedFolderStore.id;

    if (this.uploaded) {
      this.files = this.files.filter((f) => f.action !== "upload");
      this.filesSize = 0;
      this.uploadToFolder = null;
      this.percent = 0;
    }
    if (this.uploaded && this.converted) {
      this.files = [];
      this.filesToConversion = [];
    }

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
        toFolderId,
        action: "upload",
        error: file.size ? null : t("EmptyFile"),
        fileInfo: null,
        cancel: false,
        needConvert,
        encrypted: file.encrypted,
      };

      needConvert
        ? this.tempConversionFiles.push(newFile)
        : newFiles.push(newFile);

      filesSize += file.size;
      convertSize += file.size;
    }

    const countUploadingFiles = newFiles.length;
    const countConversionFiles = this.tempConversionFiles.length;

    if (countUploadingFiles && !countConversionFiles) {
      this.isUploading = true;
    } else {
      this.isUploadingAndConversion = true;
    }
    this.convertFilesSize = convertSize;

    //console.log("this.tempConversionFiles", this.tempConversionFiles);

    if (countConversionFiles)
      this.settingsStore.hideConfirmConvertSave
        ? this.convertUploadedFiles(t)
        : this.dialogsStore.setConvertDialogVisible(true);

    const clearArray = this.removeDuplicate([
      ...newFiles,
      ...this.uploadedFilesHistory,
    ]);

    this.uploadedFilesHistory = clearArray;

    const newUploadData = {
      files: newFiles,
      filesSize,
      uploadedFiles: this.uploadedFiles,
      percent: this.percent,
      uploaded: false,
      converted: !!this.tempConversionFiles.length,
    };

    if (this.uploaded && countUploadingFiles) {
      this.setUploadData(newUploadData);
      this.startUploadFiles(t);
    }
  };

  refreshFiles = async (currentFile) => {
    const {
      files,
      setFiles,
      folders,
      setFolders,
      filter,
      setFilter,
    } = this.filesStore;

    const { withPaging } = this.authStore.settingsStore;

    if (window.location.pathname.indexOf("/history") === -1) {
      const newFiles = files;
      const newFolders = folders;
      const path = currentFile.path ? currentFile.path.slice() : [];
      const fileIndex = newFiles.findIndex(
        (x) => x.id === currentFile.fileInfo.id
      );

      let folderInfo = null;
      const index = path.findIndex((x) => x === this.selectedFolderStore.id);
      const folderId = index !== -1 ? path[index + 1] : null;
      if (folderId) folderInfo = await getFolderInfo(folderId);

      const newPath = [];
      if (folderInfo || path[path.length - 1] === this.selectedFolderStore.id) {
        let i = 0;
        while (path[i] && path[i] !== folderId) {
          newPath.push(path[i]);
          i++;
        }
      }

      if (
        newPath[newPath.length - 1] !== this.selectedFolderStore.id &&
        path.length
      ) {
        return;
      }

      const addNewFile = () => {
        if (folderInfo) {
          const isFolderExist = newFolders.find((x) => x.id === folderInfo.id);
          if (!isFolderExist && folderInfo) {
            newFolders.unshift(folderInfo);
            setFolders(newFolders);
            const newFilter = filter;
            newFilter.total += 1;
            setFilter(newFilter);
          }
        } else {
          if (currentFile && currentFile.fileInfo) {
            if (fileIndex === -1) {
              newFiles.unshift(currentFile.fileInfo);
              setFiles(newFiles);
              const newFilter = filter;
              newFilter.total += 1;
              setFilter(newFilter);
            } else if (!this.settingsStore.storeOriginalFiles) {
              newFiles[fileIndex] = currentFile.fileInfo;
              setFiles(newFiles);
            }
          }
        }

        this.filesStore.setOperationAction(false);
      };

      const isFiltered =
        filter.filterType ||
        filter.authorType ||
        filter.search ||
        (withPaging && filter.page !== 0);

      if ((!currentFile && !folderInfo) || isFiltered) return;
      if (folderInfo && this.selectedFolderStore.id === folderInfo.id) return;

      if (folderInfo) {
        const folderIndex = folders.findIndex((f) => f.id === folderInfo.id);
        if (folderIndex !== -1) {
          folders[folderIndex] = folderInfo;
          return;
        }
      }

      if (filter.total >= filter.pageCount && withPaging) {
        if (files.length) {
          fileIndex === -1 && newFiles.pop();
          addNewFile();
        } else {
          newFolders.pop();
          addNewFile();
        }
      } else {
        addNewFile();
      }
    }
  };

  uploadFileChunks = async (
    location,
    requestsDataArray,
    fileSize,
    indexOfFile,
    file,
    path
  ) => {
    this.filesStore.setOperationAction(true);
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

      if (!res.data.data && res.data.message) {
        return Promise.reject(res.data.message);
      }

      const fileId = res.data.data.id;

      const { uploaded } = res.data.data;

      const uploadedSize = uploaded
        ? fileSize
        : index * this.settingsStore.chunkUploadSize;

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
        const fileInfo = await getFileInfo(fileId);
        runInAction(() => {
          this.files[indexOfFile].action = "uploaded";
          this.files[indexOfFile].fileId = fileId;
          this.files[indexOfFile].fileInfo = fileInfo;
          this.percent = newPercent;
        });
        //setUploadData(uploadData);
      }
    }

    // All chuncks are uploaded

    const currentFile = this.files[indexOfFile];
    currentFile.path = path;
    if (!currentFile) return Promise.resolve();
    const { needConvert } = currentFile;

    if (needConvert) {
      runInAction(() => (currentFile.action = "convert"));

      if (!currentFile.fileId) return;

      if (!this.filesToConversion.length || this.converted) {
        this.filesToConversion.push(currentFile);
        this.startConversion();
      } else {
        this.filesToConversion.push(currentFile);
      }
      return Promise.resolve();
    } else {
      if (currentFile.action === "uploaded") {
        this.refreshFiles(currentFile);
      }
      return Promise.resolve();
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

    if (!this.filesToConversion.length) {
      this.finishUploadFiles();
    } else {
      runInAction(() => (this.uploaded = true));
      const uploadedFiles = this.files.filter((x) => x.action === "uploaded");
      const totalErrorsCount = sumBy(uploadedFiles, (f) => (f.error ? 1 : 0));
      if (totalErrorsCount > 0)
        console.log("Upload errors: ", totalErrorsCount);

      setTimeout(() => {
        if (!this.uploadPanelVisible && !totalErrorsCount && this.converted) {
          this.clearUploadedFiles();
        }
      }, TIMEOUT);
    }
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
    } else if (
      item.action === "uploaded" ||
      item.action === "convert" ||
      item.action === "converted"
    ) {
      return Promise.resolve();
    }

    const { chunkUploadSize } = this.settingsStore;

    const { file, toFolderId /*, action*/ } = item;
    const chunks = Math.ceil(file.size / chunkUploadSize, chunkUploadSize);
    const fileName = file.name;
    const fileSize = file.size;
    const relativePath = file.path
      ? file.path.slice(1, -file.name.length)
      : file.webkitRelativePath
      ? file.webkitRelativePath.slice(0, -file.name.length)
      : "";

    return startUploadSession(
      toFolderId,
      fileName,
      fileSize,
      relativePath,
      file.encrypted,
      file.lastModifiedDate
    )
      .then((res) => {
        const location = res.data.location;
        const path = res.data.path;

        const requestsDataArray = [];

        let chunk = 0;

        while (chunk < chunks) {
          const offset = chunk * chunkUploadSize;
          const formData = new FormData();
          formData.append("file", file.slice(offset, offset + chunkUploadSize));
          requestsDataArray.push(formData);
          chunk++;
        }

        return { location, requestsDataArray, fileSize, path };
      })
      .then(({ location, requestsDataArray, fileSize, path }) => {
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
          path
        );
      })
      .catch((error) => {
        if (this.files[indexOfFile] === undefined) {
          this.primaryProgressDataStore.setPrimaryProgressBarData({
            icon: "upload",
            percent: 0,
            visible: false,
            alert: true,
          });
          return Promise.resolve();
        }
        let errorMessage = "";
        if (typeof error === "object") {
          errorMessage =
            error?.response?.data?.error?.message ||
            error?.statusText ||
            error?.message ||
            "";
        } else {
          errorMessage = error;
        }

        this.files[indexOfFile].error = errorMessage;

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
    const { fetchFiles, filter } = this.filesStore;
    const { withPaging } = this.authStore.settingsStore;

    const totalErrorsCount = sumBy(this.files, (f) => {
      f.error && toastr.error(f.error);
      return f.error ? 1 : 0;
    });

    if (totalErrorsCount > 0) {
      this.primaryProgressDataStore.setPrimaryProgressBarShowError(true); // for empty file
      this.primaryProgressDataStore.setPrimaryProgressBarErrors(
        totalErrorsCount
      );
      console.log("Errors: ", totalErrorsCount);
    }

    this.uploaded = true;
    this.converted = true;

    const uploadData = {
      filesSize: 0,
      uploadedFiles: 0,
      percent: 0,
      conversionPercent: 0,
    };

    if (this.files.length > 0) {
      const toFolderId = this.files[0]?.toFolderId;
      withPaging && fetchFiles(toFolderId, filter);

      if (toFolderId) {
        const { socketHelper } = this.authStore.settingsStore;

        socketHelper.emit({
          command: "refresh-folder",
          data: toFolderId,
        });
      }
    }

    setTimeout(() => {
      if (!this.primaryProgressDataStore.alert) {
        //this.primaryProgressDataStore.clearPrimaryProgressData();
      }

      if (this.uploadPanelVisible || this.primaryProgressDataStore.alert) {
        uploadData.files = this.files;
        uploadData.filesToConversion = this.filesToConversion;
      } else {
        // uploadData.files = [];
        // uploadData.filesToConversion = [];
        // this.isUploadingAndConversion = false;
        // this.isUploading = false;
      }

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
    } = this.secondaryProgressDataStore;

    return copyToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        if (res[0]?.error) return Promise.reject(res[0].error);

        const data = res[0] ? res[0] : null;
        const pbData = { icon: "duplicate" };

        return this.loopFilesOperations(data, pbData).then(() =>
          this.moveToCopyTo(destFolderId, pbData, true, fileIds, folderIds)
        );
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        this.clearActiveOperations(fileIds, folderIds);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return Promise.reject(err);
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
    } = this.secondaryProgressDataStore;

    return moveToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    )
      .then((res) => {
        const data = res[0] ? res[0] : null;
        const pbData = { icon: "move" };

        return this.loopFilesOperations(data, pbData).then(() =>
          this.moveToCopyTo(destFolderId, pbData, false, fileIds, folderIds)
        );
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        this.clearActiveOperations(fileIds, folderIds);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return Promise.reject(err);
      });
  };

  copyAsAction = (fileId, title, folderId, enableExternalExt, password) => {
    const { fetchFiles, filter } = this.filesStore;

    return fileCopyAs(fileId, title, folderId, enableExternalExt, password)
      .then(() => {
        fetchFiles(folderId, filter, true, true);
      })
      .catch((err) => {
        return Promise.reject(err);
      });
  };

  fileCopyAs = async (fileId, title, folderId, enableExternalExt, password) => {
    return fileCopyAs(fileId, title, folderId, enableExternalExt, password);
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
      filesCount: this.secondaryProgressDataStore.filesCount + fileIds.length,
    });

    return isCopy
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

  loopFilesOperations = async (data, pbData, isDownloadAction) => {
    const {
      clearSecondaryProgressData,
      setSecondaryProgressBarData,
    } = this.secondaryProgressDataStore;

    const label = this.secondaryProgressDataStore.label;
    let progress = data.progress;

    if (!data) {
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      return;
    }

    let operationItem = data;
    let finished = data.finished;

    while (!finished) {
      const item = await this.getOperationProgress(data.id);
      operationItem = item;

      progress = item ? item.progress : 100;
      finished = item
        ? isDownloadAction
          ? item.finished && item.url
          : item.finished
        : true;

      setSecondaryProgressBarData({
        icon: pbData.icon,
        label: pbData.label || label,
        percent: progress,
        visible: true,
        alert: false,
        currentFile: item,
      });
    }

    return operationItem;
  };

  moveToCopyTo = (destFolderId, pbData, isCopy, fileIds, folderIds) => {
    const {
      fetchFiles,
      filter,
      isEmptyLastPageAfterOperation,
      resetFilterPage,
    } = this.filesStore;

    const {
      clearSecondaryProgressData,
      setSecondaryProgressBarData,
      label,
    } = this.secondaryProgressDataStore;

    let receivedFolder = destFolderId;
    let updatedFolder = this.selectedFolderStore.id;

    if (this.dialogsStore.isFolderActions) {
      receivedFolder = this.selectedFolderStore.parentId;
      updatedFolder = destFolderId;
    }

    if (!isCopy || destFolderId === this.selectedFolderStore.id) {
      let newFilter;

      if (isEmptyLastPageAfterOperation()) {
        newFilter = resetFilterPage();
      }

      fetchFiles(
        updatedFolder,
        newFilter ? newFilter : filter,
        true,
        true,
        false
      ).finally(() => {
        this.clearActiveOperations(fileIds, folderIds);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        this.dialogsStore.setIsFolderActions(false);
      });
    } else {
      this.clearActiveOperations(fileIds, folderIds);
      setSecondaryProgressBarData({
        icon: pbData.icon,
        label: pbData.label || label,
        percent: 100,
        visible: true,
        alert: false,
      });

      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
    }
  };

  getOperationProgress = async (id) => {
    const promise = new Promise((resolve, reject) => {
      setTimeout(async () => {
        try {
          await getProgress().then((res) => {
            const currentItem = res.find((x) => x.id === id);
            if (currentItem?.error) {
              reject(currentItem.error);
            }
            resolve(currentItem);
          });
        } catch (error) {
          reject(error);
        }
      }, 1000);
    });
    return promise;
  };

  clearActiveOperations = (fileIds = [], folderIds = []) => {
    const {
      activeFiles,
      activeFolders,
      setActiveFiles,
      setActiveFolders,
    } = this.filesStore;

    const newActiveFiles = activeFiles.filter((el) => !fileIds?.includes(el));
    const newActiveFolders = activeFolders.filter(
      (el) => !folderIds.includes(el)
    );

    setActiveFiles(newActiveFiles);
    setActiveFolders(newActiveFolders);
  };

  clearUploadedFilesHistory = () => {
    this.primaryProgressDataStore.clearPrimaryProgressData();
    this.uploadedFilesHistory = [];
  };

  removeDuplicate = (items) => {
    let obj = {};
    return items.filter((x) => {
      if (obj[x.uniqueId]) return false;
      obj[x.uniqueId] = true;
      return true;
    });
  };
}

export default UploadDataStore;
