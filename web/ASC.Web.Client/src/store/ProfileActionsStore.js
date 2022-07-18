import { makeAutoObservable } from "mobx";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import history from "@appserver/common/history";
import authStore from "@appserver/common/store/AuthStore";
import { isMobile } from "react-device-detect";

const { proxyURL } = AppServerConfig;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, "/");
const PROFILE_SELF_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/products/people/view/@self"
);
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");

class ProfileActionsStore {
  authStore = null;
  isAboutDialogVisible = false;

  constructor() {
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  setIsAboutDialogVisible = (visible) => {
    this.isAboutDialogVisible = visible;
  };

  getUserRole = (user) => {
    let isModuleAdmin = user.listAdminModules && user.listAdminModules.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  onProfileClick = () => {
    this.authStore.availableModules.some((m) => m.appName === "people")
      ? history.push(PROFILE_SELF_URL)
      : history.push(PROFILE_MY_URL);
  };

  onSettingsClick = () => {
    const settingsModule = this.authStore.availableModules.find(
      (module) => module.id === "settings"
    );
    const settingsUrl =
      settingsModule && combineUrl(PROXY_HOMEPAGE_URL, settingsModule.link);
    history.push(settingsUrl);
  };

  onHotkeysClick = () => {
    this.authStore.settingsStore.setHotkeyPanelVisible(true);
  };

  onAboutClick = () => {
    this.setIsAboutDialogVisible(true);
  };

  onLogoutClick = () => {
    this.authStore.logout && this.authStore.logout();
  };

  onDebugClick = () => {
    console.log("on debug click");
  };

  getActions = (t) => {
    const modules = this.authStore.availableModules;
    const settingsModule = modules.find((module) => module.id === "settings");
    const {
      isPersonal,
      currentProductId,
      debugInfo,
    } = this.authStore.settingsStore;

    const settings =
      settingsModule && !isPersonal
        ? {
            key: "SettingsBtn",
            icon: "/static/images/catalog.settings.react.svg",
            label: t("Common:Settings"),
            onClick: this.onSettingsClick,
          }
        : null;

    let hotkeys = null;
    if (modules) {
      const moduleIndex = modules.findIndex((m) => m.appName === "files");

      if (
        moduleIndex !== -1 &&
        modules[moduleIndex].id === currentProductId &&
        !isMobile
      ) {
        hotkeys = {
          key: "HotkeysBtn",
          icon: "/static/images/hotkeys.react.svg",
          label: t("Common:Hotkeys"),
          onClick: this.onHotkeysClick,
        };
      }
    }
    const actions = [
      {
        key: "ProfileBtn",
        icon: "/static/images/profile.react.svg",
        label: t("Common:Profile"),
        onClick: this.onProfileClick,
      },
      settings,
      hotkeys,
      {
        key: "AboutBtn",
        icon: "/static/images/info.react.svg",
        label: t("AboutCompanyTitle"),
        onClick: this.onAboutClick,
      },
      {
        isSeparator: true,
        key: "separator",
      },
      {
        key: "LogoutBtn",
        icon: "/static/images/logout.react.svg",
        label: t("LogoutButton"),
        onClick: this.onLogoutClick,
      },
    ];

    if (debugInfo) {
      actions.splice(3, 0, {
        key: "DebugBtn",
        label: "Debug Info",
        onClick: this.onDebugClick,
      });
    }

    return actions;
  };
}

export default ProfileActionsStore;
