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
  updateContext: "update-context",

  updateStatus: "update-status",

  showToast: "show-toast",

  showSettingsModal: "show-settings-modal",

  showCreateDialogModal: "show-create-dialog-modal",

  showModal: "show-modal",
  closeModal: "close-modal",
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
});

export const PluginUsersType = Object.freeze({
  owner: "Owner",
  docSpaceAdmin: "DocSpaceAdmin",
  roomAdmin: "RoomAdmin",
  collaborator: "Collaborator",
  user: "User",
});
