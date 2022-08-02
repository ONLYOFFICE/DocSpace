import { makeAutoObservable } from "mobx";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import history from "@docspace/common/history";
import authStore from "@docspace/common/store/AuthStore";
import { isDesktop, isTablet, isMobile } from "react-device-detect";

const { proxyURL } = AppServerConfig;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, "/");
const PROFILE_SELF_URL = combineUrl(PROXY_HOMEPAGE_URL, "/accounts/view/@self");
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");
const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
const PAYMENTS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/payments");
const HELP_URL = "https://onlyoffice.com/";
const SUPPORT_URL = "https://onlyoffice.com/";
const VIDEO_GUIDES_URL = "https://onlyoffice.com/";

class ProfileActionsStore {
  authStore = null;
  isAboutDialogVisible = false;
  isDebugDialogVisible = false;

  constructor() {
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  setIsAboutDialogVisible = (visible) => {
    this.isAboutDialogVisible = visible;
  };

  setIsDebugDialogVisible = (visible) => {
    this.isDebugDialogVisible = visible;
  };

  getUserRole = (user) => {
    let isModuleAdmin =
      user?.listAdminModules && user?.listAdminModules?.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  onProfileClick = () => {
    history.push(PROFILE_SELF_URL);
  };

  onSettingsClick = (settingsUrl) => {
    history.push(settingsUrl);
  };

  onPaymentsClick = () => {
    history.push(PAYMENTS_URL);
  };

  onHelpCenterClick = () => {
    window.open(HELP_URL, "_blank");
  };

  onSupportClick = () => {
    window.open(SUPPORT_URL, "_blank");
  };

  onVideoGuidesClick = () => {
    window.open(VIDEO_GUIDES_URL, "_blank");
  };

  onHotkeysClick = () => {
    this.authStore.settingsStore.setHotkeyPanelVisible(true);
  };

  onAboutClick = () => {
    if (isDesktop || isTablet) {
      this.setIsAboutDialogVisible(true);
    } else {
      history.push(ABOUT_URL);
    }
  };

  onLogoutClick = () => {
    this.authStore.logout && this.authStore.logout();
  };

  onDebugClick = () => {
    this.setIsDebugDialogVisible(true);
  };

  getActions = (t) => {
    const isAdmin = this.authStore.isAdmin;
    // const settingsModule = modules.find((module) => module.id === "settings");
    // const peopleAvailable = modules.some((m) => m.appName === "people");
    const settingsUrl = "/portal-settings";
    //   settingsModule && combineUrl(PROXY_HOMEPAGE_URL, settingsModule.link);

    const {
      //isPersonal,
      //currentProductId,
      debugInfo,
    } = this.authStore.settingsStore;

    const settings = isAdmin
      ? {
          key: "SettingsBtn",
          icon: "/static/images/catalog.settings.react.svg",
          label: t("Common:Settings"),
          onClick: () => this.onSettingsClick(settingsUrl),
          url: settingsUrl,
        }
      : null;

    let hotkeys = null;
    // if (modules) {
    //   const moduleIndex = modules.findIndex((m) => m.appName === "files");

    if (
      // moduleIndex !== -1 &&
      // modules[moduleIndex].id === currentProductId &&
      !isMobile
    ) {
      hotkeys = {
        key: "HotkeysBtn",
        icon: "/static/images/hotkeys.react.svg",
        label: t("Common:Hotkeys"),
        onClick: this.onHotkeysClick,
      };
    }
    // }
    const actions = [
      {
        key: "ProfileBtn",
        icon: "/static/images/profile.react.svg",
        label: t("Common:Profile"),
        onClick: this.onProfileClick,
        url: PROFILE_SELF_URL,
      },
      settings,
      {
        key: "PaymentsBtn",
        icon: "/static/images/payments.react.svg",
        label: t("Common:PaymentsTitle"),
        onClick: this.onPaymentsClick,
        url: PAYMENTS_URL,
      },
      {
        key: "HelpCenterBtn",
        icon: "/static/images/help.center.react.svg",
        label: t("Common:HelpCenter"),
        onClick: this.onHelpCenterClick,
        url: HELP_URL,
      },
      {
        key: "SupportBtn",
        icon: "/static/images/support.react.svg",
        label: t("Common:FeedbackAndSupport"),
        onClick: this.onSupportClick,
        url: SUPPORT_URL,
      },
      {
        key: "VideoBtn",
        icon: "/static/images/video.guides.react.svg",
        label: t("Common:VideoGuides"),
        onClick: this.onVideoGuidesClick,
        url: VIDEO_GUIDES_URL,
      },
      hotkeys,
      {
        key: "AboutBtn",
        icon: "/static/images/info.react.svg",
        label: t("Common:AboutCompanyTitle"),
        onClick: this.onAboutClick,
      },
      {
        isSeparator: true,
        key: "separator",
      },
      {
        key: "LogoutBtn",
        icon: "/static/images/logout.react.svg",
        label: t("Common:LogoutButton"),
        onClick: this.onLogoutClick,
        isButton: true,
      },
    ];

    if (debugInfo) {
      actions.splice(3, 0, {
        key: "DebugBtn",
        icon: "/static/images/info.react.svg",
        label: "Debug Info",
        onClick: this.onDebugClick,
      });
    }

    return actions;
  };
}

export default ProfileActionsStore;
