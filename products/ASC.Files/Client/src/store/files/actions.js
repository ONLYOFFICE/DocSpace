import { api, constants } from "asc-web-common";
const { FilterType, FileType } = constants;
const { files } = api;

export const SET_FOLDERS = "SET_FOLDERS";
export const SET_FILES = "SET_FILES";
export const SET_SELECTION = "SET_SELECTION";
export const SET_SELECTED = "SET_SELECTED";
export const SET_SELECTED_FOLDER = "SET_SELECTED_FOLDER";
export const SET_ROOT_FOLDERS = "SET_ROOT_FOLDERS";
export const SET_FILES_FILTER = "SET_FILES_FILTER";

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

// export function setFilesFilter(filter) {
//   return {
//     type: SET_FILES_FILTER,
//     filter
//   };
// }

export function fetchFiles(filter) {
  //TODO: add real API request, change algorithm
  return (dispatch, getState) => {
    let filterData = filter && filter.clone();
    // dispatch(setFilesFilter(filterData));
    const { files: filesStore } = getState();
    const currentFilterType = filter.filterType;
    const fileType = getFileTypeByFilterType(currentFilterType);
    const selectedFolderId = filesStore.selectedFolder.id;

    if (currentFilterType === FilterType.None) {
      return fetchFolder(selectedFolderId, dispatch)
    }
    else {
      return files.getFolder(selectedFolderId)
        .then(data => {
          const sortedFiles = data.files
            .filter(file => file.fileType === fileType);
          dispatch(setFiles(sortedFiles));
        });
    }
  }
}

// only for fake sorting
function getFileTypeByFilterType(filterType) {
  switch (filterType) {
    case FilterType.ImagesOnly:
      return FileType.Image;
    case FilterType.DocumentsOnly:
      return FileType.Document;
    case FilterType.PresentationsOnly:
      return FileType.Presentation;
    case FilterType.SpreadsheetsOnly:
      return FileType.Spreadsheet;
    case FilterType.ArchiveOnly:
      return FileType.Archive;
    case FilterType.MediaOnly:
      // without FileType.Video for simplifying
      return FileType.Audio;
    default:
      return FileType.Unknown;
  }
}

export function fetchFolders() {
  return Promise.resolve([]);
}

export function selectFolder() {
  return Promise.resolve([]);
}

export function fetchFolder(folderId, dispatch) {
  return files.getFolder(folderId).then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  })
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

