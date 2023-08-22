import CatalogSettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import HotkeysReactSvgUrl from "PUBLIC_DIR/images/hotkeys.react.svg?url";
import ProfileReactSvgUrl from "PUBLIC_DIR/images/profile.react.svg?url";
import PaymentsReactSvgUrl from "PUBLIC_DIR/images/payments.react.svg?url";
import HelpCenterReactSvgUrl from "PUBLIC_DIR/images/help.center.react.svg?url";
import EmailReactSvgUrl from "PUBLIC_DIR/images/email.react.svg?url";
import LiveChatReactSvgUrl from "PUBLIC_DIR/images/support.react.svg?url";
import BookTrainingReactSvgUrl from "PUBLIC_DIR/images/book.training.react.svg?url";
//import VideoGuidesReactSvgUrl from "PUBLIC_DIR/images/video.guides.react.svg?url";
import InfoOutlineReactSvgUrl from "PUBLIC_DIR/images/info.outline.react.svg?url";
import LogoutReactSvgUrl from "PUBLIC_DIR/images/logout.react.svg?url";
import { makeAutoObservable } from "mobx";
import { combineUrl } from "@docspace/common/utils";

import { isDesktop, isTablet, isMobile } from "react-device-detect";
import { getProfileMenuItems } from "SRC_DIR/helpers/plugins";
import { ZendeskAPI } from "@docspace/common/components/Zendesk";
import { LIVE_CHAT_LOCAL_STORAGE_KEY } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

const PROXY_HOMEPAGE_URL = combineUrl(window.DocSpaceConfig?.proxy?.url, "/");
const PROFILE_SELF_URL = combineUrl(PROXY_HOMEPAGE_URL, "/profile");
//const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");
const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
const PAYMENTS_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/portal-settings/payments/portal-payments"
);

//const VIDEO_GUIDES_URL = "https://onlyoffice.com/";

class ProfileActionsStore {
  authStore = null;
  filesStore = null;
  peopleStore = null;
  treeFoldersStore = null;
  selectedFolderStore = null;
  isAboutDialogVisible = false;
  isDebugDialogVisible = false;
  isShowLiveChat = false;
  profileClicked = false;

  constructor(
    authStore,
    filesStore,
    peopleStore,
    treeFoldersStore,
    selectedFolderStore
  ) {
    this.authStore = authStore;
    this.filesStore = filesStore;
    this.peopleStore = peopleStore;
    this.treeFoldersStore = treeFoldersStore;
    this.selectedFolderStore = selectedFolderStore;

    this.isShowLiveChat = this.getStateLiveChat();

    makeAutoObservable(this);
  }

  getStateLiveChat = () => {
    const state = localStorage.getItem(LIVE_CHAT_LOCAL_STORAGE_KEY) === "true";

    if (!state) return false;

    return state;
  };

  setStateLiveChat = (state) => {
    if (typeof state !== "boolean") return;

    localStorage.setItem(LIVE_CHAT_LOCAL_STORAGE_KEY, state.toString());

    this.isShowLiveChat = state;
  };

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
    if (user.isVisitor) return "user";
    return "manager";
  };

  onProfileClick = () => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;
    const { isRoomAdmin } = this.authStore;

    this.profileClicked = true;
    const prefix = window.DocSpace.location.pathname.includes("portal-settings")
      ? "/portal-settings"
      : "";

    if ((isAdmin || isOwner || isRoomAdmin) && !prefix) {
      this.selectedFolderStore.setSelectedFolder(null);
      this.treeFoldersStore.setSelectedNode(["accounts"]);
    }

    const state = {
      fromUrl: `${window.DocSpace.location.pathname}${window.DocSpace.location.search}`,
    };

    window.DocSpace.navigate(`${prefix}${PROFILE_SELF_URL}`, { state });
  };

  onSettingsClick = (settingsUrl) => {
    this.selectedFolderStore.setSelectedFolder(null);
    window.DocSpace.navigate(settingsUrl);
  };

  onPaymentsClick = () => {
    this.selectedFolderStore.setSelectedFolder(null);
    window.DocSpace.navigate(PAYMENTS_URL);
  };

  onHelpCenterClick = () => {
    const helpUrl = this.authStore.settingsStore.helpLink;

    window.open(helpUrl, "_blank");
  };

  onLiveChatClick = (t) => {
    const isShow = !this.isShowLiveChat;

    this.setStateLiveChat(isShow);

    ZendeskAPI("webWidget", isShow ? "show" : "hide");

    toastr.success(isShow ? t("LiveChatOn") : t("LiveChatOff"));
  };

  onSupportClick = () => {
    const supportUrl =
      this.authStore.settingsStore.additionalResourcesData
        ?.feedbackAndSupportUrl;

    window.open(supportUrl, "_blank");
  };

  onBookTraining = () => {
    const trainingEmail = this.authStore.settingsStore?.bookTrainingEmail;

    trainingEmail && window.open(`mailto:${trainingEmail}`, "_blank");
  };

  // onVideoGuidesClick = () => {
  //   window.open(VIDEO_GUIDES_URL, "_blank");
  // };

  onHotkeysClick = () => {
    this.authStore.settingsStore.setHotkeyPanelVisible(true);
  };

  onAboutClick = () => {
    if (isDesktop || isTablet) {
      this.setIsAboutDialogVisible(true);
    } else {
      window.DocSpace.navigate(ABOUT_URL);
    }
  };

  onLogoutClick = () => {
    this.authStore.logout().then(() => {
      this.filesStore.reset();
      this.peopleStore.reset();
    });
  };

  onDebugClick = () => {
    this.setIsDebugDialogVisible(true);
  };

  getActions = (t) => {
    const { enablePlugins, standalone } = this.authStore.settingsStore;
    const isAdmin = this.authStore.isAdmin;
    const isCommunity = this.authStore.isCommunity;

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
          key: "user-menu-settings",
          icon: CatalogSettingsReactSvgUrl,
          label: t("Common:SettingsDocSpace"),
          onClick: () => this.onSettingsClick(settingsUrl),
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
        key: "user-menu-hotkeys",
        icon: HotkeysReactSvgUrl,
        label: t("Common:Hotkeys"),
        onClick: this.onHotkeysClick,
      };
    }
    // }

    let liveChat = null;

    if (!isMobile && this.authStore.isLiveChatAvailable) {
      liveChat = {
        key: "user-menu-live-chat",
        icon: LiveChatReactSvgUrl,
        label: t("Common:LiveChat"),
        onClick: () => this.onLiveChatClick(t),
        checked: this.isShowLiveChat,
        withToggle: true,
      };
    }

    let bookTraining = null;

    if (!isMobile && this.authStore.isTeamTrainingAlertAvailable) {
      bookTraining = {
        key: "user-menu-book-training",
        icon: BookTrainingReactSvgUrl,
        label: t("Common:BookTraining"),
        onClick: this.onBookTraining,
      };
    }

    const actions = [
      {
        key: "user-menu-profile",
        icon: ProfileReactSvgUrl,
        label: t("Common:Profile"),
        onClick: this.onProfileClick,
      },
      settings,
      isAdmin &&
        !isCommunity && {
          key: "user-menu-payments",
          icon: PaymentsReactSvgUrl,
          label: t("Common:PaymentsTitle"),
          onClick: this.onPaymentsClick,
        },
      {
        isSeparator: true,
        key: "separator1",
      },
      {
        key: "user-menu-help-center",
        icon: HelpCenterReactSvgUrl,
        label: t("Common:HelpCenter"),
        onClick: this.onHelpCenterClick,
      },
      // {
      //   key: "user-menu-video",
      //   icon: VideoGuidesReactSvgUrl,
      //   label: "VideoGuides",
      //   onClick: this.onVideoGuidesClick,
      // },
      hotkeys,
      {
        isSeparator: true,
        key: "separator2",
      },
      liveChat,
      {
        key: "user-menu-support",
        icon: EmailReactSvgUrl,
        label: t("Common:FeedbackAndSupport"),
        onClick: this.onSupportClick,
      },
      bookTraining,
      {
        key: "user-menu-about",
        icon: InfoOutlineReactSvgUrl,
        label: t("Common:AboutCompanyTitle"),
        onClick: this.onAboutClick,
      },
      {
        isSeparator: true,
        key: "separator3",
      },
      {
        key: "user-menu-logout",
        icon: LogoutReactSvgUrl,
        label: t("Common:LogoutButton"),
        onClick: this.onLogoutClick,
        isButton: true,
      },
    ];

    if (debugInfo) {
      actions.splice(4, 0, {
        key: "user-menu-debug",
        icon: InfoOutlineReactSvgUrl,
        label: "Debug Info",
        onClick: this.onDebugClick,
      });
    }

    if (enablePlugins) {
      const pluginActions = getProfileMenuItems();

      if (pluginActions) {
        pluginActions.forEach((option) => {
          actions.splice(option.value.position, 0, {
            key: option.key,
            ...option.value,
          });
        });
      }
    }

    return this.checkEnabledActions(actions);
  };

  checkEnabledActions = (actions) => {
    const actionsArray = actions;

    if (!this.authStore.settingsStore.additionalResourcesData) {
      return actionsArray;
    }

    const feedbackAndSupportEnabled =
      this.authStore.settingsStore.additionalResourcesData
        ?.feedbackAndSupportEnabled;
    const videoGuidesEnabled =
      this.authStore.settingsStore.additionalResourcesData?.videoGuidesEnabled;
    const helpCenterEnabled =
      this.authStore.settingsStore.additionalResourcesData?.helpCenterEnabled;

    if (!feedbackAndSupportEnabled) {
      const index = actionsArray.findIndex(
        (item) => item?.key === "user-menu-support"
      );

      actionsArray.splice(index, 1);
    }

    if (!videoGuidesEnabled) {
      const index = actionsArray.findIndex(
        (item) => item?.key === "user-menu-video"
      );

      actionsArray.splice(index, 1);
    }

    if (!helpCenterEnabled) {
      const index = actionsArray.findIndex(
        (item) => item?.key === "user-menu-help-center"
      );

      actionsArray.splice(index, 1);
    }

    return actionsArray;
  };
}

export default ProfileActionsStore;
