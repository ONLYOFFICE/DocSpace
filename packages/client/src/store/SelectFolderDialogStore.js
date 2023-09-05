import { makeAutoObservable } from "mobx";

class SelectFolderDialogStore {
  roomType = null;

  newPath = "";
  basePath = "";

  constructor() {
    makeAutoObservable(this);
  }

  toDefault = () => {
    this.basePath = "";
    this.newPath = "";
  };

  convertPath = (foldersArray) => {
    let path = "";

    if (foldersArray.length > 1) {
      for (let item of foldersArray) {
        if (!path) {
          path = path + `${item.label}`;
        } else path = path + " " + "/" + " " + `${item.label}`;
      }
    } else {
      for (let item of foldersArray) {
        path = `${item.label}`;
      }
    }

    return path;
  };

  resetNewFolderPath = () => {
    this.newPath = this.basePath;
  };

  setNewPath = (folders) => {
    this.newPath = this.convertPath(folders);
  };

  setBasePath = (folders) => {
    this.basePath = this.convertPath(folders);
  };

  setResultingFolderId = (id) => {
    this.resultingFolderId = id;
  };

  setRoomType = (type) => {
    this.roomType = type;
  };
}

export default new SelectFolderDialogStore();
