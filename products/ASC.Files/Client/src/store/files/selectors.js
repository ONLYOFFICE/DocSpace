import { find, filter } from "lodash";
import { constants } from 'asc-web-common';

const { FileType, FilterType } = constants;

export const getRootFolders = files => {
  const { my, share, common, project, trash } = files;

  const data = [
    {
      id: my.id.toString(),
      key: "0-0",
      title: my.title,
      foldersCount: my.folders.length
    },
    {
      id: share.id.toString(),
      key: "0-1",
      title: share.title,
      foldersCount: share.folders.length
    },
    {
      id: common.id.toString(),
      key: "0-2",
      title: common.title,
      foldersCount: common.folders.length
    },
    {
      id: project.id.toString(),
      key: "0-3",
      title: project.title,
      foldersCount: project.folders.length
    },
    {
      id: trash.id.toString(),
      key: "0-4",
      title: trash.title,
      foldersCount: null
    }
  ];

  return data;
};

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
