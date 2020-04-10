import { find, filter } from "lodash";
import { constants, store } from 'asc-web-common';

const { FileType, FilterType, FolderType } = constants;
const { isAdmin } = store.auth.selectors;

const presentInArray = (array, search) => {
  const result = array.findIndex(item => item === search);
  return result === -1 ? false : true;
}

export const canWebEdit = extension => {
  const formats = ['.pptx', '.pptm', '.ppt', '.ppsx', '.ppsm', '.pps', '.potx', '.potm', '.pot', '.odp', '.fodp', '.otp', '.xlsx', '.xlsm', '.xls', '.xltx', '.xltm', '.xlt', '.ods', '.fods', '.ots', '.csv', '.docx', '.docm', '.doc', '.dotx', '.dotm', '.dot', '.odt', '.fodt', '.ott', '.txt', '.rtf', '.mht', '.html', '.htm'];
  return presentInArray(formats, extension);
}

export const canConvert = extension => {
  const formats = ['.pptm', '.ppt', '.ppsm', '.pps', '.potx', '.potm', '.pot', '.odp', '.fodp', '.otp', '.xlsm', '.xls', '.xltx', '.xltm', '.xlt', '.ods', '.fods', '.ots', '.docm', '.doc', '.dotx', '.dotm', '.dot', '.odt', '.fodt', '.ott', '.rtf'];
  return presentInArray(formats, extension);
}

export const isArchive = extension => {
  const formats = ['.zip', '.rar', '.ace', '.arc', '.arj', '.cab', '.enc', '.jar', '.lha', '.lzh', '.pak', '.pk3', '.tar', '.tgz', '.uue', '.xxe', '.zoo', '.bh', '.gz', '.ha'];
  return presentInArray(formats, extension);
}

export const isImage = extension => {
  const formats = ['.bmp', '.cod', '.gif', '.ief', '.jpe', '.jpg', '.tif', '.cmx', '.ico', '.pnm', '.pbm', '.ppm', '.psd', '.rgb', '.xbm', '.xpm', '.xwd', '.png', '.ai', '.jpeg'];
  return presentInArray(formats, extension);
}

export const isSound = extension => {
  const formats = ['.mp3', '.wav', '.pcm', '.3gp', '.fla', '.cda', '.ogg', '.aiff', '.flac'];
  return presentInArray(formats, extension);
}

export const isHtml = extension => {
  const formats = ['.htm', '.mht', '.html'];
  return presentInArray(formats, extension);
}

export const isEbook = extension => {
  const formats = ['.fb2', '.ibk', '.prc', '.epub'];
  return presentInArray(formats, extension);
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

//TODO: Get the whole list of extensions
export const getAccessOption = selection => {
  const isFolder = selection.find(x => x.fileExst === undefined);
  const isMedia = selection.find(
    x => x.fileExst === ".mp3" || x.fileExst === ".mp4"
  );
  const isPresentationOrTable = selection.find(x => x.fileExst === ".pptx" || x.fileExst === ".xlsx");

  if (isFolder || isMedia) {
    return ["FullAccess", "ReadOnly", "DenyAccess"];
  } else if (isPresentationOrTable) {
    return ["FullAccess", "ReadOnly", "DenyAccess", "Comment"];
  } else {
    return ["FullAccess", "ReadOnly", "DenyAccess", "Comment", "Review", "FormFilling"];
  }
};

export const getFolderIcon = (providerKey, size = 32) => {
  const folderPath = `images/icons/${size}`

  switch (providerKey) {
    case 'Box':
    case 'BoxNet':
      return `${folderPath}/folder/box.svg`;
    case 'DropBox':
    case 'DropboxV2':
      return `${folderPath}/folder/dropbox.svg`;
    case 'Google':
    case 'GoogleDrive':
      return `${folderPath}/folder/google.svg`;
    case 'OneDrive':
      return `${folderPath}/folder/onedrive.svg`;
    case 'SharePoint':
        return `${folderPath}/folder/sharepoint.svg`;
    case 'Yandex':
      return `${folderPath}/Folder/yandex.svg`;
    default:
      return `${folderPath}/folder.svg`;
  }
}

export const getFileIcon = (extension, size = 32) => {

  const folderPath = `images/icons/${size}`

  if (isArchive(extension))
    return `${folderPath}/file_archive.svg`;

  if (isImage(extension))
    return `${folderPath}/image.svg`;

  if (isSound(extension))
    return `${folderPath}/sound.svg`;

  if (isEbook(extension))
    return `${folderPath}/ebook.svg`;

  if (isHtml(extension))
    return `${folderPath}/html.svg`;

  switch (extension) {
    case '.avi':
      return `${folderPath}/avi.svg`;
    case '.csv':
      return `${folderPath}/csv.svg`;
    case '.djvu':
      return `${folderPath}/djvu.svg`;
    case '.doc':
      return `${folderPath}/doc.svg`;
    case '.docx':
      return `${folderPath}/docx.svg`;
    case '.dvd':
      return `${folderPath}/dvd.svg`;
    case '.flv':
      return `${folderPath}/flv.svg`;
    case '.iaf':
      return `${folderPath}/iaf.svg`;
    case '.m2ts':
      return `${folderPath}/m2ts.svg`;
    case '.mkv':
      return `${folderPath}/mkv.svg`;
    case '.mov':
      return `${folderPath}/mov.svg`;
    case '.mp4':
      return `${folderPath}/mp4.svg`;
    case '.mpg':
      return `${folderPath}/mpg.svg`;
    case '.odp':
      return `${folderPath}/odp.svg`;
    case '.ods':
      return `${folderPath}/ods.svg`;
    case '.odt':
      return `${folderPath}/odt.svg`;
    case '.pdf':
      return `${folderPath}/pdf.svg`;
    case '.pps':
      return `${folderPath}/pps.svg`;
    case '.ppsx':
      return `${folderPath}/ppsx.svg`;
    case '.ppt':
      return `${folderPath}/ppt.svg`;
    case '.pptx':
      return `${folderPath}/pptx.svg`;
    case '.rtf':
      return `${folderPath}/rtf.svg`;
    case '.svg':
      return `${folderPath}/svg.svg`;
    case '.txt':
      return `${folderPath}/txt.svg`;
    case '.xls':
      return `${folderPath}/xls.svg`;
    case '.xlsx':
      return `${folderPath}/xlsx.svg`;
    case '.xps':
      return `${folderPath}/xps.svg`;
    default:
      return `${folderPath}/file.svg`;
  }
}