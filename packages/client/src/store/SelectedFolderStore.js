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
  roomType = null;
  pinned = null;
  isRoom = null;
  logo = null;

  settingsStore = null;

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
    this.roomType = null;
    this.pinned = null;
    this.isRoom = null;
    this.logo = null;
  };

  setParentId = (parentId) => {
    this.parentId = parentId;
  };

  setSelectedFolder = (selectedFolder) => {
    const { socketHelper } = this.settingsStore;

    if (this.id !== null) {
      socketHelper.emit({ command: "unsubscribe", data: `DIR-${this.id}` });
    }

    if (selectedFolder) {
      socketHelper.emit({
        command: "subscribe",
        data: `DIR-${selectedFolder.id}`,
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
