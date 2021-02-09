import { makeObservable, action, observable } from "mobx";
import { store, utils } from "asc-web-common";
import FilesStore from "./FilesStore";
import config from "../../package.json";

const { authStore } = store;

class MainFilesStore {
  isLoaded = false;
  isLoading = false;
  dragging = false;

  filesStore = null;

  constructor() {
    this.filesStore = new FilesStore();

    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeObservable(this, {
      filesStore: observable,
      isLoaded: observable,
      isLoading: observable,
      dragging: observable,

      initFiles: action,
      setIsLoaded: action,
      setIsLoading: action,
      setDragging: action,
    });
  }

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setDragging = (dragging) => {
    this.dragging = dragging;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  initFiles = () => {
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
        this.filesStore.treeFoldersStore.fetchTreeFolders()
      );

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }

    return Promise.all(requests);
  };
}

export default new MainFilesStore();
