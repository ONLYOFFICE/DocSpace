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
  providerItem = null;

  constructor() {
    makeAutoObservable(this);
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
    this.providerItem = null;
  };

  setSelectedFolder = (selectedFolder) => {
    if (!selectedFolder) {
      this.toDefault();
    } else {
      const selectedFolderItems = Object.keys(selectedFolder);

      for (let key of selectedFolderItems) {
        if (key in this) {
          this[key] = selectedFolder[key];
        }
      }
    }
  };
}

export default new SelectedFolderStore();
