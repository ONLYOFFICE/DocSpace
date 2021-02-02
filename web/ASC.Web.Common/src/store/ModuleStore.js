import { makeAutoObservable } from "mobx";
import api from "../api";

class ModuleStore {
  isLoading = false;
  modules = [];

  constructor() {
    makeAutoObservable(this);
  }

  getModules = async () => {
    this.isLoading = true;
    const list = await api.modules.getModulesList();

    this.modules = list.map((item) => {
      return {
        id: item.id,
        title: item.title,
        iconName: item.iconName, // || iconName || "PeopleIcon", //TODO: Change to URL
        iconUrl: item.iconUrl,
        notifications: 0,
        url: item.link,
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

    this.isLoading = false;
  };
}

export default ModuleStore;
