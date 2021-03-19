import { action, computed, makeObservable, observable } from "mobx";
import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
import history from "../history";
import ModuleStore from "./ModuleStore";
import SettingsStore from "./SettingsStore";
import UserStore from "./UserStore";
import { logout as logoutDesktop, desktopConstants } from "../desktop";
import { isAdmin } from "../utils";
import isEmpty from "lodash/isEmpty";

class AuthStore {
  userStore = null;
  moduleStore = null;
  settingsStore = null;

  isLoading = false;
  isAuthenticated = false;
  version = null;

  constructor() {
    this.setUserStore(new UserStore());
    this.setModuleStore(new ModuleStore());
    this.setSettingsStore(new SettingsStore());

    makeObservable(this, {
      isLoading: observable,
      isAuthenticated: observable,
      isAdmin: computed,
      isLoaded: computed,
      language: computed,
      product: computed,
      availableModules: computed,
      userStore: observable,
      moduleStore: observable,
      settingsStore: observable,
      version: observable,
      init: action,
      login: action,
      logout: action,
      setIsAuthenticated: action,
      setUserStore: action,
      setModuleStore: action,
      setSettingsStore: action,
      replaceFileStream: action,
      getEncryptionAccess: action,
      setEncryptionAccess: action,
      setProductVersion: action,
    });
  }

  init = async () => {
    await this.getIsAuthenticated();

    const requests = [];
    requests.push(this.settingsStore.init());

    if (this.isAuthenticated) {
      requests.push(this.userStore.init());
      requests.push(this.moduleStore.init());
    }

    return Promise.all(requests);
  };

  get isLoaded() {
    let success = false;
    if (this.isAuthenticated) {
      success =
        this.userStore.isLoaded &&
        this.moduleStore.isLoaded &&
        this.settingsStore.isLoaded;
    } else {
      success = this.settingsStore.isLoaded;
    }

    return success;
  }

  get language() {
    return (
      (this.userStore.user && this.userStore.user.cultureName) ||
      this.settingsStore.culture ||
      "en-US"
    );
  }

  get isAdmin() {
    const { user } = this.userStore;
    const { currentProductId } = this.settingsStore;

    if (!user || !user.id) return false;

    return isAdmin(user, currentProductId);
  }

  get product() {
    return this.moduleStore.modules.find(
      (item) => item.id === this.settingsStore.currentProductId
    );
  }

  get availableModules() {
    const { user } = this.userStore;
    const { modules, toModuleWrapper } = this.moduleStore;
    if (isEmpty(modules) || isEmpty(this.userStore.user)) {
      return [];
    }

    const isUserAdmin = user.isAdmin;
    const customModules = this.getCustomModules(isUserAdmin);
    const products = modules.map((m) => toModuleWrapper(m, false));
    const primaryProducts = products.filter((m) => m.isPrimary === true);
    const dummyProducts = products.filter((m) => m.isPrimary === false);

    return [
      {
        separator: true,
        id: "nav-products-separator",
      },
      ...primaryProducts,
      ...customModules,
      {
        separator: true,
        dashed: true,
        id: "nav-dummy-products-separator",
      },
      ...dummyProducts,
    ];
  }

  getCustomModules = (isAdmin) => {
    if (!isAdmin) {
      return [];
    }
    const settingsModuleWrapper = this.moduleStore.toModuleWrapper(
      {
        id: "settings",
        title: "Settings",
        link: "/settings",
      },
      false,
      "SettingsIcon"
    );

    return [settingsModuleWrapper];
  };

  getIsAuthenticated = async () => {
    const isAuthenticated = await api.user.checkIsAuthenticated();
    this.setIsAuthenticated(isAuthenticated);
  };

  login = async (user, hash) => {
    try {
      const response = await api.user.login(user, hash);

      console.log("Login response", response);

      setWithCredentialsStatus(true);

      await this.init();

      return true;
    } catch (e) {
      return false;
    }
  };

  logout = async (withoutRedirect) => {
    const response = await api.user.logout();

    console.log("Logout response ", response);

    setWithCredentialsStatus(false);

    const { isDesktopClient: isDesktop } = this.settingsStore;

    isDesktop && logoutDesktop();

    //dispatch(setLogout());

    //this.setIsAuthenticated(false);
    this.setUserStore(new UserStore());
    this.setModuleStore(new ModuleStore());
    this.setSettingsStore(new SettingsStore());

    this.init();

    if (!withoutRedirect) {
      history.push("/login");
    }
  };

  setIsAuthenticated = (isAuthenticated) => {
    this.isAuthenticated = isAuthenticated;
  };

  setUserStore = (store) => {
    this.userStore = store;
  };

  setModuleStore = (store) => {
    this.moduleStore = store;
  };

  setSettingsStore = (store) => {
    this.settingsStore = store;
  };

  getEncryptionAccess = (fileId) => {
    return api.files
      .getEncryptionAccess(fileId)
      .then((keys) => {
        return Promise.resolve(keys);
      })
      .catch((err) => console.error(err));
  };

  replaceFileStream = (fileId, file, encrypted, forcesave) => {
    return api.files.updateFileStream(file, fileId, encrypted, forcesave);
  };

  setEncryptionAccess = (file) => {
    return this.getEncryptionAccess(file.id).then((keys) => {
      let promise = new Promise((resolve, reject) => {
        try {
          window.AscDesktopEditor.cloudCryptoCommand(
            "share",
            {
              cryptoEngineId: desktopConstants.guid,
              file: [file.viewUrl],
              keys: keys,
            },
            (obj) => {
              let file = null;
              if (obj.isCrypto) {
                let bytes = obj.bytes;
                let filename = "temp_name";
                file = new File([bytes], filename);
              }
              resolve(file);
            }
          );
        } catch (e) {
          reject(e);
        }
      });
      return promise;
    });
  };

  setDocumentTitle = (subTitle = null) => {
    let title;

    const currentModule = this.settingsStore.product;
    const organizationName = this.settingsStore.organizationName;

    if (subTitle) {
      if (this.isAuthenticated && currentModule) {
        title = subTitle + " - " + currentModule.title;
      } else {
        title = subTitle + " - " + organizationName;
      }
    } else if (currentModule && organizationName) {
      title = currentModule.title + " - " + organizationName;
    } else {
      title = organizationName;
    }

    document.title = title;
  };

  setProductVersion = (version) => {
    this.version = version;
  };
}

export default new AuthStore();
