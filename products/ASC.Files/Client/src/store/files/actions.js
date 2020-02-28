import { api } from "asc-web-common";

export const SET_FOLDERS = "SET_FOLDERS";
export const SET_FILES = "SET_FILES";
export const SET_SELECTION = "SET_SELECTION";
export const SET_SELECTED = "SET_SELECTED";
export const SET_SELECTED_FOLDER = "SET_SELECTED_FOLDER";

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
  return api.files.getMyFolderList().then(data => {
    
    dispatch(setFolders(data.folders));
    
    dispatch(setSelectedFolder(data.current));

    dispatch(setFiles(data.files));
    
    return Promise.resolve([]); 
  });
}

