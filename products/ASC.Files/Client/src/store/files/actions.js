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
  TIMEOUT,
} from "../../helpers/constants";
import config from "../../../package.json";
import {
  createTreeFolders,
  canConvert,
  loopTreeFolders,
  getSelectedFolderId,
  getFilter,
  getIsRecycleBinFolder,
  getPrimaryProgressData,
  getSecondaryProgressData,
  getTreeFolders,
  getSettingsTree,
  getPrivacyFolder,
} from "./selectors";

import sumBy from "lodash/sumBy";
import throttle from "lodash/throttle";

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
export const SET_PRIMARY_PROGRESS_BAR_DATA = "SET_PRIMARY_PROGRESS_BAR_DATA";
export const SET_SECONDARY_PROGRESS_BAR_DATA =
  "SET_SECONDARY_PROGRESS_BAR_DATA";
export const SET_VIEW_AS = "SET_VIEW_AS";
export const SET_CONVERT_DIALOG_VISIBLE = "SET_CONVERT_DIALOG_VISIBLE";
export const SET_SHARING_PANEL_VISIBLE = "SET_SHARING_PANEL_VISIBLE";
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

export function setPrimaryProgressBarData(primaryProgressData) {
  return {
    type: SET_PRIMARY_PROGRESS_BAR_DATA,
    primaryProgressData,
  };
}

export function setSecondaryProgressBarData(secondaryProgressData) {
  return {
    type: SET_SECONDARY_PROGRESS_BAR_DATA,
    secondaryProgressData,
  };
}

export function setConvertDialogVisible(convertDialogVisible) {
  return {
    type: SET_CONVERT_DIALOG_VISIBLE,
    convertDialogVisible,
  };
}

export function setSharingPanelVisible(sharingPanelVisible) {
  return {
    type: SET_SHARING_PANEL_VISIBLE,
    sharingPanelVisible,
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
export function fetchFiles(folderId, filter, clearFilter = true) {
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
        if (clearFilter) {
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
        }
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
      if (clearFilter) {
        dispatch(setSelected("close"));
      }
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
  sharingMessage,
  externalAccess
) {
  const foldersRequests = folderIds.map((id) =>
    files.setShareFolder(id, share, notify, sharingMessage)
  );

  const filesRequests = fileIds.map((id) =>
    files.setShareFiles(id, share, notify, sharingMessage)
  );

  let externalAccessRequest = [];

  if (fileIds.length === 1 && externalAccess !== null) {
    externalAccessRequest = fileIds.map((id) =>
      files.setExternalAccess(id, externalAccess)
    );
  }

  const requests = [
    ...foldersRequests,
    ...filesRequests,
    ...externalAccessRequest,
  ];
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

export function clearPrimaryProgressData() {
  return (dispatch) => {
    dispatch(
      setPrimaryProgressBarData({
        visible: false,
        percent: 0,
        label: "",
        icon: "",
        alert: false,
      })
    );
  };
}

export function clearSecondaryProgressData() {
  return (dispatch) => {
    dispatch(
      setSecondaryProgressBarData({
        visible: false,
        percent: 0,
        label: "",
        icon: "",
        alert: false,
      })
    );
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

    const { uploadData } = state.files;

    console.log("start upload", uploadData);

    let newFiles = state.files.uploadData.files;
    let filesSize = 0;

    for (let index of Object.keys(uploadFiles)) {
      const file = uploadFiles[index];

      const parts = file.name.split(".");
      const ext = parts.length > 1 ? "." + parts.pop() : "";
      const needConvert = canConvert(ext)(state);

      newFiles.push({
        file: file,
        fileId: null,
        toFolderId: folderId,
        action: needConvert ? "convert" : "upload",
        error: null,
      });

      filesSize += file.size;
    }

    //const showConvertDialog = uploadStatus === "pending";

    const { percent, uploadedFiles, uploaded } = uploadData;

    console.log("newFiles: ", newFiles);

    const newUploadData = {
      files: newFiles,
      filesSize,
      uploadedFiles,
      percent,
      uploaded: false,
    };

    dispatch(setUploadData(newUploadData));

    if (uploaded) {
      startUploadFiles(t, dispatch, getState);
    }
  };
};

const startUploadFiles = async (t, dispatch, getState) => {
  let state = getState();

  let { files, percent } = state.files.uploadData;

  if (files.length === 0) return finishUploadFiles(getState, dispatch);

  const progressData = {
    visible: true,
    percent,
    label: t("UploadingLabel", {
      file: 0,
      totalFiles: files.length,
    }),
    icon: "upload",
    alert: false,
  };

  dispatch(setPrimaryProgressBarData(progressData));

  let index = 0;
  let len = files.length;

  while (index < len) {
    await startSessionFunc(index, t, dispatch, getState);
    index++;

    state = getState();
    files = state.files.uploadData.files;
    len = files.length;
  }

  //TODO: Uncomment after fix conversation
  /*const filesToConvert = getFilesToConvert(files);

  if (filesToConvert.length > 0) {
    // Ask to convert options
    return dispatch(setConvertDialogVisible(true));
  }*/

  // All files has been uploaded and nothing to convert
  finishUploadFiles(getState, dispatch);
};

const getFilesToConvert = (files) => {
  const filesToConvert = files.filter((f) => f.action === "convert");
  return filesToConvert;
};

const finishUploadFiles = (getState, dispatch) => {
  const state = getState();
  const { files } = state.files.uploadData;

  const totalErrorsCount = sumBy(files, (f) => (f.error ? 1 : 0));

  if (totalErrorsCount > 0) return;

  const uploadData = {
    files: [],
    filesSize: 0,
    uploadStatus: null,
    uploadedFiles: 0,
    percent: 0,
    uploaded: true,
  };

  setTimeout(() => {
    dispatch(clearPrimaryProgressData());
    dispatch(setUploadData(uploadData));
  }, TIMEOUT);
};

const chunkSize = 1024 * 1023; //~0.999mb

const throttleRefreshFiles = throttle((toFolderId, dispatch, getState) => {
  return refreshFiles(toFolderId, dispatch, getState).catch((err) => {
    console.log("RefreshFiles failed", err);
    return Promise.resolve();
  });
}, 1000);

const startSessionFunc = (indexOfFile, t, dispatch, getState) => {
  const state = getState();
  const { uploadData } = state.files;
  const { uploaded, files } = uploadData;

  console.log("START UPLOAD SESSION FUNC", uploadData);

  if (!uploaded && files.length === 0) {
    uploadData.uploaded = true;
    dispatch(setUploadData(uploadData));
    return;
  }

  const item = files[indexOfFile];

  if (!item) {
    console.error("Empty files");
    return Promise.resolve();
  }

  const { file, toFolderId /*, action*/ } = item;

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
      const chunks = Math.ceil(file.size / chunkSize, chunkSize);
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
    .then(({ location, requestsDataArray, fileSize }) =>
      uploadFileChunks(
        location,
        requestsDataArray,
        fileSize,
        indexOfFile,
        dispatch,
        t,
        getState
      )
    )
    .catch((err) => {
      console.error(err);

      const state = getState();
      const { uploadData } = state.files;

      uploadData.files[indexOfFile].error = err;

      dispatch(setUploadData(uploadData));

      const newPercent = getNewPercent(fileSize, indexOfFile, getState);

      dispatch(
        setPrimaryProgressBarData({
          icon: "upload",
          label: "Error", //TODO: Add translation
          percent: newPercent,
          visible: true,
          alert: true,
        })
      );

      return Promise.resolve();
    });
};

const uploadFileChunks = async (
  location,
  requestsDataArray,
  fileSize,
  indexOfFile,
  dispatch,
  t,
  getState
) => {
  const length = requestsDataArray.length;
  for (let index = 0; index < length; index++) {
    const res = await api.files.uploadFile(location, requestsDataArray[index]);

    console.log(`Uploaded chunk ${index}/${length}`, res);

    //let isLatestFile = indexOfFile === newFilesLength - 1;
    const fileId = res.data.data.id;

    const { uploaded } = res.data.data;

    const uploadedSize = uploaded ? fileSize : index * chunkSize;

    const newPercent = getNewPercent(uploadedSize, indexOfFile, getState);

    const newState = getState();
    const { uploadData } = newState.files;
    const { uploadedFiles, files } = uploadData;

    dispatch(
      setPrimaryProgressBarData({
        icon: "upload",
        label: t("UploadingLabel", {
          file: uploadedFiles,
          totalFiles: files.length,
        }),
        percent: newPercent,
        visible: true,
      })
    );

    if (uploaded) {
      uploadData.files[indexOfFile].fileId = fileId;
      dispatch(setUploadData(uploadData));
    }
  }

  // All chuncks are uploaded

  const newState = getState();

  const { files } = newState.files.uploadData;
  const currentFile = files[indexOfFile];

  if (!currentFile) return Promise.resolve();

  const { toFolderId } = currentFile;

  return throttleRefreshFiles(toFolderId, dispatch, getState);
};

const getNewPercent = (uploadedSize, indexOfFile, getState) => {
  const newState = getState();
  const { files } = newState.files.uploadData;

  const newTotalSize = sumBy(files, (f) => f.file.size);
  const totalUploadedFiles = files.filter((_, i) => i < indexOfFile);
  const totalUploadedSize = sumBy(totalUploadedFiles, (f) => f.file.size);
  const newPercent = ((uploadedSize + totalUploadedSize) / newTotalSize) * 100;

  console.log(
    `newPercent=${newPercent} (newTotalSize=${newTotalSize} totalUploadedSize=${totalUploadedSize} indexOfFile=${indexOfFile})`
  );

  return newPercent;
};

const updateFiles = (folderId, dispatch, getState) => {
  //console.log("folderId ", folderId);
  const uploadData = {
    files: [],
    filesSize: 0,
    convertFiles: [],
    convertFilesSize: 0,
    uploadedFiles: 0,
    percent: 0,
    uploaded: true,
  };
  return refreshFiles(folderId, dispatch, getState)
    .catch((err) => {
      dispatch(
        setPrimaryProgressBarData({
          alert: true,
          visible: true,
        })
      );
      setTimeout(() => dispatch(clearPrimaryProgressData()), TIMEOUT);
      //toastr.error(err);
    })
    .finally(() =>
      setTimeout(() => {
        dispatch(clearPrimaryProgressData());
        dispatch(setUploadData(uploadData));
      }, TIMEOUT)
    );
};

const refreshFiles = (folderId, dispatch, getState) => {
  const { files } = getState();
  const { filter, treeFolders, selectedFolder } = files;
  if (
    selectedFolder.id === folderId &&
    window.location.pathname.indexOf("/history") === -1
  ) {
    return dispatch(fetchFiles(selectedFolder.id, filter.clone(), false)).then(
      (data) => {
        const path = data.selectedFolder.pathParts;
        const newTreeFolders = treeFolders;
        const folders = data.selectedFolder.folders;
        const foldersCount = data.selectedFolder.foldersCount;
        loopTreeFolders(path, newTreeFolders, folders, foldersCount);
        dispatch(setTreeFolders(newTreeFolders));
        dispatch(setUpdateTree(true));
      }
    );
  } else {
    return api.files.getFolder(folderId, filter.clone()).then((data) => {
      const path = data.pathParts;
      const newTreeFolders = treeFolders;
      const folders = data.folders;
      const foldersCount = data.count;
      loopTreeFolders(path, newTreeFolders, folders, foldersCount);
      dispatch(setTreeFolders(newTreeFolders));
      dispatch(setUpdateTree(true));
    });
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
  const { uploadedFiles } = uploadData;
  api.files.getFileConversationProgress(fileId).then((res) => {
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
        dispatch(
          setPrimaryProgressBarData({
            visible: true,
            alert: true,
          })
        );
        setTimeout(() => dispatch(clearPrimaryProgressData()), TIMEOUT);
        //toastr.error(res[0].error);
      }
      if (isLatestFile) {
        const toFolderId = uploadData.files[indexOfFile].toFolderId;
        updateFiles(toFolderId, dispatch, getState);
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
    setPrimaryProgressBarData({
      icon: "upload",
      label: t("UploadingLabel", { file, totalFiles }),
      percent,
      visible: true,
      alert: false,
    })
  );
  if (!progressVisible) {
    setTimeout(() => dispatch(clearPrimaryProgressData()), TIMEOUT);
  }
};

export const setDialogVisible = (t) => {
  return (dispatch, getState) => {
    const { uploadData } = getState().files;
    const { files, uploadStatus, uploadedFiles, percent } = uploadData;

    dispatch(setConvertDialogVisible(false));
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
  };
};
export const convertUploadedFiles = (t) => {
  return (dispatch, getState) => {
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
  };
};

const getConversationProgress = async (fileId) => {
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

const startConvertFiles = async (files, t, dispatch, getState) => {
  const state = getState();
  const { uploadData } = state.files;

  const total = files.length;
  dispatch(setConvertDialogVisible(false));
  dispatch(
    setPrimaryProgressBarData({
      icon: "file",
      label: t("ConvertingLabel", {
        file: 0,
        totalFiles: total,
      }),
      percent: 0,
      visible: true,
    })
  );
  for (let index = 0; index < total; index++) {
    const fileId = uploadData.files[index].fileId;

    const data = await api.files.convertFile(fileId);

    if (data && data[0] && data[0].progress !== 100) {
      let progress = data[0].progress;
      let error = null;
      while (progress < 100) {
        const res = await getConversationProgress(fileId);

        progress = res && res[0] && res[0].progress;
        error = res && res[0] && res[0].error;
        if (error.length) {
          dispatch(
            setPrimaryProgressBarData({
              icon: "file",
              visible: true,
              alert: true,
            })
          );
          return;
        }
        if (progress === 100) {
          break;
        } else {
          //TODO: calculate local progress
          // const percent = (progress) + (index / total) * 100;
          // dispatch(
          //   setPrimaryProgressBarData({
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

    dispatch(
      setPrimaryProgressBarData({
        icon: "file",
        label: t("ConvertingLabel", {
          file: index + 1,
          totalFiles: total,
        }),
        percent: newPercent,
        visible: true,
      })
    );
  }
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
    const progressData = getSecondaryProgressData(state);
    const treeFolders = getTreeFolders(state);

    const loopOperation = () => {
      api.files
        .getProgress()
        .then((res) => {
          const currentItem = res.find((x) => x.id === id);
          if (currentItem && currentItem.progress !== 100) {
            dispatch(
              setSecondaryProgressBarData({
                icon: "move",
                label: progressData.label,
                percent: currentItem.progress,
                visible: true,
                alert: false,
              })
            );
            setTimeout(() => loopOperation(), 1000);
          } else {
            dispatch(
              setSecondaryProgressBarData({
                icon: "move",
                label: progressData.label,
                percent: 100,
                visible: true,
                alert: false,
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
                      dispatch(
                        setPrimaryProgressBarData({
                          visible: true,
                          alert: true,
                        })
                      );
                      //toastr.error(err);
                      setTimeout(
                        () => dispatch(clearPrimaryProgressData()),
                        TIMEOUT
                      );
                    })
                    .finally(() =>
                      setTimeout(
                        () => dispatch(clearPrimaryProgressData()),
                        TIMEOUT
                      )
                    );
                } else {
                  dispatch(
                    setSecondaryProgressBarData({
                      icon: "duplicate",
                      label: progressData.label,
                      percent: 100,
                      visible: true,
                      alert: false,
                    })
                  );
                  setTimeout(
                    () => dispatch(clearSecondaryProgressData()),
                    TIMEOUT
                  );
                  dispatch(setUpdateTree(true));
                  dispatch(setTreeFolders(newTreeFolders));
                }
              })
              .catch((err) => {
                console.log("ERROR_2", err);
                dispatch(
                  setSecondaryProgressBarData({
                    visible: true,
                    alert: true,
                  })
                );
                //toastr.error(err);
                setTimeout(
                  () => dispatch(clearSecondaryProgressData()),
                  TIMEOUT
                );
              });
          }
        })
        .catch((err) => {
          console.log("ERROR_3", err);
          dispatch(
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            })
          );
          //toastr.error(err);
          setTimeout(() => dispatch(clearSecondaryProgressData()), TIMEOUT);
        });
    };

    loopOperation();
  };
};

export function selectItemOperation(
  destFolderId,
  folderIds,
  fileIds,
  conflictResolveType,
  deleteAfter,
  isCopy
) {
  return (dispatch) => {
    return isCopy
      ? files.copyToFolder(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        )
      : files.moveToFolder(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
  };
}

export function itemOperationToFolder(
  destFolderId,
  folderIds,
  fileIds,
  conflictResolveType,
  deleteAfter,
  isCopy
) {
  return (dispatch) => {
    return dispatch(
      selectItemOperation(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter,
        isCopy
      )
    )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        dispatch(loopFilesOperations(id, destFolderId, isCopy));
      })
      .catch((err) => {
        dispatch(
          setPrimaryProgressBarData({
            visible: true,
            alert: true,
          })
        );
        //toastr.error(err);
        setTimeout(() => dispatch(clearPrimaryProgressData()), TIMEOUT);
        setTimeout(() => dispatch(clearSecondaryProgressData()), TIMEOUT);
      });
  };
}
