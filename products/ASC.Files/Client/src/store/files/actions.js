import { api } from "asc-web-common";

const { files } = api;

export const SET_FOLDERS = "SET_FOLDERS";
export const SET_FILES = "SET_FILES";
export const SET_SELECTION = "SET_SELECTION";
export const SET_SELECTED = "SET_SELECTED";
export const SET_SELECTED_FOLDER = "SET_SELECTED_FOLDER";
export const SET_ROOT_FOLDERS = "SET_ROOT_FOLDERS";

export function setFiles(files) {
  return {
    type: SET_FILES,
    files
  };
}

export function setFolders(folders) {
  return {
    type: SET_FOLDERS,
    folders
  };
}

export function setSelection(selection) {
  return {
    type: SET_SELECTION,
    selection
  };
}

export function setSelected(selected) {
  return {
    type: SET_SELECTED,
    selected
  };
}

export function setSelectedFolder(selectedFolder) {
  return {
    type: SET_SELECTED_FOLDER,
    selectedFolder
  };
}

export function setRootFolders(rootFolders) {
  return {
    type: SET_ROOT_FOLDERS,
    rootFolders
  };
}

export function fetchFiles() {
  return Promise.resolve([]);
}

export function fetchFolders() {
  return Promise.resolve([]);
}

export function selectFolder() {
  return Promise.resolve([]);
}

export function fetchMyFolder(dispatch) {
  return files.getMyFolderList().then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchTrashFolder(dispatch) {
  return files.getTrashFolderList().then(data => {
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchCommonFolder(dispatch) {
  return files.getCommonFolderList().then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchProjectsFolder(dispatch) {
  return files.getProjectsFolderList().then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchSharedFolder(dispatch) {
  return files.getSharedFolderList().then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function fetchRootFolders(dispatch) {

  //TODO: Make some more Useful

  let root = {
    my: null,
    share: null,
    common: null,
    project: null,
    trash: null
  };

  return files.getMyFolderList()
    .then(data => root.my = data.current)
    .then(() => files.getCommonFolderList()
      .then(data => root.common = data.current))
    .then(() => files.getProjectsFolderList()
      .then(data => root.project = data.current))
    .then(() => files.getTrashFolderList()
      .then(data => root.trash = data.current))
    .then(() => files.getSharedFolderList()
      .then(data => root.share = data.current))
    .then(() => dispatch(setRootFolders(root)));
}

