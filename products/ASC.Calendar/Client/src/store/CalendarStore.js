import { action, makeObservable, observable } from "mobx";
import config from "../../package.json";
import store from "studio/store";
const { auth: authStore } = store;

class CalendarStore {
  isLoading = false;
  isLoaded = false;
  isInit = false;

  constructor() {
    makeObservable(this, {
      isLoading: observable,
      isLoaded: observable,
      setIsLoading: action,
      setIsLoaded: action,
      init: action,
    });
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    authStore.settingsStore.setModuleInfo(config.homepage, config.id);

    this.setIsLoaded(true);
  };

  setIsLoading = (loading) => {
    this.isLoading = loading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default CalendarStore;
