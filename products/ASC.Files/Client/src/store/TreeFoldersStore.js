import { makeAutoObservable } from "mobx";
import { getFoldersTree, getSubfolders } from "@appserver/common/api/files";
import { FolderType } from "@appserver/common/constants";
import { createTreeFolders } from "../helpers/files-helpers";

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
    !this.expandedKeys.includes(item) && this.expandedKeys.push(item);
  };

  createNewExpandedKeys = (pathParts) => {
    return createTreeFolders(pathParts, this.expandedKeys);
  };

  updateRootBadge = (id, count) => {
    const rootItem = this.treeFolders.find((x) => x.id === id);
    if (rootItem) rootItem.newItems -= count;
  };

  isMy = (myType) => myType === FolderType.USER;
  isCommon = (commonType) => commonType === FolderType.COMMON;
  isShare = (shareType) => shareType === FolderType.SHARE;

  getRootFolder = (rootFolderType) => {
    return this.treeFolders.find((x) => x.rootFolderType === rootFolderType);
  };

  getSubfolders = (folderId) => getSubfolders(folderId);

  get myFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.USER);
  }

  get shareFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.SHARE);
  }

  get favoritesFolder() {
    return this.treeFolders.find(
      (x) => x.rootFolderType === FolderType.Favorites
    );
  }

  get recentFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.Recent);
  }

  get privacyFolder() {
    return this.treeFolders.find(
      (x) => x.rootFolderType === FolderType.Privacy
    );
  }

  get commonFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.COMMON);
  }

  get recycleBinFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.TRASH);
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

  get selectedKeys() {
    const selectedKeys =
      this.selectedTreeNode.length > 0 &&
      this.selectedTreeNode[0] !== "@my" &&
      this.selectedTreeNode[0] !== "@common"
        ? this.selectedTreeNode
        : [this.selectedFolderStore.id + ""];
    return selectedKeys;
  }
}

export default TreeFoldersStore;
