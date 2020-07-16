import { api } from "asc-web-common";
import { toastr } from "asc-web-components";
import {
  fetchFiles,
  setProgressBarData,
  clearProgressData,
  setConvertDialogVisible,
  setTreeFolders
} from "../../../store/files/actions";
import { canConvert, loopTreeFolders } from "../../../store/files/selectors";
import store from "../../../store/store";

let state = { uploadedFiles: 0, percent: 0, uploaded: true };

const updateState = (newState) => {
  state = { ...state, ...newState };
};

export const startUpload = (uploadFiles, folderId, t) => {
  return (dispatch, getState) => {
    const { files } = getState();
    const { filter, selectedFolder, treeFolders } = files;

    const newFiles = [];
    let filesSize = 0;
    const convertFiles = [];
    let convertFilesSize = 0;

    for (let item of uploadFiles) {
      if (item.size !== 0) {
        const parts = item.name.split(".");
        const ext = parts.length > 1 ? "." + parts.pop() : "";
        if (canConvert(ext)) {
          convertFiles.push(item);
          convertFilesSize += item.size;
        } else {
          newFiles.push(item);
          filesSize += item.size;
        }
      } else {
        toastr.error(t("ErrorUploadMessage"));
      }
    }

    const showConvertDialog = !!convertFiles.length;
    showConvertDialog &&
      store.dispatch(setConvertDialogVisible(showConvertDialog));
    const uploadStatus = convertFiles.length ? "pending" : null;
    const uploadToFolder = folderId;

    const newState = {
      files: newFiles,
      filesSize,
      convertFiles,
      convertFilesSize,
      uploadToFolder,
      showConvertDialog,
      uploaded: false,
      uploadStatus,
      filter,
      selectedFolder,
      treeFolders,
      t
    };

    startUploadFiles(newState);
  };
};

const startUploadFiles = (newState) => {
  const { files, filesSize, convertFiles, convertFilesSize, t } = newState;

  if (files.length > 0 || convertFiles.length > 0) {
    const totalSize = convertFilesSize + filesSize;
    const updatedState = { ...newState, ...{ totalSize } };
    updateState(updatedState);
    const progressData = { visible: true, percent: 0, label: "" };
    progressData.label = t("UploadingLabel", { file: 0, totalFiles: files.length + convertFiles.length });
    store.dispatch(setProgressBarData(progressData));
    startSessionFunc(0);
  }
};

const startSessionFunc = (indexOfFile) => {
  const { files, convertFiles, uploaded, uploadToFolder } = state;

  const currentFiles = uploaded ? convertFiles : files;

  if (!uploaded && files.length === 0) {
    updateState({uploaded: true});
    return;
  }

  let file = files[indexOfFile];
  let isLatestFile = indexOfFile === files.length - 1;

  if (uploaded) {
    if (convertFiles.length) {
      file = convertFiles[indexOfFile];
      isLatestFile = indexOfFile === convertFiles.length - 1;
    } else {
      //Test return empty convert files
      return;
    }
  }

  const fileName = file.name;
  const fileSize = file.size;
  const relativePath = file.relativePath
    ? file.relativePath.slice(1, -file.name.length)
    : file.webkitRelativePath
    ? file.webkitRelativePath.slice(0, -file.name.length)
    : "";

  let location;
  const requestsDataArray = [];
  const chunkSize = 1024 * 1023; //~0.999mb
  const chunks = Math.ceil(file.size / chunkSize, chunkSize);
  let chunk = 0;

  api.files
    .startUploadSession(uploadToFolder, fileName, fileSize, relativePath)
    .then((res) => {
      location = res.data.location;
      while (chunk < chunks) {
        const offset = chunk * chunkSize;
        const formData = new FormData();
        formData.append("file", file.slice(offset, offset + chunkSize));
        requestsDataArray.push(formData);
        chunk++;
      }
    })
    .then(() =>
      sendChunk(
        currentFiles,
        location,
        requestsDataArray,
        isLatestFile,
        indexOfFile
      )
    )
    .catch((err) => {
      toastr.error(err);
      clearProgressData(store.dispatch);
    });
};

const sendChunk = (
  files,
  location,
  requestsDataArray,
  isLatestFile,
  indexOfFile
) => {
  const { uploadToFolder, totalSize, uploaded, uploadedFiles, t } = state;
  const sendRequestFunc = (index) => {
    let newState = {};
    api.files
      .uploadFile(location, requestsDataArray[index])
      .then((res) => {
        let newPercent = state.percent;
        const currentFile = files[indexOfFile];
        const fileId = res.data.data.id;
        const percent = (newPercent += (currentFile.size / totalSize) * 100);

        if (res.data.data && res.data.data.uploaded) {
          newState = { percent };
        }

        if (index + 1 !== requestsDataArray.length) {
          store.dispatch( setProgressBarData({ label: t("UploadingLabel", { file: uploadedFiles, totalFiles: files.length }), percent, visible: true }));
          sendRequestFunc(index + 1);
        } else if (uploaded) {
          api.files.convertFile(fileId).then((convertRes) => {
            if (convertRes && convertRes[0] && convertRes[0].progress !== 100) {
              getConvertProgress(fileId, newState, isLatestFile, indexOfFile);
            }
          });
        } else if (isLatestFile) {
          if (uploaded) {
            updateFiles(uploadToFolder);
          } else {
            const { uploadStatus } = state;
            if (uploadStatus === "convert") {
              newState = { ...newState, ...{ uploadedFiles: uploadedFiles + 1, uploadStatus: null, uploaded: true }};
              updateConvertProgress(newState, uploadStatus);
              startSessionFunc(0);
            } else if (uploadStatus === "pending") {
              newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1, uploaded: true, uploadStatus: null }};
              updateConvertProgress(newState, uploadStatus);
            } else {
              newState = { ...newState, ...{ uploadedFiles: uploadedFiles + 1, uploadStatus: null }};
              updateConvertProgress(newState, uploadStatus);
              updateFiles(uploadToFolder);
            }
          }
        } else {
          newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1 }};
          updateConvertProgress(newState, state.uploadStatus);
          startSessionFunc(indexOfFile + 1);
        }
      })
      .catch((err) => toastr.error(err));
  };

  sendRequestFunc(0);
};

const updateFiles = (folderId) => {
  const { filter, treeFolders, selectedFolder } = state;

  if (selectedFolder.id === folderId) {
    return fetchFiles(
      selectedFolder.id,
      filter.clone(),
      store.dispatch,
      treeFolders
    )
      .then((data) => {
        const path = data.selectedFolder.pathParts;
        const newTreeFolders = treeFolders;
        const folders = data.selectedFolder.folders;
        const foldersCount = data.selectedFolder.foldersCount;
        loopTreeFolders(path, newTreeFolders, folders, foldersCount);
        store.dispatch(setTreeFolders(newTreeFolders));
      })
      .catch((err) => toastr.error(err))
      .finally(() => setTimeout(() => clearProgressData(store.dispatch), 5000));
    //.finally(() => this.setState({ uploaded: true }));
  } else {
    return api.files
      .getFolder(folderId, filter.clone())
      .then((data) => {
        const path = data.pathParts;
        const newTreeFolders = treeFolders;
        const folders = data.folders;
        const foldersCount = data.count;
        loopTreeFolders(path, newTreeFolders, folders, foldersCount);
        store.dispatch(setTreeFolders(newTreeFolders));
      })
      .catch((err) => toastr.error(err))
      .finally(() => setTimeout(() => clearProgressData(store.dispatch), 5000));
    //.finally(() => this.setState({ uploaded: true }));
  }
};

const getConvertProgress = (fileId, newState, isLatestFile, indexOfFile) => {
  const { uploadedFiles, uploadStatus, uploadToFolder } = state;
  api.files.getConvertFile(fileId).then((res) => {
    if (res && res[0] && res[0].progress !== 100) {
      setTimeout(
        () => getConvertProgress(fileId, newState, isLatestFile, indexOfFile),
        1000
      );
    } else {
      newState = { ...newState, ...{ uploadedFiles: uploadedFiles + 1 } };
      updateConvertProgress(newState, uploadStatus);
      !isLatestFile && startSessionFunc(indexOfFile + 1);

      if (res[0].error) {
        toastr.error(res[0].error);
      }
      if (isLatestFile) {
        updateFiles(uploadToFolder);
        return;
      }
    }
  });
};

const updateConvertProgress = (newState, uploadStatus) => {
  let progressVisible = true;
  let uploadedFiles = newState.uploadedFiles;
  let percent = newState.percent;

  const totalFiles =
    uploadStatus === "cancel"
      ? state.files.length
      : state.files.length + state.convertFiles.length;

  if (newState.uploadedFiles === totalFiles) {
    percent = 100;
    newState.percent = 0;
    newState.uploadedFiles = 0;
    progressVisible = false;
  }

  updateState(newState);
  store.dispatch(
    setProgressBarData({
      label: state.t("UploadingLabel", { file: uploadedFiles, totalFiles }),
      percent,
      visible: true,
    })
  );
  if (!progressVisible) {
    setTimeout(() => clearProgressData(store.dispatch), 5000);
  }
};

export const setDialogVisible = () => {
  const { files, uploadStatus, uploadToFolder, uploadedFiles, percent } = state;

  store.dispatch(setConvertDialogVisible(false));
  const label = state.t("UploadingLabel", { file: uploadedFiles, totalFiles: files.length });

  if (uploadStatus === null) {
    store.dispatch(setProgressBarData({ label, percent, visible: true }));
    updateState({uploadedFiles: 0, percent: 0});
    updateFiles(uploadToFolder);
  } else if (!files.length) {
    clearProgressData(store.dispatch);
  } else {
    store.dispatch(setProgressBarData({ label, percent, visible: true }));
    updateState({ uploadStatus: "cancel", totalFiles: files.length });
  }
};

export const onConvert = () => {
  if (state.uploaded) {
    startSessionFunc(0);
  }

  updateState({ uploadStatus: "convert" });
  store.dispatch(setConvertDialogVisible(false));
};
