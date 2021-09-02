import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import axios from "axios";
import {
  setFavoritesSetting,
  setRecentSetting,
} from "@appserver/common/api/files";
import { FolderType } from "@appserver/common/constants";

class SettingsStore {
  thirdPartyStore;
  treeFoldersStore;

  isErrorSettings = null;
  expandedSetting = null;

  confirmDelete = null;
  enableThirdParty = null;
  forcesave = null;
  storeForcesave = null;
  storeOriginalFiles = null;
  updateIfExist = null;
  favoritesSection = null;
  recentSection = null;
  hideConfirmConvertSave = null;
  chunkUploadSize = 1024 * 1023; // 1024 * 1023; //~0.999mb

  settingsIsLoaded = false;

  constructor(thirdPartyStore, treeFoldersStore) {
    makeAutoObservable(this);

    this.thirdPartyStore = thirdPartyStore;
    this.treeFoldersStore = treeFoldersStore;
  }

  setIsLoaded = (isLoaded) => {
    this.settingsIsLoaded = isLoaded;
  };

  get isLoadedSettingsTree() {
    return (
      this.confirmDelete !== null &&
      this.enableThirdParty !== null &&
      this.forcesave !== null &&
      this.storeForcesave !== null &&
      this.storeOriginalFiles !== null &&
      this.updateIfExist !== null
    );
  }

  setFilesSettings = (settings) => {
    const settingsItems = Object.keys(settings);
    for (let key of settingsItems) {
      this[key] = settings[key];
    }
  };

  setIsErrorSettings = (isError) => {
    this.isErrorSettings = isError;
  };

  setExpandSettingsTree = (expandedSetting) => {
    this.expandedSetting = expandedSetting;
  };

  getFilesSettings = () => {
    if (!this.isLoadedSettingsTree) {
      return api.files
        .getSettingsFiles()
        .then((settings) => {
          this.setFilesSettings(settings);
          if (settings.enableThirdParty) {
            this.setIsLoaded(true);
            return axios
              .all([
                api.files.getThirdPartyCapabilities(),
                api.files.getThirdPartyList(),
              ])
              .then(([capabilities, providers]) => {
                for (let item of capabilities) {
                  item.splice(1, 1);
                }
                this.thirdPartyStore.setThirdPartyCapabilities(capabilities); //TODO: Out of bounds read: 1
                this.thirdPartyStore.setThirdPartyProviders(providers);
              });
          }
          return this.setIsLoaded(true);
        })
        .catch(() => this.setIsErrorSettings(true));
    } else {
      return Promise.resolve();
    }
  };

  setFilesSetting = (setting, val) => {
    this[setting] = val;
  };

  setUpdateIfExist = (data, setting) =>
    api.files
      .updateIfExist(data)
      .then((res) => this.setFilesSetting(setting, res));

  setStoreOriginal = (data, setting) =>
    api.files
      .storeOriginal(data)
      .then((res) => this.setFilesSetting(setting, res));

  setConfirmDelete = (data, setting) =>
    api.files
      .changeDeleteConfirm(data)
      .then((res) => this.setFilesSetting(setting, res));

  setStoreForceSave = (data, setting) =>
    api.files
      .storeForceSave(data)
      .then((res) => this.setFilesSetting(setting, res));

  setEnableThirdParty = async (data, setting) => {
    const res = await api.files.thirdParty(data);
    this.setFilesSetting(setting, res);

    if (data) {
      return axios
        .all([
          api.files.getThirdPartyCapabilities(),
          api.files.getThirdPartyList(),
        ])
        .then(([capabilities, providers]) => {
          for (let item of capabilities) {
            item.splice(1, 1);
          }
          this.thirdPartyStore.setThirdPartyCapabilities(capabilities); //TODO: Out of bounds read: 1
          this.thirdPartyStore.setThirdPartyProviders(providers);
        });
    } else {
      return Promise.resolve();
    }
  };

  setForceSave = (data, setting) =>
    api.files.forceSave(data).then((res) => this.setFilesSetting(setting, res));

  updateRootTreeFolders = (set, rootFolderIndex, folderType) => {
    const {
      getFoldersTree,
      treeFolders,
      setTreeFolders,
    } = this.treeFoldersStore;

    getFoldersTree().then((root) => {
      if (set) {
        const rootFolder = root.find((x) => x.rootFolderType === folderType);
        const newTreeFolders = treeFolders;
        newTreeFolders.splice(rootFolderIndex, 0, rootFolder);
        setTreeFolders(newTreeFolders);
      } else {
        const newTreeFolders = treeFolders.filter(
          (x) => x.rootFolderType !== folderType
        );
        setTreeFolders(newTreeFolders);
      }
    });
  };

  setFavoritesSetting = (set, setting) => {
    return setFavoritesSetting(set).then((res) => {
      this.setFilesSetting(setting, res);
      this.updateRootTreeFolders(set, 2, FolderType.Favorites);
    });
  };

  setRecentSetting = (set, setting) => {
    return setRecentSetting(set).then((res) => {
      this.setFilesSetting(setting, res);
      const index = this.treeFoldersStore.favoritesFolder ? 3 : 2;
      this.updateRootTreeFolders(set, index, FolderType.Recent);
    });
  };

  hideConfirmConvert = async (save = true) => {
    const hideConfirmConvertSave = await api.files.hideConfirmConvert(save);
    this.hideConfirmConvertSave = hideConfirmConvertSave;
  };
}

export default SettingsStore;
