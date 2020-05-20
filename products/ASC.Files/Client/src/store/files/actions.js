import { api, history } from "asc-web-common";
import axios from "axios";
import {
  AUTHOR_TYPE,
  FILTER_TYPE,
  PAGE_COUNT,
  PAGE,
  SEARCH_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  FOLDER
} from "../../helpers/constants";
import config from "../../../package.json";
import { getTreeFolders } from "./selectors";

const { files, FilesFilter } = api;

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

export function setFile(file) {
  return {
    type: SET_FILE,
    file
  };
}

export function setFiles(files) {
  return {
    type: SET_FILES,
    files
  };
}

export function setFolder(folder) {
  return {
    type: SET_FOLDER,
    folder
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

export function setAction(fileAction) {
  return {
    type: SET_ACTION,
    fileAction
  };
}

export function setSelectedFolder(selectedFolder) {
  return {
    type: SET_SELECTED_FOLDER,
    selectedFolder
  };
}

export function setTreeFolders(treeFolders) {
  return {
    type: SET_TREE_FOLDERS,
    treeFolders
  };
}

export function setFilesFilter(filter) {
  setFilterUrl(filter);
  return {
    type: SET_FILES_FILTER,
    filter
  };
}
export function setFilter(filter) {
  return {
    type: SET_FILTER,
    filter
  };
};

export function selectFile(file) {
  return {
    type: SELECT_FILE,
    file
  };
}

export function deselectFile(file) {
  return {
    type: DESELECT_FILE,
    file
  };
}

export function setFilterUrl(filter) {
  const defaultFilter = FilesFilter.getDefault();
  const params = [];

  if (filter.filterType) {
    params.push(`${FILTER_TYPE}=${filter.filterType}`);
  }

  if (filter.withSubfolders === 'false') {
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

  params.push(`${PAGE}=${filter.page + 1}`);
  params.push(`${SORT_BY}=${filter.sortBy}`);
  params.push(`${SORT_ORDER}=${filter.sortOrder}`);

  history.push(`${config.homepage}/filter?${params.join("&")}`);
}

// TODO: similar to fetchFolder, remove one
export function fetchFiles(folderId, filter, dispatch) {
  const filterData = filter ? filter.clone() : FilesFilter.getDefault();
  filterData.folder = folderId;
  return files.getFolder(folderId, filter).then(data => {
    filterData.treeFolders = getTreeFolders(data.pathParts, filterData);
    filterData.total = data.total;
    dispatch(setFilesFilter(filterData));
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    dispatch(setSelected("close"));
    return dispatch(setSelectedFolder({ folders: data.folders, ...data.current, pathParts: data.pathParts }));
  })
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
    return dispatch(setSelectedFolder({ folders: data.folders, ...data.current, pathParts: data.pathParts }));
  })
}

export function fetchMyFolder(dispatch) {
  return files.getMyFolderList().then(data => {
    dispatch(setFolders(data.folders));
    dispatch(setFiles(data.files));
    return dispatch(setSelectedFolder(data.current));
  });
}

export function getFolder(folderId) {
  return dispatch => {
    return files.getFolder(folderId);
  };
};

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

export function fetchTreeFolders(dispatch) {
  return files.getFoldersTree().then(data => dispatch(setTreeFolders(data)));
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
  return dispatch => {
    return files.createFile(folderId, title)
      .then(folder => {
        fetchFolder(folderId, dispatch);
      });
  };
}

export function createFolder(parentFolderId, title) {
  return dispatch => {
    return files.createFolder(parentFolderId, title)
      .then(folder => {
        fetchFolder(parentFolderId, dispatch);
      });
  };
}

export function updateFile(fileId, title) {
  return dispatch => {
    return files.updateFile(fileId, title)
      .then(file => {
        dispatch(setFile(file));
      });
  };
}

export function renameFolder(folderId, title) {
  return dispatch => {
    return files.renameFolder(folderId, title)
      .then(folder => {
        dispatch(setFolder(folder));
      });
  };
}

export function deleteFile(fileId, deleteAfter, immediately) {
  return dispatch => {
    return files.deleteFile(fileId, deleteAfter, immediately)
  }
}

export function deleteFolder(folderId, deleteAfter, immediately) {
  return (dispatch, getState) => {
    const { files } = getState();
    const { folders } = files;

    return api.files.deleteFolder(folderId, deleteAfter, immediately)
      .then(res => {
        return dispatch(setFolder(folders.filter(f => f.id !== folderId)));
      })
  }
}

export function setShareFiles(folderIds, fileIds, share, notify, sharingMessage) {
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
  const foldersRequests = folderIds.map(folderId => files.getShareFolders(folderId));
  const filesRequests = fileIds.map(fileId => files.getShareFiles(fileId));
  const requests = [...foldersRequests, ...filesRequests];

  return axios.all(requests).then(res => res);
}

export function getProgress() {
  return dispatch => {
    return files.getProgress();
  };
};

export function copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) {
  return dispatch => {
    return files.copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
  };
};

export function moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) {
  return dispatch => {
    return files.moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
  };
};

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