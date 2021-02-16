import { makeObservable, observable, computed, action } from "mobx";
import { api, constants } from "asc-web-common";
import SelectedFolderStore from "./SelectedFolderStore";

const { FolderType } = constants;
class TreeFoldersStore {
  selectedFolderStore = null;

  treeFolders = [];
  selectedTreeNode = [];

  constructor() {
    makeObservable(this, {
      selectedFolderStore: observable,

      treeFolders: observable,
      selectedTreeNode: observable,

      myFolderId: computed,
      //shareFolderId: computed,
      //favoritesFolderId: computed,
      //recentFolderId: computed,
      commonFolderId: computed,

      myFolder: computed,
      shareFolder: computed,
      favoritesFolder: computed,
      recentFolder: computed,
      privacyFolder: computed,
      commonFolder: computed,
      recycleBinFolder: computed,

      isMyFolder: computed,
      isShareFolder: computed,
      isFavoritesFolder: computed,
      isRecentFolder: computed,
      isPrivacyFolder: computed,
      isCommonFolder: computed,
      isRecycleBinFolder: computed,

      fetchTreeFolders: action,
      setTreeFolders: action,
    });

    this.selectedFolderStore = new SelectedFolderStore();
  }

  fetchTreeFolders = async () => {
    const treeFolders = await api.files.getFoldersTree();
    this.setTreeFolders(treeFolders);
  };

  setTreeFolders = (treeFolders) => {
    this.treeFolders = treeFolders;
  };

  setSelectedNode = (node) => {
    if (node[0]) {
      this.selectedTreeNode = node;
    }
  };

  renameTreeFolder = (folders, newItems, currentFolder) => {
    const newItem = folders.find((x) => x.id === currentFolder.id);
    const oldItemIndex = newItems.folders.findIndex(
      (x) => x.id === currentFolder.id
    );
    newItem.folders = newItems.folders[oldItemIndex].folders;
    newItems.folders[oldItemIndex] = newItem;

    return;
  };

  removeTreeFolder = (folders, newItems, foldersCount) => {
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

  addTreeFolder = (folders, newItems, foldersCount) => {
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

  loopTreeFolders = (path, item, folders, foldersCount, currentFolder) => {
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
          this.addTreeFolder(folders, newItems, foldersCount);
        } else if (folders.length < foldersLength) {
          this.removeTreeFolder(folders, newItems, foldersCount);
        } else if (
          folders.length > 0 &&
          newItems.folders.length > 0 &&
          currentFolder
        ) {
          this.renameTreeFolder(folders, newItems, currentFolder);
        } else {
          return;
        }
        return;
      }
      this.loopTreeFolders(
        newPath,
        newItems.folders,
        folders,
        foldersCount,
        currentFolder
      );
    }
  };

  /////////////////////////////////////TODO: FOLDER

  get myFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@my");
  }

  get shareFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@share");
  }

  get favoritesFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@favorites");
  }

  get recentFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@recent");
  }

  get privacyFolder() {
    return this.treeFolders.find(
      (x) => x.rootFolderType === FolderType.Privacy
    );
  }

  get commonFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@common");
  }

  get recycleBinFolder() {
    return this.treeFolders.find((x) => x.rootFolderName === "@trash");
  }

  /////////////////////////////////////TODO: ID

  get myFolderId() {
    return this.myFolder ? this.myFolder.id : null;
  }

  // get shareFolderId() {
  //   return this.shareFolder ?this.shareFolder.id : null;
  // }

  // get favoritesFolderId() {
  //   return this.favoritesFolder ? this.favoritesFolder.id : null;
  // }

  // get recentFolderId() {

  //   return this.recentFolder ? this.recentFolder.id : null;
  // }

  get commonFolderId() {
    return this.commonFolder ? this.commonFolder.id : null;
  }

  /////////////////////////////////////TODO: IS

  get isMyFolder() {
    return this.myFolder && this.myFolder.id === this.selectedFolderStore.id;
  }

  get isShareFolder() {
    return (
      this.shareFolder && this.shareFolder.id === this.selectedFolderStore.id
    );
  }

  get isFavoritesFolder() {
    return (
      this.favoritesFolder &&
      this.selectedFolderStore.id === this.favoritesFolder.id
    );
  }

  get isRecentFolder() {
    return (
      this.recentFolder && this.selectedFolderStore.id === this.recentFolder.id
    );
  }

  get isPrivacyFolder() {
    return (
      this.privacyFolder &&
      this.privacyFolder.rootFolderType ===
        this.selectedFolderStore.rootFolderType
    );
  }

  get isCommonFolder() {
    return (
      this.commonFolder && this.commonFolder.id === this.selectedFolderStore.id
    );
  }

  get isRecycleBinFolder() {
    return (
      this.recycleBinFolder &&
      this.selectedFolderStore.id === this.recycleBinFolder.id
    );
  }

  get operationsFolders() {
    if (this.isPrivacyFolder) {
      return this.treeFolders.filter(
        (folder) => folder.rootFolderType === FolderType.Privacy && folder
      );
    } else {
      return this.treeFolders.filter(
        (folder) =>
          (folder.rootFolderType === FolderType.USER ||
            folder.rootFolderType === FolderType.COMMON ||
            folder.rootFolderType === FolderType.Projects) &&
          folder
      );
    }
  }
}

export default new TreeFoldersStore();
