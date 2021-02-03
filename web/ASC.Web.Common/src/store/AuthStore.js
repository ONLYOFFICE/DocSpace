import { action, computed, makeObservable, observable } from "mobx";
import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
import history from "../history";
import ModuleStore from "./ModuleStore";
import SettingsStore from "./SettingsStore";
import UserStore from "./UserStore";
import { logout as logoutDesktop } from "../desktop";

class AuthStore {
  userStore = null;
  moduleStore = null;
  settingsStore = null;

  isLoading = false;
  isAuthenticated = false;

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
      userStore: observable,
      moduleStore: observable,
      settingsStore: observable,
      init: action,
      login: action,
      logout: action,
      setIsAuthenticated: action,
      setUserStore: action,
      setModuleStore: action,
      setSettingsStore: action,
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

    // await this.settingsStore.init();

    // if (!this.isAuthenticated) return;

    // await this.userStore.init();
    // await this.moduleStore.init();
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
    const { user: currentUser } = this.userStore;
    const { currentProductId } = this.settingsStore;

    if (!currentUser || !currentUser.id) return false;

    let productName = null;

    switch (currentProductId) {
      case "f4d98afd-d336-4332-8778-3c6945c81ea0":
        productName = "people";
        break;
      case "e67be73d-f9ae-4ce1-8fec-1880cb518cb4":
        productName = "documents";
        break;
      default:
        break;
    }

    const isProductAdmin =
      currentUser.listAdminModules && productName
        ? currentUser.listAdminModules.includes(productName)
        : false;

    return currentUser.isAdmin || currentUser.isOwner || isProductAdmin;
  }

  get product() {
    return this.moduleStore.modules.find(
      (item) => item.id === this.settingsStore.currentProductId
    );
  }

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
}

export default new AuthStore();
