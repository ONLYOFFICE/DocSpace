import { makeAutoObservable } from "mobx";
import { api } from "asc-web-common";

class TreeFoldersStore {
  treeFolders = [];

  constructor() {
    makeAutoObservable(this);
  }

  fetchTreeFolders = async () => {
    this.treeFolders = await api.files.getFoldersTree();
  };
}
export default TreeFoldersStore;
