import { makeAutoObservable } from "mobx";
import { getFoldersTree, getSubfolders } from "@docspace/common/api/files";
import { FolderType } from "@docspace/common/constants";

class TreeFoldersStore {
  selectedFolderStore;
  authStore;

  treeFolders = [];
  selectedTreeNode = [];
  expandedPanelKeys = null;
  rootFoldersTitles = {};
  isLoadingNodes = false;

  constructor(selectedFolderStore, authStore) {
    makeAutoObservable(this);

    this.selectedFolderStore = selectedFolderStore;
    this.authStore = authStore;
  }

  fetchTreeFolders = async () => {
    const treeFolders = await getFoldersTree();
    this.setRootFoldersTitles(treeFolders);
    this.setTreeFolders(treeFolders);
    this.listenTreeFolders(treeFolders);
    return treeFolders;
  };

  listenTreeFolders = (treeFolders) => {
    const { socketHelper } = this.authStore.settingsStore;

    if (treeFolders.length > 0) {
      socketHelper.emit({
        command: "unsubscribe",
        data: {
          roomParts: treeFolders.map((f) => `DIR-${f.id}`),
          individual: true,
        },
      });

      socketHelper.emit({
        command: "subscribe",
        data: {
          roomParts: treeFolders.map((f) => `DIR-${f.id}`),
          individual: true,
        },
      });
    }
  };

  updateTreeFoldersItem = (opt) => {
    if (opt?.data && opt?.cmd === "create") {
      const data = JSON.parse(opt.data);

      const parentId = opt?.type === "file" ? data.folderId : data.parentId;

      const idx = this.treeFolders.findIndex((f) => f.id === parentId);

      if (idx >= 0) {
        if (opt.type === "file") {
          this.treeFolders[idx].filesCount++;
          if (this.treeFolders[idx].files) {
            this.treeFolders[idx].files.push(data);
          } else {
            this.treeFolders[idx].files = [data];
          }
        } else {
          this.treeFolders[idx].foldersCount++;
          if (this.treeFolders[idx].folders) {
            this.treeFolders[idx].folders.push(data);
          } else {
            this.treeFolders[idx].folders = [data];
          }
        }
      }
    }
  };

  resetTreeItemCount = () => {
    this.treeFolders.map((item) => {
      return (item.newItems = 0);
    });
  };

  setRootFoldersTitles = (treeFolders) => {
    treeFolders.forEach((elem) => {
      this.rootFoldersTitles[elem.rootFolderType] = {
        id: elem.id,
        title: elem.title,
      };
    });
  };

  getFoldersTree = () => getFoldersTree();

  setTreeFolders = (treeFolders) => {
    this.treeFolders = treeFolders;
  };

  setIsLoadingNodes = (isLoadingNodes) => {
    this.isLoadingNodes = isLoadingNodes;
  };
  setSelectedNode = (node) => {
    if (node[0]) {
      this.selectedTreeNode = node;
    }
  };

  setExpandedPanelKeys = (expandedPanelKeys) => {
    this.expandedPanelKeys = expandedPanelKeys;
  };

  // updateRootBadge = (id, count) => {
  //   const index = this.treeFolders.findIndex((x) => x.id === id);
  //   if (index < 0) return;

  //   this.treeFolders = this.treeFolders.map((f, i) => {
  //     if (i !== index) return f;
  //     f.newItems -= count;
  //     return f;
  //   });
  // };

  isMy = (myType) => myType === FolderType.USER;
  isCommon = (commonType) => commonType === FolderType.COMMON;
  isShare = (shareType) => shareType === FolderType.SHARE;
  isRoomRoot = (type) => type === FolderType.Rooms;

  getRootFolder = (rootFolderType) => {
    return this.treeFolders.find((x) => x.rootFolderType === rootFolderType);
  };

  getSubfolders = (folderId) => getSubfolders(folderId);

  get myRoomsId() {
    return this.rootFoldersTitles[FolderType.Rooms]?.id;
  }

  get archiveRoomsId() {
    return this.rootFoldersTitles[FolderType.Archive]?.id;
  }

  get recycleBinFolderId() {
    return this.rootFoldersTitles[FolderType.TRASH]?.id;
  }

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

  get roomsFolder() {
    return this.treeFolders.find((x) => x.rootFolderType === FolderType.Rooms);
  }

  get archiveFolder() {
    return this.treeFolders.find(
      (x) => x.rootFolderType === FolderType.Archive
    );
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

  get roomsFolderId() {
    return this.roomsFolder ? this.roomsFolder.id : null;
  }

  get archiveFolderId() {
    return this.archiveFolder ? this.archiveFolder.id : null;
  }

  get recycleBinFolderId() {
    return this.recycleBinFolder ? this.recycleBinFolder.id : null;
  }

  get isPersonalRoom() {
    return (
      this.myFolder &&
      this.myFolder.rootFolderType === this.selectedFolderStore.rootFolderType
    );
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

  get isTrashFolder() {
    return (
      this.recycleBinFolder &&
      this.selectedFolderStore.id === this.recycleBinFolder.id
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

  get isRoomsFolder() {
    return (
      this.roomsFolder && this.selectedFolderStore.id === this.roomsFolder.id
    );
  }

  get isRoom() {
    return (
      this.roomsFolder &&
      this.roomsFolder.rootFolderType ===
        this.selectedFolderStore.rootFolderType
    );
  }

  get isArchiveFolder() {
    return (
      this.archiveFolder &&
      this.selectedFolderStore.id === this.archiveFolder.id
    );
  }

  get isArchiveFolderRoot() {
    return FolderType.Archive === this.selectedFolderStore.rootFolderType;
  }

  get isPersonalFolderRoot() {
    return FolderType.USER === this.selectedFolderStore.rootFolderType;
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
