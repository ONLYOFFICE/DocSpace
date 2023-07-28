import { makeAutoObservable } from "mobx";
import { cloneDeep } from "lodash";

import api from "@docspace/common/api";

import defaultConfig from "PUBLIC_DIR/scripts/config.json";

import {
  PluginFileType,
  PluginScopes,
  PluginUsersType,
} from "SRC_DIR/helpers/plugins/constants";
import { getPluginUrl, messageActions } from "SRC_DIR/helpers/plugins/utils";

let { api: apiConf, proxy: proxyConf } = defaultConfig;
let { orign: apiOrigin, prefix: apiPrefix } = apiConf;
let { url: proxyURL } = proxyConf;

const origin =
  window.DocSpaceConfig?.api?.origin || apiOrigin || window.location.origin;
const proxy = window.DocSpaceConfig?.proxy?.url || proxyURL;
const prefix = window.DocSpaceConfig?.api?.prefix || apiPrefix;

class PluginStore {
  authStore = null;
  selectedFolderStore = null;

  plugins = null;

  contextMenuItems = null;
  infoPanelItems = null;
  mainButtonItems = null;
  profileMenuItems = null;
  eventListenerItems = null;

  pluginFrame = null;

  isInit = false;

  settingsPluginDialogVisible = false;
  currentSettingsDialogPlugin = null;
  isAdminSettingsDialog = false;

  pluginDialogVisible = false;
  pluginDialogProps = null;

  constructor(authStore, selectedFolderStore) {
    this.authStore = authStore;
    this.selectedFolderStore = selectedFolderStore;

    this.plugins = [];

    this.contextMenuItems = new Map();
    this.infoPanelItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();
    this.eventListenerItems = new Map();

    makeAutoObservable(this);
  }

  setCurrentSettingsDialogPlugin = (value) => {
    this.currentSettingsDialogPlugin = value;
  };

  setSettingsPluginDialogVisible = (value) => {
    this.settingsPluginDialogVisible = value;
  };

  setIsAdminSettingsDialog = (value) => {
    this.isAdminSettingsDialog = value;
  };

  setPluginDialogVisible = (value) => {
    this.pluginDialogVisible = value;
  };

  setPluginDialogProps = (value) => {
    this.pluginDialogProps = value;
  };

  updatePluginStatus = (id) => {
    const plugin = this.plugins.find((p) => p.id === id);

    const newStatus = plugin.getStatus();

    const pluginIdx = this.plugins.findIndex((p) => p.id === id);

    if (pluginIdx !== -1) {
      this.plugins[pluginIdx].status = newStatus;
    }
  };

  setPluginFrame = (frame) => {
    this.pluginFrame = frame;

    this.pluginFrame.contentWindow.Plugins = {};
  };

  setIsInit = (isInit) => {
    this.isInit = isInit;
  };

  initPlugins = async () => {
    const frame = document.createElement("iframe");
    frame.id = "plugin-iframe";
    frame.width = 0;
    frame.height = 0;
    frame.style.display = "none";
    frame.sandbox = "allow-same-origin allow-scripts";

    this.setIsInit(true);

    document.body.appendChild(frame);

    this.setPluginFrame(frame);

    this.updatePlugins();
  };

  updatePlugins = async () => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    this.plugins = [];

    const plugins = await api.plugins.getPlugins(
      !isAdmin && !isOwner ? true : null
    );

    plugins.forEach((plugin) => this.initPlugin(plugin));
  };

  addPlugin = async (data) => {
    const plugin = await api.plugins.addPlugin(data);

    this.initPlugin(plugin);
  };

  initPlugin = (plugin, callback) => {
    const onLoad = async () => {
      const newPlugin = cloneDeep({
        ...plugin,
        ...this.pluginFrame.contentWindow.Plugins[plugin.pluginName],
      });

      newPlugin.createBy = newPlugin.createBy.displayName;
      newPlugin.scopes = newPlugin.scopes.split(",");

      this.installPlugin(newPlugin);

      callback && callback(newPlugin);
    };

    const onError = () => {};

    const frameDoc = this.pluginFrame.contentDocument;

    const script = frameDoc.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", `${plugin.name}-${plugin.id}`);

    if (onLoad) script.onload = onLoad.bind(this);
    if (onError) script.onerror = onError.bind(this);

    script.src = plugin.url;
    script.async = true;

    frameDoc.body.appendChild(script);
  };

  installPlugin = (plugin, addToList = true) => {
    if (addToList) {
      const idx = this.plugins.findIndex((p) => p.id === plugin.id);

      if (idx === -1) {
        this.plugins.push(plugin);
      } else {
        this.plugins[idx] = plugin;
      }
    }

    if (!plugin || !plugin.enabled) return;

    if (plugin.onLoadCallback) {
      plugin.onLoadCallback();
    }

    if (plugin.scopes.includes(PluginScopes.API)) {
      plugin.setAPI(origin, proxy, prefix);
    }

    if (
      plugin.scopes.includes(PluginScopes.ContextMenu) &&
      plugin.contextMenuItems
    ) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.set(key, value);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.InfoPanel) &&
      plugin.infoPanelItems
    ) {
      Array.from(plugin.infoPanelItems).map(([key, value]) => {
        this.infoPanelItems.set(key, value);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.MainButton) &&
      plugin.mainButtonItems
    ) {
      Array.from(plugin.mainButtonItems).map(([key, value]) => {
        this.mainButtonItems.set(key, value);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.ProfileMenu) &&
      plugin.profileMenuItems
    ) {
      Array.from(plugin.profileMenuItems).map(([key, value]) => {
        this.profileMenuItems.set(key, value);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.EventListener) &&
      plugin.eventListenerItems
    ) {
      Array.from(plugin.eventListenerItems).map(([key, value]) => {
        this.eventListenerItems.set(key, value);
      });
    }
  };

  activatePlugin = async (id) => {
    const plugin = this.plugins.find((p) => p.id === id);

    plugin.enabled = true;

    this.installPlugin(plugin, false);
  };

  deactivatePlugin = async (id) => {
    const plugin = this.plugins.find((p) => p.id === id);

    if (!plugin) return;

    plugin.enabled = false;

    if (
      plugin.scopes.includes(PluginScopes.ContextMenu) &&
      plugin.contextMenuItems
    ) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.delete(key);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.InfoPanel) &&
      plugin.infoPanelItems
    ) {
      Array.from(plugin.infoPanelItems).map(([key, value]) => {
        this.infoPanelItems.delete(key);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.ProfileMenu) &&
      plugin.profileMenuItems
    ) {
      Array.from(plugin.profileMenuItems).map(([key, value]) => {
        this.profileMenuItems.delete(key);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.MainButton) &&
      plugin.mainButtonItems
    ) {
      Array.from(plugin.mainButtonItems).map(([key, value]) => {
        this.mainButtonItems.delete(key);
      });
    }

    if (
      plugin.scopes.includes(PluginScopes.EventListener) &&
      plugin.eventListenerItems
    ) {
      Array.from(plugin.eventListenerItems).map(([key, value]) => {
        this.eventListenerItems.delete(key);
      });
    }
  };

  uninstallPlugin = async (id) => {
    this.deactivatePlugin(id);
    const pluginIdx = this.plugins.findIndex((p) => p.id === id);

    if (pluginIdx !== -1) {
      this.plugins.splice(pluginIdx, 1);
    }
    await api.plugins.deletePlugin(id);
  };

  changePluginStatus = async (id, status) => {
    if (status === "true") {
      this.activatePlugin(id);
    } else {
      this.deactivatePlugin(id);
    }

    const plugin = await api.plugins.activatePlugin(id, status === "true");

    return plugin;
  };

  getUserRole = () => {
    const { user } = this.authStore.userStore;

    const { isOwner, isAdmin, isCollaborator, isVisitor } = user;

    const userRole = isOwner
      ? PluginUsersType.owner
      : isAdmin
      ? PluginUsersType.docSpaceAdmin
      : isCollaborator
      ? PluginUsersType.collaborator
      : isVisitor
      ? PluginUsersType.user
      : PluginUsersType.roomAdmin;

    return userRole;
  };

  getContextMenuKeysByType = (type, fileExst) => {
    if (!this.contextMenuItems) return;

    const userRole = this.getUserRole();

    const itemsMap = Array.from(this.contextMenuItems);
    const keys = [];

    switch (type) {
      case PluginFileType.Files:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Files)) {
            if (item.fileExt) {
              if (item.fileExt.includes(fileExst)) {
                if (item.usersType) {
                  if (item.usersType.includes(userRole)) keys.push(item.key);
                } else {
                  keys.push(item.key);
                }
              }
            } else {
              if (item.usersType) {
                if (item.usersType.includes(userRole)) keys.push(item.key);
              } else {
                keys.push(item.key);
              }
            }
          }
        });
        break;
      case PluginFileType.Folders:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Folders)) {
            if (item.usersType) {
              if (item.usersType.includes(userRole)) keys.push(item.key);
            } else {
              keys.push(item.key);
            }
          }
        });
        break;
      case PluginFileType.Rooms:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Rooms)) {
            if (item.usersType) {
              if (item.usersType.includes(userRole)) keys.push(item.key);
            } else {
              keys.push(item.key);
            }
          }
        });
        break;
      default:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType) return;
          if (item.usersType) {
            if (item.usersType.includes(userRole)) keys.push(item.key);
          } else {
            keys.push(item.key);
          }
        });
    }

    if (keys.length === 0) return null;

    return keys;
  };

  get pluginList() {
    return this.plugins;
  }

  get enabledPluginList() {
    return this.plugins.filter((p) => p.enabled);
  }

  get contextMenuItemsList() {
    const items = [];

    this.plugins.forEach((plugin) => {
      if (plugin.scopes.includes(PluginScopes.ContextMenu)) {
        const iconUrl = getPluginUrl(plugin.url, "assets");

        Array.from(plugin.getContextMenuItems(), ([key, value]) => {
          if (value.onClick) {
            const onClick = async (id) => {
              const message = await value.onClick(id);

              messageActions(
                message,
                null,
                null,
                plugin.id,
                this.setSettingsPluginDialogVisible,
                this.setCurrentSettingsDialogPlugin,
                this.updatePluginStatus,
                null,
                this.setPluginDialogVisible,
                this.setPluginDialogProps
              );
            };

            value.onClick = onClick;
          }

          items.push({
            key,
            value: {
              ...value,
              pluginId: plugin.id,
              icon: `${iconUrl}/${value.icon}`,
            },
          });
        });
      }
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get infoPanelItemsList() {
    const items = [];

    const userRole = this.getUserRole();

    this.plugins.forEach((plugin) => {
      if (plugin.scopes.includes(PluginScopes.InfoPanel)) {
        Array.from(plugin.getInfoPanelItems(), ([key, value]) => {
          if (value.submenu.onClick) {
            const onClick = async () => {
              const message = await value.submenu.onClick();

              messageActions(
                message,
                null,
                null,
                plugin.id,
                this.setSettingsPluginDialogVisible,
                this.setCurrentSettingsDialogPlugin,
                this.updatePluginStatus,
                null,
                this.setPluginDialogVisible,
                this.setPluginDialogProps
              );
            };

            value.submenu.onClick = onClick;
          }

          if (value.usersType) {
            if (value.usersType.includes(userRole))
              items.push({ key, value: { ...value, pluginId: plugin.id } });
          } else {
            items.push({ key, value: { ...value, pluginId: plugin.id } });
          }
        });
      }
    });

    return items;
  }

  get profileMenuItemsList() {
    const items = [];

    const userRole = this.getUserRole();

    this.plugins.forEach((plugin) => {
      if (plugin.scopes.includes(PluginScopes.ProfileMenu)) {
        const iconUrl = getPluginUrl(plugin.url, "assets");

        Array.from(plugin.getProfileMenuItems(), ([key, value]) => {
          if (value.onClick) {
            const onClick = async () => {
              const message = await value.onClick();

              messageActions(
                message,
                null,
                null,
                plugin.id,
                this.setSettingsPluginDialogVisible,
                this.setCurrentSettingsDialogPlugin,
                this.updatePluginStatus,
                null,
                this.setPluginDialogVisible,
                this.setPluginDialogProps
              );
            };

            value.onClick = onClick;
          }

          if (value.usersType) {
            if (value.usersType.includes(userRole)) {
              items.push({
                key,
                value: {
                  ...value,
                  pluginId: plugin.id,
                  icon: `${iconUrl}/${value.icon}`,
                },
              });
            }
          } else {
            items.push({
              key,
              value: {
                ...value,
                pluginId: plugin.id,
                icon: `${iconUrl}/${value.icon}`,
              },
            });
          }
        });
      }
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get mainButtonItemsList() {
    const items = [];

    const userRole = this.getUserRole();

    this.plugins.forEach((plugin) => {
      if (plugin.scopes.includes(PluginScopes.MainButton)) {
        const iconUrl = getPluginUrl(plugin.url, "assets");

        Array.from(plugin.getMainButtonItems(), ([key, value]) => {
          if (value.items) {
            const newItems = [];

            value.items.forEach((i) => {
              const onClick = async () => {
                const message = await i.onClick(this.selectedFolderStore.id);

                messageActions(
                  message,
                  null,
                  null,
                  plugin.id,
                  this.setSettingsPluginDialogVisible,
                  this.setCurrentSettingsDialogPlugin,
                  this.updatePluginStatus,
                  null,
                  this.setPluginDialogVisible,
                  this.setPluginDialogProps
                );
              };

              newItems.push({
                ...i,
                onClick,
                icon: `${iconUrl}/${i.icon}`,
              });
            });

            value.items = newItems;
          }

          if (value.onClick) {
            const onClick = async () => {
              const message = await option.value.onClick(
                this.selectedFolderStore.id
              );

              messageActions(
                message,
                null,
                null,
                plugin.id,
                this.setSettingsPluginDialogVisible,
                this.setCurrentSettingsDialogPlugin,
                this.updatePluginStatus,
                null,
                this.setPluginDialogVisible,
                this.setPluginDialogProps
              );
            };

            value.onClick = onClick;
          }

          if (value.usersType) {
            if (value.usersType.includes(userRole)) {
              items.push({
                key,
                value: {
                  ...value,
                  pluginId: plugin.id,
                  icon: `${iconUrl}/${value.icon}`,
                  iconUrl,
                },
              });
            }
          } else {
            items.push({
              key,
              value: {
                ...value,
                pluginId: plugin.id,
                icon: `${iconUrl}/${value.icon}`,
              },
            });
          }
        });
      }
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }
}

export default PluginStore;
