import { makeAutoObservable } from "mobx";

class SelectFolderDialogStore {
  resultingFolderId = null;
  roomType = null;
  fileInfo = null;
  folderTitle = "";
  providerKey = null;
  baseFolderPath = "";
  newFolderPath = "";
  isPathError = false;
  isLoading = false;
  resultingFolderTree = [];
  securityItem = {};

  constructor() {
    makeAutoObservable(this);
  }

  toDefault = () => {
    this.resultingFolderId = null;
    this.roomType = null;
    this.resultingFolderTree = [];
    this.baseFolderPath = "";
    this.newFolderPath = "";
    this.folderTitle = "";
    this.isLoading = false;
    this.isPathError = false;
    this.setProviderKey(null);
  };

  setItemSecurity = (security) => {
    this.securityItem = security;
  };

  updateBaseFolderPath = () => {
    this.baseFolderPath = this.newFolderPath;
    this.setIsPathError(false);
  };

  resetNewFolderPath = (id) => {
    this.newFolderPath = this.baseFolderPath;
    this.setIsPathError(false);
    this.setResultingFolderId(id);
  };

  setBaseFolderPath = (baseFolderPath) => {
    this.baseFolderPath = baseFolderPath;
  };

  setIsPathError = (isPathError) => {
    this.isPathError = isPathError;
  };

  setNewFolderPath = (newFolderPath) => {
    this.newFolderPath = newFolderPath;
  };
  setResultingFolderId = (id) => {
    this.resultingFolderId = id;
  };

  setRoomType = (type) => {
    this.roomType = type;
  };

  setFolderTitle = (title) => {
    this.folderTitle = title;
  };

  setProviderKey = (providerKey) => {
    this.providerKey = providerKey;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setResultingFoldersTree = (tree) => {
    this.resultingFolderTree = tree;
  };
}

export default new SelectFolderDialogStore();
