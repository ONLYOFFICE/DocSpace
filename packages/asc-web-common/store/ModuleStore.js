import api from "../api";
import { makeAutoObservable } from "mobx";
import { combineUrl } from "../utils";
import { AppServerConfig } from "../constants";
const { proxyURL } = AppServerConfig;

class ModuleStore {
  isLoading = false;
  isLoaded = false;
  modules = [];

  constructor() {
    makeAutoObservable(this);
  }

  getModules = async () => {
    const list = await api.modules.getModulesList();

    const extendedModules = list.map((m) => this.toModuleWrapper(m));

    this.setModules(extendedModules);
  };

  toModuleWrapper = (item) => {
    const id =
      item.id && typeof item.id === "string" ? item.id.toLowerCase() : null;

    const link = item.link
      ? combineUrl(proxyURL, item.link.toLowerCase())
      : null;
    //const iconUrl = combineUrl(proxyURL, item.iconUrl.toLowerCase());
    //const imageUrl = combineUrl(proxyURL, item.imageUrl.toLowerCase());

    const result = {
      ...item,
      id,
      link,
      origLink: item.link && item.link.toLowerCase(),
      notifications: 0,
      iconUrl: item.iconUrl && item.iconUrl.toLowerCase(),
      imageUrl: item.imageUrl && item.imageUrl.toLowerCase(),
    };

    return result;
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
