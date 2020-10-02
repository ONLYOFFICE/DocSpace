import { find, filter } from 'lodash';
import { constants, store } from 'asc-web-common';

const { FileType, FilterType, FolderType } = constants;
const { isAdmin } = store.auth.selectors;

const presentInArray = (array, search) => {
  const result = array.findIndex(item => item === search);
  return result === -1 ? false : true;
}

export const canWebEdit = extension => {
  const formats = ['.pptx', '.pptm', '.ppt', '.ppsx', '.ppsm', '.pps',
    '.potx', '.potm', '.pot', '.odp', '.fodp', '.otp', '.xlsx', '.xlsm',
    '.xls', '.xltx', '.xltm', '.xlt', '.ods', '.fods', '.ots', '.csv',
    '.docx', '.docm', '.doc', '.dotx', '.dotm', '.dot', '.odt', '.fodt',
    '.ott', '.txt', '.rtf', '.mht', '.html', '.htm'];
  return presentInArray(formats, extension);
}

export const canConvert = extension => {
  const formats = ['.pptm', '.ppt', '.ppsm', '.pps', '.potx', '.potm',
    '.pot', '.odp', '.fodp', '.otp', '.xlsm', '.xls', '.xltx', '.xltm',
    '.xlt', '.ods', '.fods', '.ots', '.docm', '.doc', '.dotx', '.dotm',
    '.dot', '.odt', '.fodt', '.ott', '.rtf'];
  return presentInArray(formats, extension);
}

export const isArchive = extension => {
  const formats = ['.zip', '.rar', '.ace', '.arc', '.arj', '.bh', '.cab',
    '.enc', '.gz', '.ha', '.jar', '.lha', '.lzh', '.pak', '.pk3', '.tar',
    '.tgz', '.gz', '.uu', '.uue', '.xxe', '.z', '.zoo'];
  return presentInArray(formats, extension);
}

export const isImage = extension => {
  const formats = ['.bmp', '.cod', '.gif', '.ief', '.jpe', '.jpg', '.tif',
    '.cmx', '.ico', '.pnm', '.pbm', '.ppm', '.psd', '.rgb', '.xbm', '.xpm',
    '.xwd', '.png', '.ai', '.jpeg'];
  return presentInArray(formats, extension);
}

export const isSound = extension => {
  const formats = ['.aac', '.ac3', '.aiff', '.amr', '.ape', '.cda', '.flac',
    '.m4a', '.mid', '.mka', '.mp3', '.mpc', '.oga', '.ogg', '.pcm', '.ra',
    '.raw', '.wav', '.wma'];
  return presentInArray(formats, extension);
}

export const isVideo = extension => {
  const formats = ['.3gp', '.asf', '.avi', '.f4v', '.fla', '.flv', '.m2ts',
    '.m4v', '.mkv', '.mov', '.mp4', '.mpeg', '.mpg', '.mts', '.ogv', '.svi',
    '.vob', '.webm', '.wmv'];
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

export const isDocument = extension => {
  const formats = ['.doc', '.docx', '.docm', '.dot', '.dotx', '.dotm', '.odt',
    '.fodt', '.ott', '.rtf', '.txt', '.html', '.htm', '.mht', '.pdf', '.djvu',
    '.fb2', '.epub', '.xps', '.doct', '.docy', '.gdoc'];
  return presentInArray(formats, extension);
}

export const isPresentation = extension => {
  const formats = ['.pps', '.ppsx', '.ppsm', '.ppt', '.pptx', '.pptm', '.pot',
    '.potx', '.potm', '.odp', '.fodp', '.otp', '.pptt', '.ppty', '.gslides'];
  return presentInArray(formats, extension);
}

export const isSpreadsheet = extension => {
  const formats = ['.xls', '.xlsx', '.xlsm', '.xlt', '.xltx', '.xltm', '.ods',
    '.fods', '.ots', '.csv', '.xlst', '.xlsy', '.xlsb', '.gsheet'];
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
    case 'all':
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

const renameTreeFolder = (folders, newItems, currentFolder) => {
  const newItem = folders.find(x => x.id === currentFolder.id);
  const oldItemIndex = newItems.folders.findIndex(x => x.id === currentFolder.id);
  newItem.folders = newItems.folders[oldItemIndex].folders;
  newItems.folders[oldItemIndex] = newItem;

  return;
}

const removeTreeFolder = (folders, newItems, foldersCount) => {
  const newFolders = JSON.parse(JSON.stringify(newItems.folders));
  for (let folder of newFolders) {
    let currentFolder;
    if(folders) {
      currentFolder = folders.find((x) => x.id === folder.id);
    }

    if (!currentFolder) {
      const arrayFolders = newItems.folders.filter(x => x.id !== folder.id);
      newItems.folders = arrayFolders;
      newItems.foldersCount = foldersCount;
    }
  }
}

const addTreeFolder = (folders, newItems, foldersCount) => {
  let array;
  let newItemFolders = newItems.folders ? newItems.folders : [];
  for (let folder of folders) {
    let currentFolder;
    if(newItemFolders) {
      currentFolder = newItemFolders.find((x) => x.id === folder.id);
    }

    if (folders.length < 1 || !currentFolder) {
      array = [...newItemFolders, ...[folder]].sort((prev, next) =>
        prev.title.toLowerCase() < next.title.toLowerCase() ? -1 : 1
      );
      newItems.folders = array;
      newItemFolders = array;
      newItems.foldersCount = foldersCount;
    }
  }
}

export const loopTreeFolders = (path, item, folders, foldersCount, currentFolder) => {
  const newPath = path;
  while (path.length !== 0) {
    const newItems = item.find((x) => x.id === path[0]);
    if(!newItems) { return; }
    newPath.shift();
    if (path.length === 0) {
      let foldersLength = newItems.folders ? newItems.folders.length : 0;
      //new Date(prev.updated) > new Date(next.updated) ? -1 : 1;
      if (folders.length > foldersLength) {
        addTreeFolder(folders, newItems, foldersCount);
      } else if (folders.length < foldersLength) {
        removeTreeFolder(folders, newItems, foldersCount);
      } else if (folders.length > 0 && newItems.length > 0) {
        renameTreeFolder(folders, newItems, currentFolder);
      } else {
        return;
      }
      return;
    }
    loopTreeFolders(newPath, newItems.folders, folders, foldersCount, currentFolder);
  }
}

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
  const isMedia = selection.find(x =>
    isSound(x.fileExst) || isVideo(x.fileExst)
  );
  const isPresentationOrTable = selection.find(x =>
    isSpreadsheet(x.fileExst) || isPresentation(x.fileExst)
  );

  if (isFolder || isMedia) {
    return ['FullAccess', 'ReadOnly', 'DenyAccess'];
  } else if (isPresentationOrTable) {
    return ['FullAccess', 'ReadOnly', 'DenyAccess', 'Comment'];
  } else {
    return ['FullAccess', 'ReadOnly', 'DenyAccess', 'Comment', 'Review', 'FormFilling'];
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

export const checkFolderType = (id, index, treeFolders) => {
  return treeFolders.length && treeFolders[index].id === id;
}

export const getFolderType = (id, treeFolders) => {
  
  const indexOfMy = 0;
  const indexOfShare = 1;
  const indexOfCommon = 2;
  const indexOfTrash = 3;
  const indexOfFavorites = 4;

  if(checkFolderType(id, indexOfMy, treeFolders)) { return "My"; }
  else if(checkFolderType(id, indexOfShare, treeFolders)) { return "Share"; }
  else if(checkFolderType(id, indexOfCommon, treeFolders)) { return "Common"; }
  else if(checkFolderType(id, indexOfTrash, treeFolders)) { return "Trash"; }
  else if(checkFolderType(id, indexOfFavorites, treeFolders)) { return "Favorites"; }
}

export const selectFavoriteFilesIds = (data) => {
    return Object.values(data.files)
    .map(item => item.id);
}

export const isFavorite = (data, id) => {
  return selectFavoriteFilesIds(data)
  .find(value => value === id)
}
