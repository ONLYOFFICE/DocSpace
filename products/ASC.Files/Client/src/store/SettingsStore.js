import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import axios from "axios";

class SettingsStore {
  thirdPartyStore;

  isErrorSettings = null;
  expandedSetting = null;

  confirmDelete = null;
  enableThirdParty = null;
  forcesave = null;
  storeForcesave = null;
  storeOriginalFiles = null;
  updateIfExist = null;

  settingsIsLoaded = false;

  constructor(thirdPartyStore) {
    makeAutoObservable(this);

    this.thirdPartyStore = thirdPartyStore;
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
}

export default SettingsStore;
