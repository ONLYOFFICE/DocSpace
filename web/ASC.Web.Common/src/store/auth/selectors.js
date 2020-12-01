import { createSelector } from "reselect";
import isEmpty from "lodash/isEmpty";

export function isMe(user, userName) {
  return (
    user && user.id && (userName === "@self" || user.userName === userName)
  );
}

const toModuleWrapper = (item, iconName) => {
  return {
    id: item.id,
    title: item.title,
    iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
    iconUrl: item.iconUrl,
    notifications: 0,
    url: item.link,
    onClick: (e) => {
      if (e) {
        window.open(item.link, "_self");
        e.preventDefault();
      }
    },
    onBadgeClick: (e) => console.log(iconName + " Badge Clicked", e),
  };
};

const getCustomModules = (isAdmin) => {
  if (!isAdmin) {
    return [];
  } // Temporarily hiding the settings module

  /*  const separator = getSeparator("nav-modules-separator");
      const settingsModuleWrapper = toModuleWrapper(
        {
          id: "settings",
          title: i18n.t('Settings'),
          link: "/settings"
        },
        "SettingsIcon"
      );
    
      return [separator, settingsModuleWrapper];*/ return [];
};

export const getCurrentUser = (state) => state.auth.user;

export const getCurrentUserId = (state) => state.auth.user;

export const getModules = (state) => state.auth.modules;

export const getSettings = (state) => state.auth.settings;

export const getSettingsHomepage = (state) => state.auth.settings.homepage;

export const getSettingsCustomNames = (state) =>
  state.auth.settings.customNames;

export const getSettingsCustomNamesGroupsCaption = (state) =>
  state.auth.settings.customNames.groupsCaption;

export const getIsLoaded = (state) => state.auth.isLoaded;

export const getIsLoadedSection = (state) => state.auth.isLoadedSection;

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
    const products = modules.map((m) => toModuleWrapper(m));

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
  return isEncryptionSupport || false;
});

export const getOrganizationName = createSelector([getSettings], (settings) => {
  const { organizationName } = settings;
  return organizationName;
});

export const getUrlSupport = (state) => state.auth.settings.urlSupport;

export const getUrlAuthKeys = (state) => state.auth.settings.urlAuthKeys;
