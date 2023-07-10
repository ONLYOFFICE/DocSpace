export const PluginContextMenuItemType = Object.freeze({
  Files: "Files",
  Folders: "Folders",
  Rooms: "Rooms",
  All: "All",
});

export const PluginType = Object.freeze({
  CONTEXT_MENU: "context menu",
  ACTION_BUTTON: "action button",
  PROFILE_MENU: "profile menu",
});

export const PluginStatus = Object.freeze({
  active: "active",
  hide: "hide",
  pending: "pending",
});

export const PluginSettingsType = Object.freeze({
  modal: "modal",
  settingsPage: "settings-page",
  both: "both",
});

export const PluginActions = Object.freeze({
  updateProps: "update-props",
  updateAcceptButtonProps: "update-accept-button-props",
  updateLoading: "update-loading",
  updateStatus: "update-status",
  showToast: "show-toast",
  closeModal: "close-modal",
});

export const PluginToastType = Object.freeze({
  success: "success",
  error: "error",
  warning: "warning",
  info: "info",
});
