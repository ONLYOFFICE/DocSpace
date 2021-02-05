import api from "../api";
import { makeAutoObservable } from "mobx";

class ModuleStore {
  isLoading = false;
  isLoaded = false;
  modules = [];

  constructor() {
    makeAutoObservable(this);
  }

  getModules = async () => {
    const list = await api.modules.getModulesList();
    this.setModules(list);
  };

  get totalNotificationsCount() {
    let totalNotifications = 0;
    this.modules
      .filter((item) => !item.separator)
      .forEach((item) => (totalNotifications += item.notifications || 0));

    return totalNotifications;
  }

  init = async () => {
    this.setIsLoading(true);

    await this.getModules();

    this.setIsLoading(false);
    this.setIsLoaded(true);
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setModules = (modules) => {
    this.modules = modules;
  };
}

export default ModuleStore;
