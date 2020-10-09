import { find, filter } from "lodash";
import { constants, store } from "asc-web-common";
import { createSelector } from "reselect";

const { FileType, FilterType, FolderType } = constants;
const { isAdmin } = store.auth.selectors;

const presentInArray = (array, search) => {
  const result = array.findIndex((item) => item === search);
  return result === -1 ? false : true;
};

export const getMediaViewerImageFormats = (state) => {
  return state.files.mediaViewerFormats.images;
};

export const getMediaViewerMediaFormats = (state) => {
  return state.files.mediaViewerFormats.media;
};

export const getEditedFormats = state => {
  return state.files.docservice.editedDocs;
};

export const getConvertedFormats = state => {
  return state.files.docservice.convertDocs;
};

export const getArchiveFormats = state => {
  return state.files.formats.archive;
};

export const getImageFormats = state => {
  return state.files.formats.image;
};

export const getSoundFormats = state => {
  return state.files.formats.sound;
};

export const getVideoFormats = state => {
  return state.files.formats.video;
};

export const getHtmlFormats = state => {
  return state.files.formats.html;
};

export const getEbookFormats = state => {
  return state.files.formats.ebook;
};

export const getDocumentFormats = state => {
  return state.files.formats.document;
};

export const getPresentationFormats = state => {
  return state.files.formats.presentation;
};

export const getSpreadsheetFormats = state => {
  return state.files.formats.spreadsheet;
};

export const canWebEdit = (extension) => {
  return createSelector(
    getEditedFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const canConvert = (extension) => {
  return createSelector(
    getConvertedFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isArchive = (extension) => {
  return createSelector(
    getArchiveFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isImage = (extension) => {
  return createSelector(
    getImageFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isSound = (extension) => {
  return createSelector(
    getSoundFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isVideo = (extension) => {
  return createSelector(
    getVideoFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isHtml = (extension) => {
  return createSelector(
    getHtmlFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isEbook = (extension) => {
  return createSelector(
    getEbookFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isDocument = (extension) => {
  return createSelector(
    getDocumentFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isPresentation = (extension) => {
  return createSelector(
    getPresentationFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export const isSpreadsheet = (extension) => {
  return createSelector(
    getSpreadsheetFormats,
    (formats) => {
      return presentInArray(formats, extension);
    }
  )
};

export function getSelectedFile(selection, fileId, parentId) {
  return find(selection, function (obj) {
    return obj.id === fileId && obj.parentId === parentId;
  });
}

export function isFileSelected(selection, fileId, parentId) {
  return getSelectedFile(selection, fileId, parentId) !== undefined;
}

export function skipFile(selection, fileId) {
  return filter(selection, function (obj) {
    return obj.id !== fileId;
  });
}

export function getFilesBySelected(files, selected) {
  let newSelection = [];
  files.forEach((file) => {
    const checked = getFilesChecked(file, selected);

    if (checked) newSelection.push(file);
  });

  return newSelection;
}

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

export const getTitleWithoutExst = (item) => {
  return item.fileExst
    ? item.title.split(".").slice(0, -1).join(".")
    : item.title;
};

export const createTreeFolders = (pathParts, filterData) => {
  let treeFolders = [];
  if (pathParts.length > 0) {
    for (let item of pathParts) {
      treeFolders.push(item.toString());
    }
  }
  if (treeFolders.length > 0) {
    treeFolders = treeFolders.concat(
      filterData.treeFolders.filter((x) => !treeFolders.includes(x))
    );
  }
  return treeFolders;
};

const renameTreeFolder = (folders, newItems, currentFolder) => {
  const newItem = folders.find((x) => x.id === currentFolder.id);
  const oldItemIndex = newItems.folders.findIndex(
    (x) => x.id === currentFolder.id
  );
  newItem.folders = newItems.folders[oldItemIndex].folders;
  newItems.folders[oldItemIndex] = newItem;

  return;
};

const removeTreeFolder = (folders, newItems, foldersCount) => {
  const newFolders = JSON.parse(JSON.stringify(newItems.folders));
  for (let folder of newFolders) {
    let currentFolder;
    if (folders) {
      currentFolder = folders.find((x) => x.id === folder.id);
    }

    if (!currentFolder) {
      const arrayFolders = newItems.folders.filter((x) => x.id !== folder.id);
      newItems.folders = arrayFolders;
      newItems.foldersCount = foldersCount;
    }
  }
};

const addTreeFolder = (folders, newItems, foldersCount) => {
  let array;
  let newItemFolders = newItems.folders ? newItems.folders : [];
  for (let folder of folders) {
    let currentFolder;
    if (newItemFolders) {
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
};

export const loopTreeFolders = (
  path,
  item,
  folders,
  foldersCount,
  currentFolder
) => {
  const newPath = path;
  while (path.length !== 0) {
    const newItems = item.find((x) => x.id === path[0]);
    if (!newItems) {
      return;
    }
    newPath.shift();
    if (path.length === 0) {
      let foldersLength = newItems.folders ? newItems.folders.length : 0;
      if (folders.length > foldersLength) {
        addTreeFolder(folders, newItems, foldersCount);
      } else if (folders.length < foldersLength) {
        removeTreeFolder(folders, newItems, foldersCount);
      } else if (folders.length > 0 && newItems.folders.length > 0 && currentFolder) {
        renameTreeFolder(folders, newItems, currentFolder);
      } else {
        return;
      }
      return;
    }
    loopTreeFolders(
      newPath,
      newItems.folders,
      folders,
      foldersCount,
      currentFolder
    );
  }
};

export const getSelectedFolder = (state) => {
  return state.files.selectedFolder;
};

export const getSelectedFolderId = (state) => {
  return state.files.selectedFolder.id;
};

export const getSelectedFolderParentId = (state) => {
  return state.files.selectedFolder.parentId;
};

export const getSelectedFolderNew = (state) => {
  return state.files.selectedFolder.new;
};

export const getSelectedFolderTitle = (state) => {
  return state.files.selectedFolder.title;
};

export const getPathParts = (state) => {
  return state.files.selectedFolder.pathParts;
};

export const getSelectedFolderRootFolderType = (state) => {
  return state.files.selectedFolder.rootFolderType;
};

const getSelectedFolderAccess = (state) => {
  return state.files.selectedFolder.access;
};

export const getIsRootFolder = (state) => {
  return state.files.selectedFolder.parentId === 0;
};

export const getRootFolderId = (state) => {
  if (state.files.selectedFolder.pathParts)
    return state.files.selectedFolder.pathParts[0];
};

export const isCanCreate = createSelector(
  getSelectedFolderRootFolderType,
  isAdmin,
  getPathParts,
  getSelectedFolderAccess,
  (folderType, isAdmin, pathParts, access) => {
    switch (folderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        const isNotRootFolder = pathParts.length > 1;
        const canCreateInSharedFolder = access === 1;
        return isNotRootFolder && canCreateInSharedFolder;
      case FolderType.COMMON:
        return isAdmin;
      case FolderType.TRASH:
      default:
        return false;
    }
  }
);

export const isCanBeDeleted = createSelector(
  getSelectedFolderRootFolderType, isAdmin,
  (folderType, isAdmin) => {
    switch (folderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return false;
      case FolderType.COMMON:
        return isAdmin;
      case FolderType.TRASH:
        return true;
      default:
        return false;
    }
  }
);

//TODO: Get the whole list of extensions
export const getAccessOption = (selection) => {
  const isFolder = selection.find((x) => x.fileExst === undefined);
  const isMedia = selection.find(
    (x) => isSound(x.fileExst) || isVideo(x.fileExst)
  );
  const isPresentationOrTable = selection.find(
    (x) => isSpreadsheet(x.fileExst) || isPresentation(x.fileExst)
  );

  if (isFolder || isMedia) {
    return ["FullAccess", "ReadOnly", "DenyAccess"];
  } else if (isPresentationOrTable) {
    return ["FullAccess", "ReadOnly", "DenyAccess", "Comment"];
  } else {
    return [
      "FullAccess",
      "ReadOnly",
      "DenyAccess",
      "Comment",
      "Review",
      "FormFilling",
    ];
  }
};

export const getFolderIcon = (providerKey, size = 32) => {
  const folderPath = `images/icons/${size}`;

  switch (providerKey) {
    case "Box":
    case "BoxNet":
      return `${folderPath}/folder/box.svg`;
    case "DropBox":
    case "DropboxV2":
      return `${folderPath}/folder/dropbox.svg`;
    case "Google":
    case "GoogleDrive":
      return `${folderPath}/folder/google.svg`;
    case "OneDrive":
      return `${folderPath}/folder/onedrive.svg`;
    case "SharePoint":
      return `${folderPath}/folder/sharepoint.svg`;
    case "Yandex":
      return `${folderPath}/Folder/yandex.svg`;
    default:
      return `${folderPath}/folder.svg`;
  }
};

export const getFileIcon = (
  extension,
  size = 32,
  archive = false,
  image = false,
  sound = false,
  ebook = false,
  html = false
) => {
  const folderPath = `images/icons/${size}`;

  if (archive) return `${folderPath}/file_archive.svg`;

  if (image) return `${folderPath}/image.svg`;

  if (sound) return `${folderPath}/sound.svg`;

  if (ebook) return `${folderPath}/ebook.svg`;

  if (html) return `${folderPath}/html.svg`;

  switch (extension) {
    case ".avi":
      return `${folderPath}/avi.svg`;
    case ".csv":
      return `${folderPath}/csv.svg`;
    case ".djvu":
      return `${folderPath}/djvu.svg`;
    case ".doc":
      return `${folderPath}/doc.svg`;
    case ".docx":
      return `${folderPath}/docx.svg`;
    case ".dvd":
      return `${folderPath}/dvd.svg`;
    case ".flv":
      return `${folderPath}/flv.svg`;
    case ".iaf":
      return `${folderPath}/iaf.svg`;
    case ".m2ts":
      return `${folderPath}/m2ts.svg`;
    case ".mkv":
      return `${folderPath}/mkv.svg`;
    case ".mov":
      return `${folderPath}/mov.svg`;
    case ".mp4":
      return `${folderPath}/mp4.svg`;
    case ".mpg":
      return `${folderPath}/mpg.svg`;
    case ".odp":
      return `${folderPath}/odp.svg`;
    case ".ods":
      return `${folderPath}/ods.svg`;
    case ".odt":
      return `${folderPath}/odt.svg`;
    case ".pdf":
      return `${folderPath}/pdf.svg`;
    case ".pps":
      return `${folderPath}/pps.svg`;
    case ".ppsx":
      return `${folderPath}/ppsx.svg`;
    case ".ppt":
      return `${folderPath}/ppt.svg`;
    case ".pptx":
      return `${folderPath}/pptx.svg`;
    case ".rtf":
      return `${folderPath}/rtf.svg`;
    case ".svg":
      return `${folderPath}/svg.svg`;
    case ".txt":
      return `${folderPath}/txt.svg`;
    case ".xls":
      return `${folderPath}/xls.svg`;
    case ".xlsx":
      return `${folderPath}/xlsx.svg`;
    case ".xps":
      return `${folderPath}/xps.svg`;
    default:
      return `${folderPath}/file.svg`;
  }
};

export const getFileAction = (state) => {
  return state.files.fileAction;
};

export const getFiles = (state) => {
  return state.files.files;
};

export const getFolders = (state) => {
  return state.files.folders;
};

export const getFilter = (state) => {
  return state.files.filter;
};

export const getNewRowItems = (state) => {
  return state.files.newRowItems;
};

export const getSelected = (state) => {
  return state.files.selected;
};

export const getSelection = (state) => {
  return state.files.selection;
};

export const getViewAs = (state) => {
  return state.files.viewAs;
};

export const getTreeFolders = (state) => {
  return state.files.treeFolders;
};

export const getCurrentFolderCount = (state) => {
  const { filesCount, foldersCount } = state.files.selectedFolder;
  return filesCount + foldersCount;
};

export const getDragItem = (state) => {
  return state.files.dragItem;
};

export const getMediaViewerVisibility = (state) => {
  return state.files.mediaViewerData.visible;
};

export const getMediaViewerId = (state) => {
  return state.files.mediaViewerData.id;
};

export const getDragging = (state) => {
  return state.files.dragging;
};

export const getIsLoading = (state) => {
  return state.files.isLoading;
};

export const getFirstLoad = (state) => {
  return state.files.firstLoad;
};

export const isMediaOrImage = (fileExst) => {
  return createSelector(
    [getMediaViewerImageFormats, getMediaViewerMediaFormats],
    (media, images) => {
      if (
        media.includes(fileExst) ||
        images.includes(fileExst)
      ) {
        return true;
      }
      return false;
    }
  )
};

const getFilesContextOptions = (item, isRecycleBin, canOpenPlayer) => {
  const options = [];

  const isFile = !!item.fileExst;

  if (item.id <= 0) return [];

  if (isRecycleBin) {
    options.push("download");
    options.push("download-as");
    options.push("restore");
    options.push("separator2");
    options.push("delete");
  } else {
    options.push("sharing-settings");

    if (isFile) {
      options.push("send-by-email");
    }

    options.push("link-for-portal-users");
    options.push("separator0");

    if (isFile) {
      options.push("show-version-history");
      options.push("finalize-version");
      options.push("block-unblock-version");
      options.push("separator1");

      if (canOpenPlayer) {
        options.push("view");
      } else {
        options.push("edit");
        options.push("preview");
      }

      options.push("download");
    }

    options.push("move");
    options.push("copy");

    if (isFile) {
      options.push("duplicate");
    }

    options.push("rename");
    options.push("delete");
  }
  return options;
};

export const getItemsList = createSelector(
  [getFolders, getFiles],
  (folders, files) => {
    return folders && files ? [...folders, ...files] : [];
  }
);

const getMyFolder = createSelector(getTreeFolders, (treeFolders) => {
  return treeFolders.find((x) => x.rootFolderName === "@my");
});

const getShareFolder = createSelector(getTreeFolders, (treeFolders) => {
  return treeFolders.find((x) => x.rootFolderName === "@share");
});

const getCommonFolder = createSelector(getTreeFolders, (treeFolders) => {
  return treeFolders.find((x) => x.rootFolderName === "@common");
});

const getRecycleBinFolder = createSelector(getTreeFolders, (treeFolders) => {
  return treeFolders.find((x) => x.rootFolderName === "@trash");
});

export const getMyFolderId = createSelector(getMyFolder, (myFolder) => {
  if (myFolder) return myFolder.id;
});

export const getShareFolderId = createSelector(
  getShareFolder,
  (shareFolder) => {
    if (shareFolder) return shareFolder.id;
  }
);

export const getCommonFolderId = createSelector(
  getCommonFolder,
  (commonFolder) => {
    if (commonFolder) return commonFolder.id;
  }
);

export const getRecycleBinFolderId = createSelector(
  getRecycleBinFolder,
  (recycleBinFolder) => {
    if (recycleBinFolder) return recycleBinFolder.id;
  }
);

export const getIsMyFolder = createSelector(
  getMyFolder,
  getSelectedFolderId,
  (myFolder, id) => {
    return myFolder && myFolder.id === id;
  }
);

export const getIsShareFolder = createSelector(
  getShareFolder,
  getSelectedFolderId,
  (shareFolder, id) => {
    return shareFolder && shareFolder.id === id;
  }
);

export const getIsCommonFolder = createSelector(
  getCommonFolder,
  getSelectedFolderId,
  (commonFolder, id) => {
    return commonFolder && commonFolder.id === id;
  }
);

export const getIsRecycleBinFolder = createSelector(
  getRecycleBinFolder,
  getSelectedFolderId,
  (recycleBinFolder, id) => {
    return recycleBinFolder && recycleBinFolder.id === id;
  }
);

export const getFileActionId = (state) => {
  return state.files.fileAction.id;
};

export const getFilesList = (state) => {
  return createSelector(
    [getItemsList, getSelection, getIsRecycleBinFolder, getFileActionId],
    (items, selection, isRecycleBin, actionId) => {
      return items.map((item) => {
        const {
          access,
          comment,
          contentLength,
          created,
          createdBy,
          fileExst,
          filesCount,
          fileStatus,
          fileType,
          folderId,
          foldersCount,
          id,
          locked,
          parentId,
          pureContentLength,
          rootFolderType,
          shared,
          title,
          updated,
          updatedBu,
          version,
          versionGroup,
          viewUrl,
          webUrl,
          providerKey,
        } = item;
        const canOpenPlayer = isMediaOrImage(item.fileExst)(state);
        const contextOptions = getFilesContextOptions(item, isRecycleBin, canOpenPlayer);
        const checked = isFileSelected(selection, id, parentId);

        const selectedItem = selection.find(
          (x) => x.id === id && x.fileExst === fileExst
        );

        const isFolder = selectedItem ? false : fileExst ? false : true;

        const draggable = selectedItem && !isRecycleBin && selectedItem.id !== actionId;

        let value = fileExst ? `file_${id}` : `folder_${id}`;

        const isArchiveItem = isArchive(item.fileExst)(state);
        const isImageItem = isImage(item.fileExst)(state);
        const isSoundItem = isSound(item.fileExst)(state);
        const isEbookItem = isEbook(item.fileExst)(state);
        const isHtmlItem = isHtml(item.fileExst)(state);

        const icon = fileExst
          ? getFileIcon(fileExst, 24, isArchiveItem, isImageItem, isSoundItem, isEbookItem, isHtmlItem)
          : getFolderIcon(providerKey, 24);

        value += draggable ? "_draggable" : "";

        return {
          access,
          checked,
          comment,
          contentLength,
          contextOptions,
          created,
          createdBy,
          fileExst,
          filesCount,
          fileStatus,
          fileType,
          folderId,
          foldersCount,
          icon,
          id,
          isFolder,
          locked,
          new: item.new,
          parentId,
          pureContentLength,
          rootFolderType,
          selectedItem,
          shared,
          title,
          updated,
          updatedBu,
          value,
          version,
          versionGroup,
          viewUrl,
          webUrl,
          providerKey,
          draggable,
          canOpenPlayer
        };
      });
    }
  );
};

export const getSelectedTreeNode = createSelector(getSelectedFolderId, (id) => {
  if (id) return [id.toString()];
});

export const getConvertDialogVisible = (state) => {
  return state.files.convertDialogVisible;
};

export const getProgressData = (state) => {
  return state.files.progressData;
};

export const getUpdateTree = (state) => {
  return state.files.updateTree;
};

export const getSettingsSelectedTreeNode = (state) => {
  return state.files.selectedTreeNode;
};

export const getSettingsTreeStoreOriginalFiles = (state) => {
  return state.files.settingsTree.storeOriginalFiles;
};

export const getSettingsTreeConfirmDelete = (state) => {
  return state.files.settingsTree.confirmDelete;
};

export const getSettingsTreeUpdateIfExist = (state) => {
  return state.files.settingsTree.updateIfExist;
};

export const getSettingsTreeForceSave = (state) => {
  return state.files.settingsTree.forceSave;
};

export const getSettingsTreeStoreForceSave = (state) => {
  return state.files.settingsTree.storeForceSave;
};

export const getSettingsTreeEnableThirdParty = (state) => {
  return state.files.settingsTree.enableThirdParty;
};

export const getExpandedSetting = (state) => {
  return state.files.settingsTree.expandedSetting;
};

export const getEnableThirdParty = (state) => {
  return state.files.settingsTree.enableThirdParty;
};

export const getFilterSelectedItem = (state) => {
  return state.files.filter.selectedItem;
};

export const getSelectionLength = (state) => {
  return state.files.selection.length;
};

export const getHeaderVisible = (state) => {
  return state.files.selection.length > 0;
};

export const getHeaderIndeterminate = createSelector(
  getHeaderVisible,
  getSelection,
  getItemsList,
  (headerVisible, selection, items) => {
    return headerVisible && selection.length < items.length;
  }
);

export const getHeaderChecked = createSelector(
  getHeaderVisible,
  getSelection,
  getItemsList,
  (headerVisible, selection, items) => {
    return headerVisible && selection.length === items.length;
  }
);
