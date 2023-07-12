import { makeAutoObservable } from "mobx";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";

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

  updateEditedSelectedRoom = (title = this.title, tags = this.tags) => {
    this.title = title;
    this.tags = tags;
  };

  addDefaultLogoPaths = () => {
    const cachebreaker = new Date().getTime();
    this.logo = {
      small: `/storage/room_logos/root/${this.id}_small.png?` + cachebreaker,
      medium: `/storage/room_logos/root/${this.id}_medium.png?` + cachebreaker,
      large: `/storage/room_logos/root/${this.id}_large.png?` + cachebreaker,
      original:
        `/storage/room_logos/root/${this.id}_original.png?` + cachebreaker,
    };
  };

  removeLogoPaths = () => {
    this.logo = {
      small: "",
      medium: "",
      large: "",
      original: "",
    };
  };

  updateLogoPathsCacheBreaker = () => {
    if (!this.logo.original) return;

    const cachebreaker = new Date().getTime();
    this.logo = {
      small: this.logo.small.split("?")[0] + "?" + cachebreaker,
      medium: this.logo.medium.split("?")[0] + "?" + cachebreaker,
      large: this.logo.large.split("?")[0] + "?" + cachebreaker,
      original: this.logo.original.split("?")[0] + "?" + cachebreaker,
    };
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

      setDocumentTitle(selectedFolder.title);

      for (let key of selectedFolderItems) {
        if (key in this) {
          this[key] = selectedFolder[key];
        }
      }
    }
  };
}

export default SelectedFolderStore;
