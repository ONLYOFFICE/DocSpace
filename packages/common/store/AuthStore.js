import { makeAutoObservable } from "mobx";
import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
import history from "../history";

import SettingsStore from "./SettingsStore";
import UserStore from "./UserStore";
import TfaStore from "./TfaStore";
import InfoPanelStore from "./InfoPanelStore";
import {
  logout as logoutDesktop,
  desktopConstants,
} from "../desktop";
import {
  combineUrl,
  isAdmin,
  setCookie,
  getCookie,
} from "../utils";import {
  AppServerConfig,
  LANGUAGE,
  COOKIE_EXPIRATION_YEAR,
  TenantStatus,
} from "../constants";
const { proxyURL } = AppServerConfig;
class AuthStore {
  userStore = null;

  settingsStore = null;
  tfaStore = null;
  infoPanelStore = null;

  isLoading = false;
  version = null;

  providers = [];
  isInit = false;

  constructor() {
    this.userStore = new UserStore();

    this.settingsStore = new SettingsStore();
    this.tfaStore = new TfaStore();
    this.infoPanelStore = new InfoPanelStore();

    makeAutoObservable(this);
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    try {
      await this.userStore.init();
    } catch (e) {
      console.error(e);
    }

    const requests = [];
    requests.push(this.settingsStore.init());

    if (this.isAuthenticated) {
      !this.settingsStore.passwordSettings &&
        requests.push(this.settingsStore.getPortalPasswordSettings());
    }

    return Promise.all(requests);
  };
  setLanguage() {
    if (this.userStore.user?.cultureName) {
      getCookie(LANGUAGE) !== this.userStore.user.cultureName &&
        setCookie(LANGUAGE, this.userStore.user.cultureName, {
          "max-age": COOKIE_EXPIRATION_YEAR,
        });
    } else {
      setCookie(LANGUAGE, this.settingsStore.culture || "en-US", {
        "max-age": COOKIE_EXPIRATION_YEAR,
      });
    }
  }
  get isLoaded() {
    let success = false;
    if (this.isAuthenticated) {
      success =
        (this.userStore.isLoaded && this.settingsStore.isLoaded) ||
        this.settingsStore.tenantStatus === TenantStatus.PortalRestore;

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
      "en"
    );
  }

  get isAdmin() {
    const { user } = this.userStore;
    const { currentProductId } = this.settingsStore;

    if (!user || !user.id) return false;

    return isAdmin(user, currentProductId);
  }

  login = async (user, hash, session = true) => {
    try {
      const response = await api.user.login(user, hash, session);

      if (!response || (!response.token && !response.tfa))
        throw response.error.message;

      if (response.tfa && response.confirmUrl) {
        const url = response.confirmUrl.replace(window.location.origin, "");
        return Promise.resolve({ url, user, hash });
      }

      setWithCredentialsStatus(true);

      this.reset();

      this.init();

      return Promise.resolve({ url: this.settingsStore.defaultPage });
    } catch (e) {
      return Promise.reject(e);
    }
  };

  loginWithCode = async (userName, passwordHash, code) => {
    await this.tfaStore.loginWithCode(userName, passwordHash, code);
    setWithCredentialsStatus(true);

    this.reset();

    this.init();

    return Promise.resolve(this.settingsStore.defaultPage);
  };

  thirdPartyLogin = async (SerializedProfile) => {
    try {
      const response = await api.user.thirdPartyLogin(SerializedProfile);

      if (!response || !response.token) throw new Error("Empty API response");

      setWithCredentialsStatus(true);

      this.reset();

      this.init();

      return Promise.resolve(this.settingsStore.defaultPage);
    } catch (e) {
      return Promise.reject(e);
    }
  };

  reset = (skipUser = false) => {
    this.isInit = false;
    this.skipModules = false;
    if (!skipUser) {
      this.userStore = new UserStore();
    }

    this.settingsStore = new SettingsStore();
  };

  logout = async (redirectToLogin = true, redirectPath = null) => {
    await api.user.logout();

    //console.log("Logout response ", response);

    setWithCredentialsStatus(false);

    const { isDesktopClient: isDesktop, personal } = this.settingsStore;

    isDesktop && logoutDesktop();

    if (redirectToLogin) {
      if (redirectPath) {
        return window.location.replace(redirectPath);
      }
      if (personal) {
        return window.location.replace("/");
      } else {
        this.reset(true);
        this.userStore.setUser(null);
        this.init();
        return history.push(combineUrl(proxyURL, "/login"));
      }
    } else {
      this.reset();
      this.init();
    }
  };

  get isAuthenticated() {
    return (
      this.userStore.isAuthenticated ||
      this.settingsStore.tenantStatus === TenantStatus.PortalRestore
    );
  }

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
      return new Promise((resolve, reject) => {
        try {
          window.AscDesktopEditor.cloudCryptoCommand(
            "share",
            {
              cryptoEngineId: desktopConstants.cryptoEngineId,
              file: [file.viewUrl],
              keys: keys,
            },
            (obj) => {
              let resFile = null;
              if (obj.isCrypto) {
                let bytes = obj.bytes;
                let filename = "temp_name";
                resFile = new File([bytes], filename);
              }
              resolve(resFile);
            }
          );
        } catch (e) {
          reject(e);
        }
      });
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

  getOforms = () => {
    const culture =
      this.userStore.user.cultureName || this.settingsStore.culture;

    const promise = new Promise(async (resolve, reject) => {
      let oforms = await api.settings.getOforms(
        `${this.settingsStore.urlOforms}${culture}`
      );

      if (!oforms?.data?.data.length) {
        oforms = await api.settings.getOforms(
          `${this.settingsStore.urlOforms}en`
        );
      }

      resolve(oforms);
    });

    return promise;
  };
}

export default new AuthStore();
