import { makeAutoObservable } from "mobx";
import ModuleStore from "./ModuleStore";
import SettingsStore from "./SettingsStore";
import UserStore from "./UserStore";

class AuthStore {
  userStore = null;
  moduleStore = null;
  settingsStore = null;

  constructor() {
    this.userStore = new UserStore();
    this.moduleStore = new ModuleStore();
    this.settingsStore = new SettingsStore();
    makeAutoObservable(this);
  }

  init = async () => {
    await this.settingsStore.getPortalSettings();
    await this.userStore.getCurrentUser();
    await this.moduleStore.getModules();
  };
}

export default new AuthStore();
