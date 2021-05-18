import { makeAutoObservable } from "mobx";
import { getFoldersTree } from "@appserver/common/api/files";
import { FolderType } from "@appserver/common/constants";

class TreeFoldersStore {
  selectedFolderStore;

  treeFolders = [];
  selectedTreeNode = [];
  expandedKeys = [];
  expandedPanelKeys = null;

  constructor(selectedFolderStore) {
    makeAutoObservable(this);
    this.selectedFolderStore = selectedFolderStore;
  }

  fetchTreeFolders = async () => {
    const treeFolders = await getFoldersTree();
    this.setTreeFolders(treeFolders);
    return treeFolders;
  };

  getFoldersTree = () => getFoldersTree();

  setTreeFolders = (treeFolders) => {
    this.treeFolders = treeFolders;
  };

  setSelectedNode = (node) => {
    if (node[0]) {
      this.selectedTreeNode = node;
    }
  };

  setExpandedKeys = (expandedKeys) => {
    this.expandedKeys = expandedKeys;
  };

  setExpandedPanelKeys = (expandedPanelKeys) => {
    this.expandedPanelKeys = expandedPanelKeys;
  };

  addExpandedKeys = (item) => {
    this.expandedKeys.push(item);
  };

  updateRootBadge = (id, count) => {
    const rootItem = this.treeFolders.find((x) => x.id === id);
    if (rootItem) rootItem.newItems -= count;
  };

  isCommon = (commonType) => commonType === FolderType.COMMON;
  isShare = (shareType) => shareType === FolderType.SHARE;

  getRootFolder = (rootFolderType) => {
    return this.treeFolders.find((x) => x.rootFolderType === rootFolderType);
  };

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

  get myFolderId() {
    return this.myFolder ? this.myFolder.id : null;
  }

  get commonFolderId() {
    return this.commonFolder ? this.commonFolder.id : null;
  }

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
            folder.rootFolderType === FolderType.Projects ||
            folder.rootFolderType === FolderType.SHARE) &&
          folder
      );
    }
  }
}

export default TreeFoldersStore;
