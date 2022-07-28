import { runInAction } from "mobx";
import { EDITOR_PROTOCOL } from "./filesConstants";
import { combineUrl } from "@docspace/common/utils";
import { homepage } from "PACKAGE_FILE";

export const presentInArray = (array, search, caseInsensitive = false) => {
  let pattern = caseInsensitive ? search.toLowerCase() : search;
  const result = array.findIndex((item) => item === pattern);
  return result === -1 ? false : true;
};

export const getAccessIcon = (access) => {
  switch (access) {
    case 1:
      return "/static/images/access.edit.react.svg";
    case 2:
      return "/static/images/eye.react.svg";
    case 3:
      return "/static/images/access.none.react.svg";
    case 4:
      return "images/catalog.question.react.svg";
    case 5:
      return "/static/images/access.review.react.svg";
    case 6:
      return "/static/images/access.comment.react.svg";
    case 7:
      return "/static/images/access.form.react.svg";
    case 8:
      return "/static/images/custom.filter.react.svg";
    default:
      return;
  }
};

export const getTitleWithoutExst = (item, fromTemplate) => {
  return item.fileExst && !fromTemplate
    ? item.title.split(".").slice(0, -1).join(".")
    : item.title;
};

export const createTreeFolders = (pathParts, expandedKeys) => {
  const newPathParts =
    pathParts.length > 1 ? [...pathParts].splice(0, pathParts.length - 1) : [];

  let treeFolders = [];
  if (newPathParts.length > 0) {
    for (let item of newPathParts) {
      treeFolders.push(item.toString());
    }
  }
  if (treeFolders.length > 0) {
    treeFolders = treeFolders.concat(
      expandedKeys.filter((x) => !treeFolders.includes(x))
    );
  }
  return treeFolders.length ? treeFolders : expandedKeys;
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
  runInAction(() => {
    const newPath = path.slice();
    while (newPath.length !== 0) {
      const newItems = item.find((x) => x.id === newPath[0]);
      if (!newItems) {
        return;
      }
      newPath.shift();
      if (newPath.length === 0) {
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
        newItems.folders ? newItems.folders : [],
        folders,
        foldersCount,
        currentFolder
      );
    }
  });
};

export const checkProtocol = (fileId, withRedirect) =>
  new Promise((resolve, reject) => {
    const onBlur = () => {
      clearTimeout(timeout);
      window.removeEventListener("blur", onBlur);
      resolve();
    };

    const timeout = setTimeout(() => {
      reject();
      window.removeEventListener("blur", onBlur);
      withRedirect &&
        window.open(
          combineUrl("", homepage, `private?fileId=${fileId}`),
          "_blank"
        );
    }, 1000);

    window.addEventListener("blur", onBlur);

    window.open(
      combineUrl(
        `${EDITOR_PROTOCOL}:${window.location.origin}`,
        homepage,
        `doceditor?fileId=${fileId}`
      ),
      "_self"
    );
  });
