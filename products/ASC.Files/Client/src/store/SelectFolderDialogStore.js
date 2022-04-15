import { makeObservable, action, observable } from "mobx";

class SelectedFilesStore {
  folderId = null;
  fileInfo = null;
  folderTitle = "";
  providerKey = null;
  baseFolderPath = "";

  constructor() {
    makeObservable(this, {
      fileInfo: observable,
      folderId: observable,

      setFolderId: action,
      setFile: action,
    });
  }

  setFolderId = (id) => {
    this.folderId = id;
  };
  setFile = (obj) => {
    this.fileInfo = obj;
  };

  setFolderTitle = (title) => {
    this.folderTitle = title;
  };

  setProviderKey = (providerKey) => {
    this.providerKey = providerKey;
  };

  setBaseFolderPath = (baseFolderPath) => {
    this.baseFolderPath = baseFolderPath;
  };
}

export default new SelectedFilesStore();
