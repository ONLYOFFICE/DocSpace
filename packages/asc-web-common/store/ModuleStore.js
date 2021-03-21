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
    const iconUrl = item.iconUrl
      ? combineUrl(proxyURL, item.iconUrl.toLowerCase())
      : null;
    const imageUrl = item.imageUrl
      ? combineUrl(proxyURL, item.imageUrl.toLowerCase())
      : null;

    const result = {
      ...item,
      id,
      link,
      notifications: 0,
      iconUrl,
      imageUrl,
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
