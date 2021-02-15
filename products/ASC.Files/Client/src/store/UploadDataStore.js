import { makeObservable, action, observable } from "mobx";
import { api } from "asc-web-common";
import uniqueid from "lodash/uniqueId";

class UploadDataStore {
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

  constructor() {
    makeObservable(this, {
      files: observable,
      filesSize: observable,
      convertFiles: observable,
      convertFilesSize: observable,
      uploadStatus: observable,
      uploadToFolder: observable,
      uploadedFiles: observable,
      percent: observable,
      uploaded: observable,
      selectedUploadFile: observable,
      uploadPanelVisible: observable,
      convertDialogVisible: observable,

      selectUploadedFile: action,
      setUploadPanelVisible: action,
      clearUploadData: action,
      cancelUpload: action,
      setUploadData: action,
      updateUploadedItem: action,
      setConvertDialogVisible: action,
    });
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

  setDialogVisible = (t) => {
    /*return (dispatch, getState) => {
      const { uploadData } = getState().files;
      const { files, uploadStatus, uploadedFiles, percent } = uploadData;
  
      this.setConvertDialogVisible(false);
      const label = t("UploadingLabel", {
        file: uploadedFiles,
        totalFiles: files.length,
      });
  
      if (uploadStatus === null) {
        dispatch(
          setPrimaryProgressBarData({
            icon: "upload",
            label,
            percent: 100,
            visible: true,
            alert: false,
          })
        );
        uploadData.uploadedFiles = 0;
        uploadData.percent = 0;
        dispatch(setUploadData(uploadData));
      } else if (!files.length) {
        dispatch(clearPrimaryProgressData());
      } else {
        dispatch(
          setPrimaryProgressBarData({
            icon: "upload",
            label,
            percent,
            visible: true,
            alert: false,
          })
        );
        uploadData.uploadStatus = "cancel";
        dispatch(setUploadData(uploadData));
      }
    };*/
  };

  convertUploadedFiles = (t) => {
    /*return (dispatch, getState) => {
      const state = getState();
  
      const { uploadData } = state.files;
      const filesToConvert = getFilesToConvert(uploadData.files);
  
      if (filesToConvert.length > 0) {
        startConvertFiles(filesToConvert, t, dispatch, getState).then(() =>
          finishUploadFiles(getState, dispatch)
        );
      } else {
        finishUploadFiles(getState, dispatch);
      }
    };*/
  };

  startUpload = (uploadFiles, folderId, t) => {
    /*let newFiles = this.files;
    let filesSize = 0;

    for (let index of Object.keys(uploadFiles)) {
      const file = uploadFiles[index];

      const parts = file.name.split(".");
      const ext = parts.length > 1 ? "." + parts.pop() : "";
      const needConvert = canConvert(ext)(state);

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

    this.setUploadData(newUploadData);

    if (this.uploaded) {
      startUploadFiles(t, dispatch, getState);
    }*/
  };

  // startUploadFiles = async (t, dispatch, getState) => {
  //   let state = getState();

  //   let { files, percent, filesSize } = state.files.uploadData;

  //   if (files.length === 0 || filesSize === 0)
  //     return finishUploadFiles(getState, dispatch);

  //   const progressData = {
  //     visible: true,
  //     percent,
  //     icon: "upload",
  //     alert: false,
  //   };

  //   dispatch(setPrimaryProgressBarData(progressData));

  //   let index = 0;
  //   let len = files.length;

  //   while (index < len) {
  //     await startSessionFunc(index, t, dispatch, getState);
  //     index++;

  //     state = getState();
  //     files = state.files.uploadData.files;
  //     len = files.length;
  //   }

  //   //TODO: Uncomment after fix conversation
  //   /*const filesToConvert = getFilesToConvert(files);
  //   if (filesToConvert.length > 0) {
  //     // Ask to convert options
  //     return dispatch(setConvertDialogVisible(true));
  //   }*/

  //   // All files has been uploaded and nothing to convert
  //   finishUploadFiles(getState, dispatch);
  // };
}

export default UploadDataStore;
