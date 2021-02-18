import { createSelector } from "reselect";
import isEmpty from "lodash/isEmpty";

export function isMe(user, userName) {
  return (
    user && user.id && (userName === "@self" || user.userName === userName)
  );
}

const toModuleWrapper = (item, noAction = true, iconName = null) => {
  switch (item.id) {
    case "6743007c-6f95-4d20-8c88-a8601ce5e76d":
      item.iconName = "CrmIcon";
      item.iconUrl = "";
      item.imageUrl = "/images/crm.svg";
      break;
    case "1e044602-43b5-4d79-82f3-fd6208a11960":
      item.iconName = "ProjectsIcon";
      item.iconUrl = "";
      item.imageUrl = "/images/projects.svg";
      break;
    case "2A923037-8B2D-487b-9A22-5AC0918ACF3F":
      item.iconName = "MailIcon";
      item.iconUrl = "";
      item.imageUrl = "/images/mail.svg";
      break;
    case "32D24CB5-7ECE-4606-9C94-19216BA42086":
      item.iconName = "CalendarCheckedIcon";
      item.iconUrl = "";
      item.imageUrl = "/images/calendar.svg";
      break;
    case "BF88953E-3C43-4850-A3FB-B1E43AD53A3E":
      item.iconName = "ChatIcon";
      item.iconUrl = "";
      item.imageUrl = "/images/talk.svg";
      item.isolateMode = true;
      break;
    default:
      break;
  }

  const actions = noAction
    ? null
    : {
        onClick: (e) => {
          if (e) {
            window.open(item.link, "_self");
            e.preventDefault();
          }
        },
        onBadgeClick: (e) => console.log(iconName + " Badge Clicked", e),
      };

  const description = noAction ? { description: item.description } : null;

  return {
    id: item.id,
    title: item.title,
    link: item.link,
    iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
    iconUrl: item.iconUrl,
    imageUrl: item.imageUrl,
    notifications: 0,
    isolateMode: item.isolateMode,
    isPrimary: item.isPrimary,
    ...description,
    ...actions,
  };
};

const getCustomModules = (isAdmin) => {
  if (!isAdmin) {
    return [];
  }
  const settingsModuleWrapper = toModuleWrapper(
    {
      id: "settings",
      title: "Settings",
      link: "/settings",
    },
    false,
    "SettingsIcon"
  );

  return [
    {
      separator: true,
      id: "nav-products-separator-custom",
    },
    settingsModuleWrapper,
  ];
};

export const getCurrentUser = (state) => state.auth.user;

export const isAuthenticated = (state) => state.auth.isAuthenticated;

export const getCurrentUserId = (state) => state.auth.user.id;

export const getModules = (state) => {
  const modules = state.auth.modules;

  const extendedModules = [
    ...modules,
    {
      id: "2A923037-8B2D-487b-9A22-5AC0918ACF3F",
      title: "Mail",
      link: "/products/mail/",
      isPrimary: false,
    },
    {
      id: "32D24CB5-7ECE-4606-9C94-19216BA42086",
      title: "Calendar",
      link: "/products/calendar/",
      isPrimary: false,
    },
    {
      id: "BF88953E-3C43-4850-A3FB-B1E43AD53A3E",
      title: "Talk",
      link: "/products/talk/",
      isPrimary: false,
    },
  ].map((m) => toModuleWrapper(m));

  return extendedModules;
};

export const getSettings = (state) => state.auth.settings;

export const getSettingsHomepage = (state) => state.auth.settings.homepage;

export const getSettingsCustomNames = (state) =>
  state.auth.settings.customNames;

export const getSettingsCustomNamesGroupsCaption = (state) =>
  state.auth.settings.customNames.groupsCaption;

export const getIsLoaded = (state) => state.auth.isLoaded;

export const getIsLoadedSection = (state) => state.auth.isLoadedSection;

export const getIsTabletView = (state) => state.auth.settings.isTabletView;

export const getDefaultPage = createSelector(
  [getSettings],
  (settings) => (settings && settings.defaultPage) || ""
);

export const getCurrentProductId = createSelector(
  [getSettings],
  (settings) => (settings && settings.currentProductId) || ""
);

export const getLanguage = createSelector(
  [getCurrentUser, getSettings],
  (user, settings) => {
    return (
      (user && user.cultureName) || (settings && settings.culture) || "en-US"
    );
  }
);

export const isVisitor = createSelector([getCurrentUser], (currentUser) => {
  return (currentUser && currentUser.isVisitor) || false;
});

export const isAdmin = createSelector(
  [getCurrentUser, getCurrentProductId],
  (currentUser, currentProductId) => {
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
);

export const getAvailableModules = createSelector(
  [getCurrentUser, getModules],
  (user, modules) => {
    if (isEmpty(modules) || isEmpty(user)) {
      return [];
    }

    const isUserAdmin = user.isAdmin;
    const customModules = getCustomModules(isUserAdmin);
    const products = modules.map((m) => toModuleWrapper(m, false));

    return [
      {
        separator: true,
        id: "nav-products-separator",
      },
      ...products,
      ...customModules,
    ];
  }
);

export const getIsolateModules = createSelector(
  [getAvailableModules],
  (availableModules) => {
    const isolateModules = availableModules.filter((item) => item.isolateMode);

    return isolateModules;
  }
);

export const getMainModules = createSelector(
  [getAvailableModules],
  (availableModules) => {
    const mainModules = availableModules.filter((item) => !item.isolateMode);

    return mainModules;
  }
);

export const getCurrentProduct = createSelector(
  [getAvailableModules, getCurrentProductId],
  (availableModules, currentProductId) => {
    if (!currentProductId) return null;

    const list = availableModules.filter((item) => item.id == currentProductId);

    return list && list.length > 0 ? list[0] : null;
  }
);

export const getCurrentProductName = createSelector(
  [getCurrentProduct],
  (currentProduct) => {
    return (currentProduct && currentProduct.title) || "";
  }
);

export const getTotalNotificationsCount = createSelector(
  [getMainModules],
  (mainModules) => {
    let totalNotifications = 0;
    mainModules
      .filter((item) => !item.separator)
      .forEach((item) => (totalNotifications += item.notifications || 0));

    return totalNotifications;
  }
);

export const isEncryptionSupport = createSelector([getSettings], (settings) => {
  const { isEncryptionSupport } = settings;
  return isEncryptionSupport;
});

export const getOrganizationName = createSelector([getSettings], (settings) => {
  const { organizationName } = settings;
  return organizationName;
});

export const getUrlSupport = (state) => state.auth.settings.urlSupport;

export const getUrlAuthKeys = (state) => state.auth.settings.urlAuthKeys;

export const getHeaderVisible = createSelector([getSettings], (settings) => {
  const { isHeaderVisible } = settings;
  return isHeaderVisible;
});

export const isDesktopClient = createSelector([getSettings], (settings) => {
  const { isDesktopClient } = settings;
  return isDesktopClient || false;
});

export const isArticlePinned = createSelector([getSettings], (settings) => {
  const { isArticlePinned } = settings;
  return isArticlePinned || false;
});
