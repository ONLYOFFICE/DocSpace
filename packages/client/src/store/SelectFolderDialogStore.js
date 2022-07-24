import { makeObservable, action, observable } from "mobx";

class SelectFolderDialogStore {
  folderId = null;
  fileInfo = null;
  folderTitle = "";
  providerKey = null;
  baseFolderPath = "";

  constructor() {
    makeObservable(this, {
      fileInfo: observable,
      folderId: observable,
      folderTitle: observable,
      providerKey: observable,

      setFolderId: action,
      setProviderKey: action,
      setFolderTitle: action,
    });
  }

  setFolderId = (id) => {
    this.folderId = id;
  };

  setFolderTitle = (title) => {
    this.folderTitle = title;
  };

  setProviderKey = (providerKey) => {
    this.providerKey = providerKey;
  };
}

export default new SelectFolderDialogStore();
