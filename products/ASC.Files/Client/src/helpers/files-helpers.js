export const presentInArray = (array, search, caseInsensitive = false) => {
  let pattern = caseInsensitive ? search.toLowerCase() : search;
  const result = array.findIndex((item) => item === pattern);
  return result === -1 ? false : true;
};

export const getAccessIcon = (access) => {
  switch (access) {
    case 1:
      return "AccessEditIcon";
    case 2:
      return "EyeIcon";
    case 3:
      return "AccessNoneIcon";
    case 4:
      return "CatalogQuestionIcon";
    case 5:
      return "AccessReviewIcon";
    case 6:
      return "AccessCommentIcon";
    case 7:
      return "AccessFormIcon";
    case 8:
      return "CustomFilterIcon";
    default:
      return;
  }
};

export const getTitleWithoutExst = (item) => {
  return item.fileExst
    ? item.title.split(".").slice(0, -1).join(".")
    : item.title;
};

export const createTreeFolders = (pathParts, expandedKeys) => {
  let treeFolders = [];
  if (pathParts.length > 0) {
    for (let item of pathParts) {
      treeFolders.push(item.toString());
    }
  }
  if (treeFolders.length > 0) {
    treeFolders = treeFolders.concat(
      expandedKeys.filter((x) => !treeFolders.includes(x))
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
      } else if (
        folders.length > 0 &&
        newItems.folders.length > 0 &&
        currentFolder
      ) {
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
