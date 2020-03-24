import { find, filter } from "lodash";
import { constants, store } from 'asc-web-common';

const { FileType, FilterType, FolderType } = constants;
const { isAdmin } = store.auth.selectors;

export const canWebEdit = fileExst => {
  const editedDocs = ['.pptx', '.pptm', '.ppt', '.ppsx', '.ppsm', '.pps', '.potx', '.potm', '.pot', '.odp', '.fodp', '.otp', '.xlsx', '.xlsm', '.xls', '.xltx', '.xltm', '.xlt', '.ods', '.fods', '.ots', '.csv', '.docx', '.docm', '.doc', '.dotx', '.dotm', '.dot', '.odt', '.fodt', '.ott', '.txt', '.rtf', '.mht', '.html', '.htm'];
  const result = editedDocs.findIndex(item => item === fileExst);
  return result === -1 ? false : true;
}

export const canConvert = fileExst => {
  const convertedDocs = ['.pptm', '.ppt', '.ppsm', '.pps', '.potx', '.potm', '.pot', '.odp', '.fodp', '.otp', '.xlsm', '.xls', '.xltx', '.xltm', '.xlt', '.ods', '.fods', '.ots', '.docm', '.doc', '.dotx', '.dotm', '.dot', '.odt', '.fodt', '.ott', '.rtf'];
  const result = convertedDocs.findIndex(item => item === fileExst);
  return result === -1 ? false : true;
}

export function getSelectedFile(selection, fileId, parentId) {
  return find(selection, function (obj) {
    return obj.id === fileId && obj.parentId === parentId;
  });
};

export function isFileSelected(selection, fileId, parentId) {
  return getSelectedFile(selection, fileId, parentId) !== undefined;
};

export function skipFile(selection, fileId) {
  return filter(selection, function (obj) {
    return obj.id !== fileId;
  });
};

export function getFilesBySelected(files, selected) {
  let newSelection = [];
  files.forEach(file => {
    const checked = getFilesChecked(file, selected);

    if (checked)
      newSelection.push(file);
  });

  return newSelection;
};

const getFilesChecked = (file, selected) => {
  const type = file.fileType;
  switch (selected) {
    case "all":
      return true;
    case FilterType.FoldersOnly.toString():
      return file.parentId;
    case FilterType.DocumentsOnly.toString():
      return type === FileType.Document;
    case FilterType.PresentationsOnly.toString():
      return type === FileType.Presentation;
    case FilterType.SpreadsheetsOnly.toString():
      return type === FileType.Spreadsheet;
    case FilterType.ImagesOnly.toString():
      return type === FileType.Image;
    case FilterType.MediaOnly.toString():
      return type === FileType.Video || type === FileType.Audio;
    case FilterType.ArchiveOnly.toString():
      return type === FileType.Archive;
    case FilterType.FilesOnly.toString():
      return type || !file.parentId;
    default:
      return false;
  }
};

export const getTitleWithoutExst = item => {
  return item.fileExst
    ? item.title.split('.').slice(0, -1).join('.')
    : item.title;
}

export const getTreeFolders = (pathParts, filterData) => {
  let treeFolders = [];
  if (pathParts.length > 0) {
    for (let item of pathParts) {
      treeFolders.push(item.toString());
    }
  }
  if (treeFolders.length > 0) {
    treeFolders = treeFolders.concat(
      filterData.treeFolders.filter(x => !treeFolders.includes(x))
    );
  }
  return treeFolders;
};

export const isCanCreate = (selectedFolder, user) => {
  if (!selectedFolder || !selectedFolder.id) return;

  const admin = isAdmin(user);
  const rootFolderType = selectedFolder.rootFolderType;

  switch (rootFolderType) {
    case FolderType.USER:
      return true;
    case FolderType.SHARE:
      const { pathParts, access } = selectedFolder;
      const isNotRootFolder = pathParts.length > 1;
      const canCreateinSharedFolder = access === 1;
      return isNotRootFolder && canCreateinSharedFolder;
    case FolderType.COMMON:
      return admin;
    case FolderType.TRASH:
    default:
      return false;
  }
};

export const isCanBeDeleted = (selectedFolder, user) => {
  const admin = isAdmin(user);
  const rootFolderType = selectedFolder.rootFolderType;

  switch (rootFolderType) {
    case FolderType.USER:
      return true;
    case FolderType.SHARE:
      return false;
    case FolderType.COMMON:
      return admin;
    case FolderType.TRASH:
      return true;
    default:
      return false;
  }
};
