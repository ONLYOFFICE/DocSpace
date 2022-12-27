import { makeAutoObservable } from "mobx";

class SelectedFolderStore {
  folders = null;
  parentId = null;
  filesCount = null;
  foldersCount = null;
  isShareable = null;
  new = null;
  id = null;
  title = null;
  access = null;
  shared = null;
  created = null;
  createdBy = null;
  updated = null;
  updatedBy = null;
  rootFolderType = null;
  pathParts = null;
  navigationPath = null;
  providerItem = null;
  providerKey = null;
  providerId = null;
  roomType = null;
  pinned = null;
  isRoom = null;
  isArchive = null;
  logo = null;
  tags = null;
  rootFolderId = null;
  settingsStore = null;
  security = null;

  constructor(settingsStore) {
    makeAutoObservable(this);
    this.settingsStore = settingsStore;
  }

  get isRootFolder() {
    return this.pathParts && this.pathParts.length <= 1;
  }

  toDefault = () => {
    this.folders = null;
    this.parentId = null;
    this.filesCount = null;
    this.foldersCount = null;
    this.isShareable = null;
    this.new = null;
    this.id = null;
    this.title = null;
    this.access = null;
    this.shared = null;
    this.created = null;
    this.createdBy = null;
    this.updated = null;
    this.updatedBy = null;
    this.rootFolderType = null;
    this.pathParts = null;
    this.navigationPath = null;
    this.providerItem = null;
    this.providerKey = null;
    this.providerId = null;
    this.roomType = null;
    this.pinned = null;
    this.isRoom = null;
    this.logo = null;
    this.tags = null;
    this.rootFolderId = null;
    this.security = null;
  };

  setParentId = (parentId) => {
    this.parentId = parentId;
  };

  setSelectedFolder = (selectedFolder) => {
    const { socketHelper } = this.settingsStore;

    if (this.id !== null) {
      socketHelper.emit({
        command: "unsubscribe",
        data: { roomParts: `DIR-${this.id}`, individual: true },
      });
    }

    if (selectedFolder) {
      socketHelper.emit({
        command: "subscribe",
        data: { roomParts: `DIR-${selectedFolder.id}`, individual: true },
      });
    }

    if (!selectedFolder) {
      this.toDefault();
    } else {
      const selectedFolderItems = Object.keys(selectedFolder);

      if (!selectedFolderItems.includes("roomType")) this.roomType = null;

      for (let key of selectedFolderItems) {
        if (key in this) {
          this[key] = selectedFolder[key];
        }
      }
    }
  };
}

export default SelectedFolderStore;
