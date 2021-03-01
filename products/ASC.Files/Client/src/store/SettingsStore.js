import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import axios from "axios";
import ThirdPartyStore from "./ThirdPartyStore";

class SettingsStore {
  thirdPartyStore = null;

  isErrorSettings = null;
  expandedSetting = [];

  confirmDelete = null;
  enableThirdParty = null;
  forcesave = null;
  storeForcesave = null;
  storeOriginalFiles = null;
  updateIfExist = null;

  constructor() {
    makeAutoObservable(this);

    this.thirdPartyStore = new ThirdPartyStore();
  }

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

  setEnableThirdParty = (data, setting) =>
    api.files
      .thirdParty(data)
      .then((res) => this.setFilesSetting(setting, res));

  setForceSave = (data, setting) =>
    api.files.forceSave(data).then((res) => this.setFilesSetting(setting, res));
}

export default new SettingsStore();
