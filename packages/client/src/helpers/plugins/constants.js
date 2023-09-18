export const PluginFileType = Object.freeze({
  Files: "file",
  Folders: "folder",
  Rooms: "room",
  Image: "image",
  Video: "video",
});

export const PluginScopes = Object.freeze({
  API: "API",
  Settings: "Settings",
  ContextMenu: "ContextMenu",
  InfoPanel: "InfoPanel",
  MainButton: "MainButton",
  ProfileMenu: "ProfileMenu",
  EventListener: "EventListener",
  File: "File",
});

export const PluginStatus = Object.freeze({
  active: "active",
  hide: "hide",
});

export const PluginActions = Object.freeze({
  updateProps: "update-props",
  updateContext: "update-context",

  updateStatus: "update-status",

  showToast: "show-toast",

  // showSettingsModal: "show-settings-modal",
  // closeSettingsModal: "close-settings-modal",

  showCreateDialogModal: "show-create-dialog-modal",

  showModal: "show-modal",
  closeModal: "close-modal",

  updateContextMenuItems: "update-context-menu-items",
  updateInfoPanelItems: "update-info-panel-items",
  updateMainButtonItems: "update-main-button-items",
  updateProfileMenuItems: "update-profile-menu-items",
  updateFileItems: "update-file-items",
  updateEventListenerItems: "update-event-listener-items",

  sendPostMessage: "send-post-message",
});

export const PluginToastType = Object.freeze({
  success: "success",
  error: "error",
  warning: "warning",
  info: "info",
});

export const PluginComponents = Object.freeze({
  box: "box",
  button: "button",
  checkbox: "checkbox",
  input: "input",
  label: "label",
  text: "text",
  textArea: "textArea",
  toggleButton: "toggleButton",
  img: "img",
  iFrame: "iFrame",
  comboBox: "comboBox",
  skeleton: "skeleton",
});

export const PluginUsersType = Object.freeze({
  owner: "Owner",
  docSpaceAdmin: "DocSpaceAdmin",
  roomAdmin: "RoomAdmin",
  collaborator: "Collaborator",
  user: "User",
});

export const PluginDevices = Object.freeze({
  mobile: "mobile",
  tablet: "tablet",
  desktop: "desktop",
});
