import { makeObservable, action, observable } from "mobx";

class SelectedFilesStore {
  folderId = null;
  fileInfo = null;

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
}

export default new SelectedFilesStore();
