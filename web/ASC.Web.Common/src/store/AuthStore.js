import { makeAutoObservable } from "mobx";
import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
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
    const isAuthenticated = await api.user.checkIsAuthenticated();

    await this.settingsStore.getPortalSettings();

    if (!isAuthenticated) return;

    await this.userStore.getCurrentUser();
    await this.moduleStore.getModules();
  };

  login = async (user, hash) => {
    try {
      const response = await api.user.login(user, hash);

      console.log("Login response", response);

      debugger;

      setWithCredentialsStatus(true);

      await this.init();

      return true;
    } catch (e) {
      return false;
    }
  };
}

export default new AuthStore();
