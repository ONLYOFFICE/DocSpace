import { makeObservable, action, observable } from "mobx";
import { api } from "asc-web-common";
import axios from "axios";

class SettingsTreeStore {
  settingsTree = {};

  constructor() {
    makeObservable(this, {
      settingsTree: observable,

      getFilesSettings: action,
      setExpandSettingsTree: action,
    });
  }

  setFilesSettings = (settings) => {
    const settingsItems = Object.keys(settings);
    for (let key of settingsItems) {
      this.settingsTree[key] = settings[key];
    }

    //forceSave: forcesave,
    //storeForceSave: storeForcesave,
  };

  setIsErrorSettings = (isError) => {
    this.settingsTree.isErrorSettings = isError;
  };

  setExpandSettingsTree = (expandedSetting) => {
    this.settingsTree.expandedSetting = expandedSetting;
  };

  getFilesSettings = () => {
    if (Object.keys(this.settingsTree).length === 0) {
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

                //setThirdPartyCapabilities(capabilities);
                //setThirdPartyProviders(providers);
              });
          }
        })
        .catch(() => this.setIsErrorSettings(true));
    } else {
      return Promise.resolve(this.settingsTree);
    }
  };

  setFilesSetting = (setting, val) => {
    this.settingsTree[setting] = val;
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

export default SettingsTreeStore;
