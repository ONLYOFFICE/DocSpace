import { makeAutoObservable } from "mobx";
import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
import history from "../history";
import ModuleStore from "./ModuleStore";
import SettingsStore from "./SettingsStore";
import UserStore from "./UserStore";
import { logout as logoutDesktop, desktopConstants } from "../desktop";
import { combineUrl, isAdmin } from "../utils";
import isEmpty from "lodash/isEmpty";
import { AppServerConfig, LANGUAGE } from "../constants";
const { proxyURL } = AppServerConfig;

class AuthStore {
  userStore = null;
  moduleStore = null;
  settingsStore = null;

  isLoading = false;
  isAuthenticated = false;
  version = null;

  providers = [];

  constructor() {
    this.userStore = new UserStore();
    this.moduleStore = new ModuleStore();
    this.settingsStore = new SettingsStore();

    makeAutoObservable(this);
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
  setLanguage() {
    if (this.userStore.user.cultureName) {
      localStorage.getItem(LANGUAGE) !== this.userStore.user.cultureName &&
        localStorage.setItem(LANGUAGE, this.userStore.user.cultureName);
    } else {
      localStorage.setItem(LANGUAGE, this.settingsStore.culture || "en-US");
    }
  }
  get isLoaded() {
    let success = false;
    if (this.isAuthenticated) {
      success =
        this.userStore.isLoaded &&
        this.moduleStore.isLoaded &&
        this.settingsStore.isLoaded;

      success && this.setLanguage();
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
    return (
      this.moduleStore.modules.find(
        (item) => item.id === this.settingsStore.currentProductId
      ) || ""
    );
  }

  get availableModules() {
    const { user } = this.userStore;
    const { modules } = this.moduleStore;
    if (isEmpty(modules) || isEmpty(this.userStore.user)) {
      return [];
    }

    const isUserAdmin = user.isAdmin;
    const customProducts = this.getCustomModules(isUserAdmin);
    const readyProducts = [];
    const inProgressProducts = [];
    modules.forEach((p) => {
      if (p.appName === "people" || p.appName === "files") {
        readyProducts.push(p);
      } else {
        inProgressProducts.push(p);
      }
    });

    return [
      {
        separator: true,
        id: "nav-products-separator",
      },
      ...readyProducts,
      ...customProducts,
      {
        separator: true,
        dashed: true,
        id: "nav-dummy-products-separator",
      },
      ...inProgressProducts,
    ];
  }

  getCustomModules = (isAdmin) => {
    if (!isAdmin) {
      return [];
    }
    const settingsModuleWrapper = this.moduleStore.toModuleWrapper({
      id: "settings",
      title: "Settings",
      link: "/settings",
      iconUrl: "/static/images/settings.react.svg",
    });

    settingsModuleWrapper.onClick = this.onClick;
    settingsModuleWrapper.onBadgeClick = this.onBadgeClick;

    return [settingsModuleWrapper];
  };

  getIsAuthenticated = async () => {
    const isAuthenticated = await api.user.checkIsAuthenticated();
    this.setIsAuthenticated(isAuthenticated);
  };

  login = async (user, hash) => {
    try {
      const response = await api.user.login(user, hash);

      if (!response || !response.token) throw "Empty API response";

      setWithCredentialsStatus(true);

      await this.init();

      return Promise.resolve(true);
    } catch (e) {
      return Promise.reject(e);
    }
  };

  thirdPartyLogin = async (SerializedProfile) => {
    try {
      const response = await api.user.thirdPartyLogin(SerializedProfile);

      if (!response || !response.token) throw "Empty API response";

      setWithCredentialsStatus(true);

      await this.init();

      return Promise.resolve(true);
    } catch (e) {
      return Promise.reject(e);
    }
  };

  reset = () => {
    this.userStore = new UserStore();
    this.moduleStore = new ModuleStore();
    this.settingsStore = new SettingsStore();
  };

  logout = async (withoutRedirect) => {
    const response = await api.user.logout();

    //console.log("Logout response ", response);

    setWithCredentialsStatus(false);

    const { isDesktopClient: isDesktop } = this.settingsStore;

    isDesktop && logoutDesktop();

    this.reset();

    this.init();

    if (!withoutRedirect) {
      history.push(combineUrl(proxyURL, "/login"));
    }
  };

  setIsAuthenticated = (isAuthenticated) => {
    this.isAuthenticated = isAuthenticated;
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

  setProviders = (providers) => {
    this.providers = providers;
  };
}

export default new AuthStore();
