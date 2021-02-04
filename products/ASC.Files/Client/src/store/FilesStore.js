import { makeObservable, action, observable } from "mobx";
import { store, utils } from "asc-web-common";
import TreeFoldersStore from "./TreeFoldersStore";
import config from "../../package.json";

const { authStore } = store;

class FilesStore {
  isLoaded = false;

  treeFoldersStore = null;

  constructor() {
    this.treeFoldersStore = new TreeFoldersStore();

    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeObservable(this, {
      treeFoldersStore: observable,
      isLoaded: observable,
      initFiles: action,
      setIsLoaded: action,
    });
  }

  initFiles = async () => {
    const isAuthenticated = authStore.isAuthenticated;
    const {
      getPortalCultures,
      isDesktopClient,
      getIsEncryptionSupport,
      getEncryptionKeys,
      setModuleInfo,
    } = authStore.settingsStore;

    setModuleInfo(config.homepage, "e67be73d-f9ae-4ce1-8fec-1880cb518cb4");

    const requests = [];

    utils.updateTempContent();
    if (!isAuthenticated) {
      return this.setIsLoaded(true);
    } else {
      utils.updateTempContent(isAuthenticated);
    }

    if (!this.isEditor) {
      requests.push(
        getPortalCultures(),
        this.treeFoldersStore.fetchTreeFolders()
      );

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }
    return Promise.all(requests).finally(() => {
      this.setIsLoaded(true);
      utils.updateTempContent();
    });
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default new FilesStore();
