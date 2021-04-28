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

  setSelectedFolder = (selectedFolder) => {
    if (!selectedFolder) {
      const newStore = new SelectedFolderStore();

      const selectedFolderItems = Object.keys(newStore);
      for (let key of selectedFolderItems) {
        if (key in this) {
          this[key] = newStore[key];
        }
      }
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
