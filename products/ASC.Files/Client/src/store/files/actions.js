import { api, history, constants, toastr, store } from "asc-web-common";
import axios from "axios";
import queryString from "query-string";
import {
  AUTHOR_TYPE,
  FILTER_TYPE,
  PAGE_COUNT,
  PAGE,
  SEARCH_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  FOLDER,
  PREVIEW,
} from "../../helpers/constants";
import config from "../../../package.json";
import {
  createTreeFolders,
  canConvert,
  loopTreeFolders,
  getSelectedFolderId,
  getFilter,
  getIsRecycleBinFolder,
  getProgressData,
  getTreeFolders,
  getSettingsTree,
  getPrivacyFolder,
} from "./selectors";

const { files, FilesFilter } = api;
const { FolderType } = constants;
const { isEncryptionSupport } = store.auth.selectors;

export const SET_FOLDER = "SET_FOLDER";
export const SET_FOLDERS = "SET_FOLDERS";
export const SET_FILE = "SET_FILE";
export const SET_FILES = "SET_FILES";
export const SET_SELECTION = "SET_SELECTION";
export const SET_SELECTED = "SET_SELECTED";
export const SET_SELECTED_FOLDER = "SET_SELECTED_FOLDER";
export const SET_TREE_FOLDERS = "SET_TREE_FOLDERS";
export const SET_FILES_FILTER = "SET_FILES_FILTER";
export const SET_FILTER = "SET_FILTER";
export const SELECT_FILE = "SELECT_FILE";
export const DESELECT_FILE = "DESELECT_FILE";
export const SET_ACTION = "SET_ACTION";
export const SET_DRAGGING = "SET_DRAGGING";
export const SET_DRAG_ITEM = "SET_DRAG_ITEM";
export const SET_MEDIA_VIEWER_VISIBLE = "SET_MEDIA_VIEWER_VISIBLE";
export const SET_PROGRESS_BAR_DATA = "SET_PROGRESS_BAR_DATA";
export const SET_VIEW_AS = "SET_VIEW_AS";
export const SET_CONVERT_DIALOG_VISIBLE = "SET_CONVERT_DIALOG_VISIBLE";
export const SET_UPDATE_TREE = "SET_UPDATE_TREE";
export const SET_NEW_ROW_ITEMS = "SET_NEW_ROW_ITEMS";
export const SET_SELECTED_NODE = "SET_SELECTED_NODE";
export const SET_EXPAND_SETTINGS_TREE = "SET_EXPAND_SETTINGS_TREE";
export const SET_IS_LOADING = "SET_IS_LOADING";
export const SET_THIRD_PARTY = "SET_THIRD_PARTY";
export const SET_FILES_SETTINGS = "SET_FILES_SETTINGS";
export const SET_FILES_SETTING = "SET_FILES_SETTING";
export const SET_IS_ERROR_SETTINGS = "SET_IS_ERROR_SETTINGS";
export const SET_FIRST_LOAD = "SET_FIRST_LOAD";
export const SET_UPLOAD_DATA = "SET_UPLOAD_DATA";

export function setFile(file) {
  return {
    type: SET_FILE,
    file,
  };
}

export function setFiles(files) {
  return {
    type: SET_FILES,
    files,
  };
}

export function setFolder(folder) {
  return {
    type: SET_FOLDER,
    folder,
  };
}

export function setFolders(folders) {
  return {
    type: SET_FOLDERS,
    folders,
  };
}

export function setSelection(selection) {
  return {
    type: SET_SELECTION,
    selection,
  };
}

export function setSelected(selected) {
  return {
    type: SET_SELECTED,
    selected,
  };
}

export function setAction(fileAction) {
  return {
    type: SET_ACTION,
    fileAction,
  };
}

export function setSelectedFolder(selectedFolder) {
  return {
    type: SET_SELECTED_FOLDER,
    selectedFolder,
  };
}

export function setTreeFolders(treeFolders) {
  return {
    type: SET_TREE_FOLDERS,
    treeFolders,
  };
}

export function setDragging(dragging) {
  return {
    type: SET_DRAGGING,
    dragging,
  };
}

export function setDragItem(dragItem) {
  return {
    type: SET_DRAG_ITEM,
    dragItem,
  };
}

export function setFilesFilter(filter) {
  setFilterUrl(filter);
  return {
    type: SET_FILES_FILTER,
    filter,
  };
}
export function setFilter(filter) {
  return {
    type: SET_FILTER,
    filter,
  };
}

export function setViewAs(viewAs) {
  return {
    type: SET_VIEW_AS,
    viewAs,
  };
}

export function selectFile(file) {
  return {
    type: SELECT_FILE,
    file,
  };
}

export function deselectFile(file) {
  return {
    type: DESELECT_FILE,
    file,
  };
}

export function setMediaViewerData(mediaViewerData) {
  return {
    type: SET_MEDIA_VIEWER_VISIBLE,
    mediaViewerData,
  };
}

export function setProgressBarData(progressData) {
  return {
    type: SET_PROGRESS_BAR_DATA,
    progressData,
  };
}

export function setConvertDialogVisible(convertDialogVisible) {
  return {
    type: SET_CONVERT_DIALOG_VISIBLE,
    convertDialogVisible,
  };
}

export function setUpdateTree(updateTree) {
  return {
    type: SET_UPDATE_TREE,
    updateTree,
  };
}

export function setNewRowItems(newRowItems) {
  return {
    type: SET_NEW_ROW_ITEMS,
    newRowItems,
  };
}

export function setSelectedNode(node) {
  return {
    type: SET_SELECTED_NODE,
    node,
  };
}

export function setExpandSettingsTree(setting) {
  return {
    type: SET_EXPAND_SETTINGS_TREE,
    setting,
  };
}

export function setIsLoading(isLoading) {
  return {
    type: SET_IS_LOADING,
    isLoading,
  };
}

export function setFilesSettings(settings) {
  return {
    type: SET_FILES_SETTINGS,
    settings,
  };
}

export function setFilesSetting(setting, val) {
  return {
    type: SET_FILES_SETTING,
    setting,
    val,
  };
}

export function setIsErrorSettings(isError) {
  return {
    type: SET_IS_ERROR_SETTINGS,
    isError,
  };
}

export function setFirstLoad(firstLoad) {
  return {
    type: SET_FIRST_LOAD,
    firstLoad,
  };
}

export function setUploadData(uploadData) {
  return {
    type: SET_UPLOAD_DATA,
    uploadData,
  };
}
export function setFilterUrl(filter) {
  const defaultFilter = FilesFilter.getDefault();
  const params = [];
  const URLParams = queryString.parse(window.location.href);

  if (filter.filterType) {
    params.push(`${FILTER_TYPE}=${filter.filterType}`);
  }

  if (filter.withSubfolders === "false") {
    params.push(`${SEARCH_TYPE}=${filter.withSubfolders}`);
  }

  if (filter.search) {
    params.push(`${SEARCH}=${filter.search.trim()}`);
  }
  if (filter.authorType) {
    params.push(`${AUTHOR_TYPE}=${filter.authorType}`);
  }
  if (filter.folder) {
    params.push(`${FOLDER}=${filter.folder}`);
  }

  if (filter.pageCount !== defaultFilter.pageCount) {
    params.push(`${PAGE_COUNT}=${filter.pageCount}`);
  }

  if (URLParams.preview) {
    params.push(`${PREVIEW}=${URLParams.preview}`);
  }

  params.push(`${PAGE}=${filter.page + 1}`);
  params.push(`${SORT_BY}=${filter.sortBy}`);
  params.push(`${SORT_ORDER}=${filter.sortOrder}`);

  history.push(`${config.homepage}/filter?${params.join("&")}`);
}

// TODO: similar to fetchFolder, remove one
export function fetchFiles(folderId, filter) {
  return (dispatch, getState) => {
    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;

    const state = getState();
    const privacyFolder = getPrivacyFolder(state);

    if (privacyFolder && privacyFolder.id === +folderId) {
      const isEncryptionSupported = isEncryptionSupport(state);

      if (!isEncryptionSupported) {
        filterData.treeFolders = createTreeFolders(
          privacyFolder.pathParts,
          filterData
        );
        filterData.total = 0;
        dispatch(setFilesFilter(filterData));

        dispatch(setFolders([]));
        dispatch(setFiles([]));
        dispatch(setSelected("close"));
        dispatch(
          setSelectedFolder({
            folders: [],
            ...privacyFolder,
            pathParts: privacyFolder.pathParts,
            ...{ new: 0 },
          })
        );
        return Promise.resolve();
      }
    }

    return files.getFolder(folderId, filter).then((data) => {
      const isPrivacyFolder =
        data.current.rootFolderType === FolderType.Privacy;
      filterData.treeFolders = createTreeFolders(data.pathParts, filterData);
      filterData.total = data.total;
      dispatch(setFilesFilter(filterData));
      dispatch(
        setFolders(isPrivacyFolder && !isEncryptionSupport ? [] : data.folders)
      );
      dispatch(
        setFiles(isPrivacyFolder && !isEncryptionSupport ? [] : data.files)
      );
      dispatch(setSelected("close"));
      return dispatch(
        setSelectedFolder({
          folders: data.folders,
          ...data.current,
          pathParts: data.pathParts,
          ...{ new: data.new },
        })
      );
    });
  };
}

export function fetchFolders() {
  return Promise.resolve([]);
}

export function selectFolder() {
  return Promise.resolve([]);
}

export function fetchFolder(folderId, dispatch) {
  return files.getFolder(folderId).then((data) => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(
      setSelectedFolder({
        folders: data.folders,
        ...data.current,
        pathParts: data.pathParts,
      })
    );
  });
}

export function fetchMyFolder(dispatch) {
  return files.getMyFolderList().then((data) => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchTrashFolder(dispatch) {
  return files.getTrashFolderList().then((data) => {
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchCommonFolder(dispatch) {
  return files.getCommonFolderList().then((data) => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchFavoritesFolder(folderId) {
  return (dispatch) => {
    return files.getFolder(folderId).then((data) => {
      dispatch(setFolders(data.folders));
      dispatch(setFiles(data.files));
      return dispatch(
        setSelectedFolder({
          folders: data.folders,
          ...data.current,
          pathParts: data.pathParts,
        })
      );
    });
  };
}

export function markItemAsFavorite(id) {
  return (dispatch) => {
    return files.markAsFavorite(id);
  };
}

export function removeItemFromFavorite(id) {
  return (dispatch) => {
    return files.removeFromFavorite(id);
  };
}

export function getFileInfo(id) {
  return (dispatch) => {
    return files.getFileInfo(id).then((data) => {
      dispatch(setFile(data));
    });
  };
}

export function fetchProjectsFolder(dispatch) {
  return files.getProjectsFolderList().then((data) => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchSharedFolder(dispatch) {
  return files.getSharedFolderList().then((data) => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchTreeFolders(dispatch) {
  return files.getFoldersTree().then((data) => dispatch(setTreeFolders(data)));
}

/*export function testUpdateMyFolder(folders) {
  return (dispatch, getState) => {
    const { files } = getState();

    console.log("folders", folders);

    const newRoot = rootFolders;
    newRoot.my.folders = folders;
    console.log("newRoot.my.folders", newRoot.my.folders);
    console.log("folders", folders);
    console.log("newRoot", newRoot);
    //dispatch(setRootFolders(null));
    dispatch(setRootFolders(newRoot));

  }
  //setRootFolders
}*/

export function createFile(folderId, title) {
  return (dispatch) => {
    return files.createFile(folderId, title).then((file) => {
      return Promise.resolve(file);
    });
  };
}

export function createFolder(parentFolderId, title) {
  return (dispatch) => {
    return files.createFolder(parentFolderId, title).then((folder) => {
      fetchFolder(parentFolderId, dispatch);
    });
  };
}

export function updateFile(fileId, title) {
  return (dispatch) => {
    return files.updateFile(fileId, title).then((file) => {
      dispatch(setFile(file));
    });
  };
}

export function addFileToRecentlyViewed(fileId) {
  return (dispatch) => {
    return files.addFileToRecentlyViewed(fileId);
  };
}

export function renameFolder(folderId, title) {
  return (dispatch) => {
    return files.renameFolder(folderId, title).then((folder) => {
      dispatch(setFolder(folder));
    });
  };
}

export function setShareFiles(
  folderIds,
  fileIds,
  share,
  notify,
  sharingMessage
) {
  const foldersRequests = folderIds.map((id) =>
    files.setShareFolder(id, share, notify, sharingMessage)
  );

  const filesRequests = fileIds.map((id) =>
    files.setShareFiles(id, share, notify, sharingMessage)
  );

  const requests = [...foldersRequests, ...filesRequests];
  return axios.all(requests);
}

export function getShareUsers(folderIds, fileIds) {
  const foldersRequests = folderIds.map((folderId) =>
    files.getShareFolders(folderId)
  );
  const filesRequests = fileIds.map((fileId) => files.getShareFiles(fileId));
  const requests = [...foldersRequests, ...filesRequests];

  return axios.all(requests).then((res) => res);
}

export function clearProgressData() {
  return (dispatch) => {
    dispatch(setProgressBarData({ visible: false, percent: 0, label: "" }));
  };
}

/*export function deleteGroup(id) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    return api.groups
      .deleteGroup(id)
      .then(res => {
        return dispatch(setGroups(groups.filter(g => g.id !== id)));
      })
      .then(() => {
        const newFilter = filter.clone(true);
        return fetchPeople(newFilter, dispatch);
      });
  };
}*/

export function setUpdateIfExist(data, setting) {
  return (dispatch) => {
    return files
      .updateIfExist(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function setStoreOriginal(data, setting) {
  return (dispatch) => {
    return files
      .storeOriginal(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function setConfirmDelete(data, setting) {
  return (dispatch) => {
    return files
      .changeDeleteConfirm(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function setStoreForceSave(data, setting) {
  return (dispatch) => {
    return files
      .storeForceSave(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function setEnableThirdParty(data, setting) {
  return (dispatch) => {
    return files
      .thirdParty(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function setForceSave(data, setting) {
  return (dispatch) => {
    return files
      .forceSave(data)
      .then((res) => dispatch(setFilesSetting(setting, res)));
  };
}

export function getFilesSettings() {
  return (dispatch, getState) => {
    const state = getState();
    const settingsTree = getSettingsTree(state);

    if (Object.keys(settingsTree).length === 0) {
      return api.files
        .getSettingsFiles()
        .then((settings) => dispatch(setFilesSettings(settings)))
        .catch(() => setIsErrorSettings(true));
    } else {
      return Promise.resolve(settingsTree);
    }
  };
}

export const startUpload = (uploadFiles, folderId, t) => {
  return (dispatch, getState) => {
    const state = getState();
    const newFiles = [];
    let filesSize = 0;
    const convertFiles = [];
    let convertFilesSize = 0;

    for (let index of Object.keys(uploadFiles)) {
      const item = uploadFiles[index];
      if (item.size !== 0) {
        const parts = item.name.split(".");
        const ext = parts.length > 1 ? "." + parts.pop() : "";
        if (canConvert(ext)(state)) {
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

    const uploadStatus = convertFiles.length ? "pending" : null;
    const uploadToFolder = folderId;
    const showConvertDialog = !!convertFiles.length;

    const newUploadData = {
      files: newFiles,
      filesSize,
      convertFiles,
      convertFilesSize,
      uploadStatus,
      uploadToFolder,
      uploadedFiles: 0,
      percent: 0,
      uploaded: false,
    };
    dispatch(setUploadData(newUploadData));

    if (showConvertDialog) {
      dispatch(setConvertDialogVisible(showConvertDialog));
    }

    startUploadFiles(
      t,
      newFiles.length,
      convertFiles.length,
      dispatch,
      getState
    );
  };
};

const startUploadFiles = (
  t,
  filesLength,
  convertFilesLength,
  dispatch,
  getState
) => {
  if (filesLength > 0 || convertFilesLength > 0) {
    const progressData = { visible: true, percent: 0, label: "" };
    progressData.label = t("UploadingLabel", {
      file: 0,
      totalFiles: filesLength + convertFilesLength,
    });
    dispatch(setProgressBarData(progressData));
    startSessionFunc(0, t, dispatch, getState);
  }
};

const startSessionFunc = (indexOfFile, t, dispatch, getState) => {
  const state = getState();
  const { uploadData } = state.files;
  const { uploaded, uploadToFolder, files, convertFiles } = uploadData;

  const currentFiles = uploaded ? convertFiles : files;

  if (!uploaded && files.length === 0) {
    uploadData.uploaded = true;
    dispatch(setUploadData(uploadData));
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
  const relativePath = file.path
    ? file.path.slice(1, -file.name.length)
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
        indexOfFile,
        t,
        dispatch,
        getState
      )
    )
    .catch((err) => {
      toastr.error(err);
      dispatch(clearProgressData());
    });
};

const sendChunk = (
  files,
  location,
  requestsDataArray,
  isLatestFile,
  indexOfFile,
  t,
  dispatch,
  getState
) => {
  const state = getState();
  const { uploadData } = state.files;
  const {
    uploaded,
    percent,
    uploadedFiles,
    uploadToFolder,
    filesSize,
    convertFilesSize,
  } = uploadData;
  const totalSize = convertFilesSize + filesSize;

  const sendRequestFunc = (index) => {
    api.files
      .uploadFile(location, requestsDataArray[index])
      .then((res) => {
        //percent problem? use getState()
        const currentFile = files[indexOfFile];
        const fileId = res.data.data.id;
        const newPercent = percent + (currentFile.size / totalSize) * 100;

        if (res.data.data && res.data.data.uploaded) {
          //newState = { percent: newPercent };
        }

        if (index + 1 !== requestsDataArray.length) {
          dispatch(
            setProgressBarData({
              label: t("UploadingLabel", {
                file: uploadedFiles,
                totalFiles: files.length,
              }),
              newPercent,
              visible: true,
            })
          );
          sendRequestFunc(index + 1);
        } else if (uploaded) {
          api.files.convertFile(fileId).then((convertRes) => {
            if (convertRes && convertRes[0] && convertRes[0].progress !== 100) {
              uploadData.percent = newPercent;
              getConvertProgress(
                fileId,
                t,
                uploadData,
                isLatestFile,
                indexOfFile,
                dispatch,
                getState
              );
            }
          });
        } else if (isLatestFile) {
          if (uploaded) {
            updateFiles(uploadToFolder, dispatch, getState);
          } else {
            const uploadStatus = getState().files.uploadData.uploadStatus;
            if (uploadStatus === "convert") {
              const newUploadData = {
                ...getState().files.uploadData,
                ...{
                  uploadedFiles: uploadedFiles + 1,
                  percent: newPercent,
                  uploaded: true,
                },
              };
              updateConvertProgress(newUploadData, t, dispatch);
              startSessionFunc(0, t, dispatch, getState);
            } else if (uploadStatus === "pending") {
              const stateUploadData = getState().files.uploadData;
              const newUploadData = {
                ...stateUploadData,
                ...{
                  uploadStatus: null,
                  uploadedFiles: uploadedFiles + 1,
                  percent: newPercent,
                  uploaded: true,
                },
              };
              updateConvertProgress(newUploadData, t, dispatch);
            } else {
              const newUploadData = {
                ...getState().files.uploadData,
                ...{ uploadedFiles: uploadedFiles + 1, percent: newPercent },
              };
              updateConvertProgress(newUploadData, t, dispatch);
              updateFiles(uploadToFolder, dispatch, getState);
            }
          }
        } else {
          const newUploadData = {
            ...getState().files.uploadData,
            ...{ uploadedFiles: uploadedFiles + 1, percent: newPercent },
          };
          updateConvertProgress(newUploadData, t, dispatch);
          startSessionFunc(indexOfFile + 1, t, dispatch, getState);
        }
      })
      .catch((err) => toastr.error(err));
  };

  sendRequestFunc(0);
};

const updateFiles = (folderId, dispatch, getState) => {
  const { files } = getState();
  const { filter, treeFolders, selectedFolder } = files;
  const uploadData = {
    files: [],
    filesSize: 0,
    convertFiles: [],
    convertFilesSize: 0,
    uploadStatus: null,
    uploadToFolder: null,
    uploadedFiles: 0,
    percent: 0,
    uploaded: true,
  };

  if (selectedFolder.id === folderId) {
    return dispatch(fetchFiles(selectedFolder.id, filter.clone()))
      .then((data) => {
        const path = data.selectedFolder.pathParts;
        const newTreeFolders = treeFolders;
        const folders = data.selectedFolder.folders;
        const foldersCount = data.selectedFolder.foldersCount;
        loopTreeFolders(path, newTreeFolders, folders, foldersCount);
        dispatch(setTreeFolders(newTreeFolders));
        dispatch(setUpdateTree(true));
      })
      .catch((err) => toastr.error(err))
      .finally(() =>
        setTimeout(() => {
          dispatch(clearProgressData());
          dispatch(setUploadData(uploadData));
        }, 5000)
      );
  } else {
    return api.files
      .getFolder(folderId, filter.clone())
      .then((data) => {
        const path = data.pathParts;
        const newTreeFolders = treeFolders;
        const folders = data.folders;
        const foldersCount = data.count;
        loopTreeFolders(path, newTreeFolders, folders, foldersCount);
        dispatch(setTreeFolders(newTreeFolders));
        dispatch(setUpdateTree(true));
      })
      .catch((err) => toastr.error(err))
      .finally(() =>
        setTimeout(() => {
          dispatch(clearProgressData());
          dispatch(setUploadData(uploadData));
        }, 5000)
      );
  }
};

const getConvertProgress = (
  fileId,
  t,
  uploadData,
  isLatestFile,
  indexOfFile,
  dispatch,
  getState
) => {
  const { uploadedFiles, uploadToFolder } = uploadData;
  api.files.getConvertFile(fileId).then((res) => {
    if (res && res[0] && res[0].progress !== 100) {
      setTimeout(
        () =>
          getConvertProgress(
            fileId,
            t,
            uploadData,
            isLatestFile,
            indexOfFile,
            dispatch,
            getState
          ),
        1000
      );
    } else {
      uploadData.uploadedFiles = uploadedFiles + 1;
      updateConvertProgress(uploadData, t, dispatch);
      !isLatestFile && startSessionFunc(indexOfFile + 1, t, dispatch, getState);

      if (res[0].error) {
        toastr.error(res[0].error);
      }
      if (isLatestFile) {
        updateFiles(uploadToFolder, dispatch, getState);
        return;
      }
    }
  });
};

const updateConvertProgress = (uploadData, t, dispatch) => {
  const { uploadedFiles, uploadStatus, files, convertFiles } = uploadData;
  let progressVisible = true;
  const file = uploadedFiles;
  let percent = uploadData.percent;

  const totalFiles =
    uploadStatus === "cancel"
      ? files.length
      : files.length + convertFiles.length;

  if (uploadedFiles === totalFiles) {
    percent = 100;
    uploadData.percent = 0;
    uploadData.uploadedFiles = 0;
    uploadData.uploadStatus = null;
    progressVisible = false;
  }

  dispatch(setUploadData(uploadData));

  dispatch(
    setProgressBarData({
      label: t("UploadingLabel", { file, totalFiles }),
      percent,
      visible: true,
    })
  );
  if (!progressVisible) {
    setTimeout(() => dispatch(clearProgressData()), 5000);
  }
};

export const setDialogVisible = (t) => {
  return (dispatch, getState) => {
    const { uploadData } = getState().files;
    const {
      files,
      uploadStatus,
      uploadToFolder,
      uploadedFiles,
      percent,
    } = uploadData;

    dispatch(setConvertDialogVisible(false));
    const label = t("UploadingLabel", {
      file: uploadedFiles,
      totalFiles: files.length,
    });

    if (uploadStatus === null) {
      dispatch(setProgressBarData({ label, percent: 100, visible: true }));
      uploadData.uploadedFiles = 0;
      uploadData.percent = 0;
      dispatch(setUploadData(uploadData));
      updateFiles(uploadToFolder, dispatch, getState);
    } else if (!files.length) {
      dispatch(clearProgressData());
    } else {
      dispatch(setProgressBarData({ label, percent, visible: true }));
      uploadData.uploadStatus = "cancel";
      dispatch(setUploadData(uploadData));
    }
  };
};

export const onConvert = (t) => {
  return (dispatch, getState) => {
    const { uploadData } = getState().files;

    if (uploadData.uploaded) {
      startSessionFunc(0, t, dispatch, getState);
    }

    uploadData.uploadStatus = "convert";
    dispatch(setUploadData(uploadData));
    dispatch(setConvertDialogVisible(false));
  };
};

export const setSelections = (items) => {
  return (dispatch, getState) => {
    const {
      selection,
      folders,
      files,
      fileActionId,
      selected,
    } = getState().files;

    if (selection.length > items.length) {
      //Delete selection
      const newSelection = [];
      let newFile = null;
      for (let item of items) {
        if (!item) break; // temporary fall protection selection tile

        item = item.split("_");
        if (item[0] === "folder") {
          newFile = selection.find(
            (x) => x.id === Number(item[1]) && !x.fileExst
          );
        } else if (item[0] === "file") {
          newFile = selection.find(
            (x) => x.id === Number(item[1]) && x.fileExst
          );
        }
        if (newFile) {
          newSelection.push(newFile);
        }
      }

      for (let item of selection) {
        const element = newSelection.find(
          (x) => x.id === item.id && x.fileExst === item.fileExst
        );
        if (!element) {
          dispatch(deselectFile(item));
        }
      }
    } else if (selection.length < items.length) {
      //Add selection
      for (let item of items) {
        if (!item) break; // temporary fall protection selection tile

        let newFile = null;
        item = item.split("_");
        if (item[0] === "folder") {
          newFile = folders.find(
            (x) => x.id === Number(item[1]) && !x.fileExst
          );
        } else if (item[0] === "file") {
          newFile = files.find((x) => x.id === Number(item[1]) && x.fileExst);
        }
        if (newFile && fileActionId !== newFile.id) {
          const existItem = selection.find(
            (x) => x.id === newFile.id && x.fileExst === newFile.fileExst
          );
          if (!existItem) {
            dispatch(selectFile(newFile));
            selected !== "none" && dispatch(setSelected("none"));
          }
        }
      }
    } else {
      return;
    }
  };
};

export const loopFilesOperations = (id, destFolderId, isCopy) => {
  return (dispatch, getState) => {
    const state = getState();

    const currentFolderId = getSelectedFolderId(state);
    const filter = getFilter(state);
    const isRecycleBin = getIsRecycleBinFolder(state);
    const progressData = getProgressData(state);
    const treeFolders = getTreeFolders(state);

    const loopOperation = () => {
      api.files
        .getProgress()
        .then((res) => {
          const currentItem = res.find((x) => x.id === id);
          if (currentItem && currentItem.progress !== 100) {
            dispatch(
              setProgressBarData({
                label: progressData.label,
                percent: currentItem.progress,
                visible: true,
              })
            );
            setTimeout(() => loopOperation(), 1000);
          } else {
            dispatch(
              setProgressBarData({
                label: progressData.label,
                percent: 100,
                visible: true,
              })
            );
            api.files
              .getFolder(destFolderId)
              .then((data) => {
                let newTreeFolders = treeFolders;
                let path = data.pathParts.slice(0);
                let folders = data.folders;
                let foldersCount = data.current.foldersCount;
                loopTreeFolders(path, newTreeFolders, folders, foldersCount);

                if (!isCopy || destFolderId === currentFolderId) {
                  dispatch(fetchFiles(currentFolderId, filter))
                    .then((data) => {
                      if (!isRecycleBin) {
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
                        dispatch(setUpdateTree(true));
                        dispatch(setTreeFolders(newTreeFolders));
                      }
                    })
                    .catch((err) => {
                      console.log("ERROR_1", err);
                      toastr.error(err);
                      dispatch(clearProgressData());
                    })
                    .finally(() =>
                      setTimeout(() => dispatch(clearProgressData()), 5000)
                    );
                } else {
                  dispatch(
                    setProgressBarData({
                      label: progressData.label,
                      percent: 100,
                      visible: true,
                    })
                  );
                  setTimeout(() => dispatch(clearProgressData()), 5000);
                  dispatch(setUpdateTree(true));
                  dispatch(setTreeFolders(newTreeFolders));
                }
              })
              .catch((err) => {
                console.log("ERROR_2", err);
                toastr.error(err);
                dispatch(clearProgressData());
              });
          }
        })
        .catch((err) => {
          console.log("ERROR_3", err);
          toastr.error(err);
          dispatch(clearProgressData());
        });
    };

    loopOperation();
  };
};

export function selectItemOperation(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter, isCopy) {
  return (dispatch) => {
    return isCopy ?
      files.copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
      :
      files.moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
  }
}

export function itemOperationToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter, isCopy) {
  return (dispatch) => {
    return dispatch(selectItemOperation(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter, isCopy))
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        dispatch(loopFilesOperations(id, destFolderId, isCopy))
      })
      .catch((err) => {
        toastr.error(err);
        dispatch(clearProgressData())
      })
  };
}
