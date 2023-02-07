import { makeAutoObservable } from "mobx";

class SelectFileDialogStore {
  folderId = null;
  fileInfo = {};

  constructor() {
    makeAutoObservable(this);
  }

  setFolderId = (id) => {
    this.folderId = id;
  };
  setFile = (obj) => {
    this.fileInfo = obj;
  };
}

export default new SelectFileDialogStore();
