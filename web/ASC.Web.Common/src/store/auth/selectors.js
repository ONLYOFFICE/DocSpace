import { createSelector } from "reselect";
import isEmpty from "lodash/isEmpty";

export function isAdmin(state) {
  const { user } = state.auth;
  const isPeopleAdmin = user.listAdminModules
    ? user.listAdminModules.includes("people")
    : false;
  return user.isAdmin || user.isOwner || isPeopleAdmin;
}

export function isMe(user, userName) {
  return userName === "@self" || (user && userName === user.userName);
}

export const getLanguage = state => {
  const { user, settings } = state.auth;

  return user.cultureName || settings.culture;
};

const toModuleWrapper = (item, iconName) => {
  return {
    id: item.id,
    title: item.title,
    iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
    iconUrl: item.iconUrl,
    notifications: 0,
    url: item.link,
    onClick: e => {
      if (e) {
        window.open(item.link, "_self");
        e.preventDefault();
      }
    },
    onBadgeClick: e => console.log(iconName + " Badge Clicked", e)
  };
};

const getCustomModules = isAdmin => {
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

export const getCurrentUser = state => state.auth.user;

export const getModules = state => state.auth.modules;

export const getAvailableModules = createSelector(
  [getCurrentUser, getModules],
  (user, modules) => {
    if (isEmpty(modules) || isEmpty(user)) {
      return [];
    }

    const isUserAdmin = user.isAdmin;
    const customModules = getCustomModules(isUserAdmin);
    const products = modules.map(m => toModuleWrapper(m));

    return [
      {
        separator: true,
        id: "nav-products-separator"
      },
      ...products,
      ...customModules
    ];
  }
);

export const getIsolateModules = createSelector(
  [getAvailableModules],
  availableModules => {
    const isolateModules = availableModules.filter(item => item.isolateMode);

    return isolateModules;
  }
);

export const getMainModules = createSelector(
  [getAvailableModules],
  availableModules => {
    const mainModules = availableModules.filter(item => !item.isolateMode);

    return mainModules;
  }
);

export const getSettings = state => state.auth.settings;

export const getDefaultPage = state => state.auth.settings.defaultPage;

export const getCurrentProductId = state =>
  state.auth.settings.currentProductId;

export const getCurrentProduct = createSelector(
  [getAvailableModules, getCurrentProductId],
  (availableModules, currentProductId) => {
    if (!currentProductId) return null;

    const list = availableModules.filter(item => item.id == currentProductId);

    return list && list.length > 0 ? list[0] : null;
  }
);

export const getCurrentProductName = createSelector(
  [getCurrentProduct],
  currentProduct => {
    return (currentProduct && currentProduct.title) || "";
  }
);

export const getTotalNotificationsCount = createSelector(
  [getMainModules],
  mainModules => {
    let totalNotifications = 0;
    mainModules
      .filter(item => !item.separator)
      .forEach(item => (totalNotifications += item.notifications || 0));

    return totalNotifications;
  }
);
