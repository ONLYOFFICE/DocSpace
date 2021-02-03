import { action, computed, makeObservable, observable } from "mobx";
import api from "../api";

class ModuleStore {
  isLoading = false;
  isLoaded = false;
  modules = [];

  constructor() {
    makeObservable(this, {
      isLoading: observable,
      isLoaded: observable,
      modules: observable,
      totalNotificationsCount: computed,
      getModules: action,
      init: action,
      setIsLoading: action,
      setIsLoaded: action,
    });
  }

  getModules = async () => {
    const list = await api.modules.getModulesList();

    this.modules = list.map((item) => {
      return {
        id: item.id,
        title: item.title,
        iconName: item.iconName, // || iconName || "PeopleIcon", //TODO: Change to URL
        iconUrl: item.iconUrl,
        notifications: item.notifications,
        url: item.link,
        link: item.link,
        isPrimary: item.isPrimary,
        description: item.description,
        imageUrl: item.imageUrl,
        onClick: (e) => {
          if (e) {
            window.open(item.link, "_self");
            e.preventDefault();
          }
        },
        onBadgeClick: (e) => console.log("Badge Clicked", e),
      };
    });
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
}

export default ModuleStore;
