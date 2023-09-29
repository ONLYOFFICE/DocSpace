import { makeAutoObservable } from "mobx";
import { cloneDeep } from "lodash";
import { isMobileOnly, isTablet } from "react-device-detect";

import api from "@docspace/common/api";

import defaultConfig from "PUBLIC_DIR/scripts/config.json";

import {
  PluginFileType,
  PluginScopes,
  PluginUsersType,
  PluginDevices,
  PluginStatus,
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
  fileItems = null;

  pluginFrame = null;

  isInit = false;

  settingsPluginDialogVisible = false;
  currentSettingsDialogPlugin = null;

  pluginDialogVisible = false;
  pluginDialogProps = null;

  deletePluginDialogVisible = false;
  deletePluginDialogProps = null;

  constructor(authStore, selectedFolderStore) {
    this.authStore = authStore;
    this.selectedFolderStore = selectedFolderStore;

    this.plugins = [];

    this.contextMenuItems = new Map();
    this.infoPanelItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();
    this.eventListenerItems = new Map();
    this.fileItems = new Map();

    makeAutoObservable(this);
  }

  setCurrentSettingsDialogPlugin = (value) => {
    this.currentSettingsDialogPlugin = value;
  };

  setSettingsPluginDialogVisible = (value) => {
    this.settingsPluginDialogVisible = value;
  };

  setPluginDialogVisible = (value) => {
    this.pluginDialogVisible = value;
  };

  setPluginDialogProps = (value) => {
    this.pluginDialogProps = value;
  };

  setDeletePluginDialogVisible = (value) => {
    this.deletePluginDialogVisible = value;
  };

  setDeletePluginDialogProps = (value) => {
    this.deletePluginDialogProps = value;
  };

  updatePluginStatus = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    const newStatus = plugin.getStatus();

    const pluginIdx = system
      ? this.plugins.findIndex((p) => p.name === name)
      : this.plugins.findIndex((p) => p.id === id);

    if (pluginIdx !== -1) {
      if (this.plugins[pluginIdx].status === newStatus) return;

      this.plugins[pluginIdx].status = newStatus;

      if (
        newStatus === PluginStatus.active &&
        this.plugins[pluginIdx].enabled
      ) {
        if (this.plugins[pluginIdx].scopes.includes(PluginScopes.ContextMenu)) {
          this.updateContextMenuItems(id, name, system);
        }

        if (this.plugins[pluginIdx].scopes.includes(PluginScopes.InfoPanel)) {
          this.updateInfoPanelItems(id, name, system);
        }

        if (this.plugins[pluginIdx].scopes.includes(PluginScopes.MainButton)) {
          this.updateMainButtonItems(id, name, system);
        }

        if (this.plugins[pluginIdx].scopes.includes(PluginScopes.ProfileMenu)) {
          this.updateProfileMenuItems(id, name, system);
        }

        if (
          this.plugins[pluginIdx].scopes.includes(PluginScopes.EventListener)
        ) {
          this.updateEventListenerItems(id, name, system);
        }

        if (this.plugins[pluginIdx].scopes.includes(PluginScopes.File)) {
          this.updateFileItems(id, name, system);
        }
      }
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

    document.body.appendChild(frame);

    this.setPluginFrame(frame);

    this.updatePlugins();

    this.setIsInit(true);
  };

  updatePlugins = async () => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    try {
      this.plugins = [];

      const plugins = await api.plugins.getPlugins(
        !isAdmin && !isOwner ? true : null
      );

      plugins.forEach((plugin) => this.initPlugin(plugin));
    } catch (e) {
      console.log(e);
    }
  };

  addPlugin = async (data) => {
    try {
      const plugin = await api.plugins.addPlugin(data);

      this.initPlugin(plugin);
    } catch (e) {
      console.log(e);
    }
  };

  uninstallPlugin = async (id, name, system) => {
    const pluginIdx = system
      ? this.plugins.findIndex((p) => p.name === name)
      : this.plugins.findIndex((p) => p.id === id);

    try {
      if (system) {
        await api.plugins.deleteSystemPlugin(name);
      } else {
        await api.plugins.deletePlugin(id);
      }

      this.deactivatePlugin(id, name, system);

      if (pluginIdx !== -1) {
        this.plugins.splice(pluginIdx, 1);
      }
    } catch (e) {
      console.log(e);
    }
  };

  initPlugin = (plugin, callback) => {
    const onLoad = async () => {
      const newPlugin = cloneDeep({
        ...plugin,
        ...this.pluginFrame.contentWindow.Plugins[plugin.pluginName],
      });

      newPlugin.createBy = newPlugin.createBy.displayName;
      newPlugin.scopes = newPlugin.scopes.split(",");
      newPlugin.iconUrl = getPluginUrl(newPlugin.url, "");

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

  installPlugin = async (plugin, addToList = true) => {
    const { system } = plugin;

    if (addToList) {
      const idx = system
        ? this.plugins.findIndex((p) => p.name === plugin.name)
        : this.plugins.findIndex((p) => p.id === plugin.id);

      if (idx === -1) {
        this.plugins.unshift(plugin);
      } else {
        this.plugins[idx] = plugin;
      }
    }

    if (!plugin || !plugin.enabled) return;

    if (plugin.scopes.includes(PluginScopes.API)) {
      plugin.setAPI && plugin.setAPI(origin, proxy, prefix);
    }

    if (plugin.onLoadCallback) {
      await plugin.onLoadCallback();

      this.updatePluginStatus(plugin.id, plugin.name, system);
    }

    if (plugin.status === PluginStatus.hide) return;

    if (plugin.scopes.includes(PluginScopes.ContextMenu)) {
      this.updateContextMenuItems(plugin.id, plugin.name, system);
    }

    if (plugin.scopes.includes(PluginScopes.InfoPanel)) {
      this.updateInfoPanelItems(plugin.id, plugin.name, system);
    }

    if (plugin.scopes.includes(PluginScopes.MainButton)) {
      this.updateMainButtonItems(plugin.id, plugin.name, system);
    }

    if (plugin.scopes.includes(PluginScopes.ProfileMenu)) {
      this.updateProfileMenuItems(plugin.id, plugin.name, system);
    }

    if (plugin.scopes.includes(PluginScopes.EventListener)) {
      this.updateEventListenerItems(plugin.id, plugin.name, system);
    }

    if (plugin.scopes.includes(PluginScopes.File)) {
      this.updateFileItems(plugin.id, plugin.name, system);
    }
  };

  changePluginStatus = async (id, name, status, system) => {
    try {
      const plugin = system
        ? await api.plugins.activateSystemPlugin(name, status)
        : await api.plugins.activatePlugin(id, status);

      if (status) {
        this.activatePlugin(id, name, system);
      } else {
        this.deactivatePlugin(id, name, system);
      }

      return plugin;
    } catch (e) {}
  };

  activatePlugin = async (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin) return;

    plugin.enabled = true;

    this.installPlugin(plugin, false);
  };

  deactivatePlugin = async (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin) return;

    plugin.enabled = false;

    if (plugin.scopes.includes(PluginScopes.ContextMenu)) {
      this.deactivateContextMenuItems(plugin);
    }

    if (plugin.scopes.includes(PluginScopes.InfoPanel)) {
      this.deactivateInfoPanelItems(plugin);
    }

    if (plugin.scopes.includes(PluginScopes.ProfileMenu)) {
      this.deactivateProfileMenuItems(plugin);
    }

    if (plugin.scopes.includes(PluginScopes.MainButton)) {
      this.deactivateMainButtonItems(plugin);
    }

    if (plugin.scopes.includes(PluginScopes.EventListener)) {
      this.deactivateEventListenerItems(plugin);
    }

    if (plugin.scopes.includes(PluginScopes.File)) {
      this.deactivateFileItems(plugin);
    }
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

  getCurrentDevice = () => {
    const device = isTablet
      ? PluginDevices.tablet
      : isMobileOnly
      ? PluginDevices.mobile
      : PluginDevices.desktop;

    return device;
  };

  getContextMenuKeysByType = (type, fileExst) => {
    if (!this.contextMenuItems) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    const itemsMap = Array.from(this.contextMenuItems);
    const keys = [];

    switch (type) {
      case PluginFileType.Files:
        itemsMap.forEach(([key, item]) => {
          if (!item.fileType) return;

          if (item.fileType.includes(PluginFileType.Files)) {
            const correctFileExt = item.fileExt
              ? item.fileExt.includes(fileExst)
              : true;

            const correctUserType = item.usersType
              ? item.usersType.includes(userRole)
              : true;

            const correctDevice = item.devices
              ? item.devices.includes(device)
              : true;

            if (correctFileExt && correctUserType && correctDevice)
              keys.push(item.key);
          }
        });
        break;
      case PluginFileType.Folders:
        itemsMap.forEach(([key, item]) => {
          if (!item.fileType) return;

          if (item.fileType.includes(PluginFileType.Folders)) {
            const correctUserType = item.usersType
              ? item.usersType.includes(userRole)
              : true;

            const correctDevice = item.devices
              ? item.devices.includes(device)
              : true;

            if (correctUserType && correctDevice) keys.push(item.key);
          }
        });
        break;
      case PluginFileType.Rooms:
        itemsMap.forEach(([key, item]) => {
          if (!item.fileType) return;

          if (item.fileType.includes(PluginFileType.Rooms)) {
            const correctUserType = item.usersType
              ? item.usersType.includes(userRole)
              : true;

            const correctDevice = item.devices
              ? item.devices.includes(device)
              : true;

            if (correctUserType && correctDevice) keys.push(item.key);
          }
        });
        break;
      case PluginFileType.Image:
        itemsMap.forEach(([key, item]) => {
          if (!item.fileType) return;

          if (item.fileType.includes(PluginFileType.Image)) {
            const correctFileExt = item.fileExt
              ? item.fileExt.includes(fileExst)
              : true;

            const correctUserType = item.usersType
              ? item.usersType.includes(userRole)
              : true;

            const correctDevice = item.devices
              ? item.devices.includes(device)
              : true;

            if (correctUserType && correctDevice && correctFileExt)
              keys.push(item.key);
          }
        });
        break;
      case PluginFileType.Video:
        itemsMap.forEach(([key, item]) => {
          if (!item.fileType) return;

          if (item.fileType.includes(PluginFileType.Video)) {
            const correctUserType = item.usersType
              ? item.usersType.includes(userRole)
              : true;

            const correctDevice = item.devices
              ? item.devices.includes(device)
              : true;

            if (correctUserType && correctDevice) keys.push(item.key);
          }
        });
        break;
      default:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType) return;

          const correctUserType = item.usersType
            ? item.usersType.includes(userRole)
            : true;

          const correctDevice = item.devices
            ? item.devices.includes(device)
            : true;

          if (correctUserType && correctDevice) keys.push(item.key);
        });
    }

    if (keys.length === 0) return null;

    return keys;
  };

  updateContextMenuItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items = plugin.getContextMenuItems && plugin.getContextMenuItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      const onClick = async (fileId) => {
        if (!value.onClick) return;

        const message = await value.onClick(fileId);

        messageActions(
          message,
          null,

          plugin.id,
          plugin.name,
          plugin.system,

          this.setSettingsPluginDialogVisible,
          this.setCurrentSettingsDialogPlugin,
          this.updatePluginStatus,
          null,
          this.setPluginDialogVisible,
          this.setPluginDialogProps,

          this.updateContextMenuItems,
          this.updateInfoPanelItems,
          this.updateMainButtonItems,
          this.updateProfileMenuItems,
          this.updateEventListenerItems,
          this.updateFileItems
        );
      };

      this.contextMenuItems.set(key, {
        ...value,
        onClick,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
        icon: `${plugin.iconUrl}/assets/${value.icon}`,
      });
    });
  };

  deactivateContextMenuItems = (plugin) => {
    if (!plugin) return;

    const items = plugin.getContextMenuItems && plugin.getContextMenuItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.contextMenuItems.delete(key);
    });
  };

  updateInfoPanelItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items = plugin.getInfoPanelItems && plugin.getInfoPanelItems();

    if (!items) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    Array.from(items).map(([key, value]) => {
      const correctUserType = value.usersType
        ? value.usersType.includes(userRole)
        : true;

      const correctDevice = value.devices
        ? value.devices.includes(device)
        : true;

      if (!correctUserType || !correctDevice) return;

      const submenu = { ...value.subMenu };

      if (value.subMenu.onClick) {
        const onClick = async () => {
          const message = await value.subMenu.onClick();

          messageActions(
            message,
            null,

            plugin.id,
            plugin.name,
            plugin.system,

            this.setSettingsPluginDialogVisible,
            this.setCurrentSettingsDialogPlugin,
            this.updatePluginStatus,
            null,
            this.setPluginDialogVisible,
            this.setPluginDialogProps,

            this.updateContextMenuItems,
            this.updateInfoPanelItems,
            this.updateMainButtonItems,
            this.updateProfileMenuItems,
            this.updateEventListenerItems,
            this.updateFileItems
          );
        };

        submenu.onClick = onClick;
      }

      this.infoPanelItems.set(key, {
        ...value,
        subMenu: submenu,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
      });
    });
  };

  deactivateInfoPanelItems = (plugin) => {
    if (!plugin) return;

    const items = plugin.getInfoPanelItems && plugin.getInfoPanelItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.infoPanelItems.delete(key);
    });
  };

  updateMainButtonItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items = plugin.getMainButtonItems && plugin.getMainButtonItems();

    if (!items) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    Array.from(items).map(([key, value]) => {
      const correctUserType = value.usersType
        ? value.usersType.includes(userRole)
        : true;

      const correctDevice = value.devices
        ? value.devices.includes(device)
        : true;

      if (!correctUserType || !correctDevice) return;

      const newItems = [];
      if (value.items) {
        value.items.forEach((i) => {
          const onClick = async () => {
            const message = await i.onClick(this.selectedFolderStore.id);

            messageActions(
              message,
              null,

              plugin.id,
              plugin.name,
              plugin.system,

              this.setSettingsPluginDialogVisible,
              this.setCurrentSettingsDialogPlugin,
              this.updatePluginStatus,
              null,
              this.setPluginDialogVisible,
              this.setPluginDialogProps,

              this.updateContextMenuItems,
              this.updateInfoPanelItems,
              this.updateMainButtonItems,
              this.updateProfileMenuItems,
              this.updateEventListenerItems,
              this.updateFileItems
            );
          };

          newItems.push({
            ...i,
            onClick,
            icon: `${plugin.iconUrl}/assets/${i.icon}`,
          });
        });
      }

      const onClick = async () => {
        if (!value.onClick) return;

        const message = await value.onClick(this.selectedFolderStore.id);

        messageActions(
          message,
          null,

          plugin.id,
          plugin.name,
          plugin.system,

          this.setSettingsPluginDialogVisible,
          this.setCurrentSettingsDialogPlugin,
          this.updatePluginStatus,
          null,
          this.setPluginDialogVisible,
          this.setPluginDialogProps,

          this.updateContextMenuItems,
          this.updateInfoPanelItems,
          this.updateMainButtonItems,
          this.updateProfileMenuItems,
          this.updateEventListenerItems,
          this.updateFileItems
        );
      };

      this.mainButtonItems.set(key, {
        ...value,
        onClick,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
        icon: `${plugin.iconUrl}/assets/${value.icon}`,
        items: newItems.length > 0 ? newItems : null,
      });
    });
  };

  deactivateMainButtonItems = (plugin) => {
    if (!plugin) return;

    const items = plugin.getMainButtonItems && plugin.getMainButtonItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.mainButtonItems.delete(key);
    });
  };

  updateProfileMenuItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items = plugin.getProfileMenuItems && plugin.getProfileMenuItems();

    if (!items) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    Array.from(items).map(([key, value]) => {
      const correctUserType = value.usersType
        ? value.usersType.includes(userRole)
        : true;

      const correctDevice = value.devices
        ? value.devices.includes(device)
        : true;

      if (!correctUserType || !correctDevice) return;

      const onClick = async () => {
        if (!value.onClick) return;

        const message = await value.onClick();

        messageActions(
          message,
          null,

          plugin.id,
          plugin.name,
          plugin.system,

          this.setSettingsPluginDialogVisible,
          this.setCurrentSettingsDialogPlugin,
          this.updatePluginStatus,
          null,
          this.setPluginDialogVisible,
          this.setPluginDialogProps,

          this.updateContextMenuItems,
          this.updateInfoPanelItems,
          this.updateMainButtonItems,
          this.updateProfileMenuItems,
          this.updateEventListenerItems,
          this.updateFileItems
        );
      };

      this.profileMenuItems.set(key, {
        ...value,
        onClick,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
        icon: `${plugin.iconUrl}/assets/${value.icon}`,
      });
    });
  };

  deactivateProfileMenuItems = (plugin) => {
    if (!plugin) return;

    const items = plugin.getProfileMenuItems && plugin.getProfileMenuItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.profileMenuItems.delete(key);
    });
  };

  updateEventListenerItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items =
      plugin.getEventListenerItems && plugin.getEventListenerItems();

    if (!items) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    Array.from(items).map(([key, value]) => {
      const correctUserType = value.usersType
        ? value.usersType.includes(userRole)
        : true;

      const correctDevice = value.devices
        ? value.devices.includes(device)
        : true;

      if (!correctUserType || !correctDevice) return;
      const eventHandler = async (e) => {
        if (!value.eventHandler) return;

        const message = await value.eventHandler(e);

        messageActions(
          message,
          null,

          plugin.id,
          plugin.name,
          plugin.system,

          this.setSettingsPluginDialogVisible,
          this.setCurrentSettingsDialogPlugin,
          this.updatePluginStatus,
          null,
          this.setPluginDialogVisible,
          this.setPluginDialogProps,

          this.updateContextMenuItems,
          this.updateInfoPanelItems,
          this.updateMainButtonItems,
          this.updateProfileMenuItems,
          this.updateEventListenerItems,
          this.updateFileItems
        );
      };

      this.eventListenerItems.set(key, {
        ...value,
        eventHandler,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
      });
    });
  };

  deactivateEventListenerItems = (plugin) => {
    if (!plugin) return;

    const items =
      plugin.getEventListenerItems && plugin.getEventListenerItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.eventListenerItems.delete(key);
    });
  };

  updateFileItems = (id, name, system) => {
    const plugin = system
      ? this.plugins.find((p) => p.name === name)
      : this.plugins.find((p) => p.id === id);

    if (!plugin || !plugin.enabled) return;

    const items = plugin.getFileItems && plugin.getFileItems();

    if (!items) return;

    const userRole = this.getUserRole();
    const device = this.getCurrentDevice();

    Array.from(items).map(([key, value]) => {
      const correctUserType = value.usersType
        ? value.usersType.includes(userRole)
        : true;

      const correctDevice = value.devices
        ? value.devices.includes(device)
        : true;

      if (!correctUserType || !correctDevice) return;

      const fileIcon = `${plugin.iconUrl}/assets/${value.fileIcon}`;

      const onClick = async (item) => {
        if (!value.onClick) return;

        const message = await value.onClick(item);

        messageActions(
          message,
          null,

          plugin.id,
          plugin.name,
          plugin.system,

          this.setSettingsPluginDialogVisible,
          this.setCurrentSettingsDialogPlugin,
          this.updatePluginStatus,
          null,
          this.setPluginDialogVisible,
          this.setPluginDialogProps,

          this.updateContextMenuItems,
          this.updateInfoPanelItems,
          this.updateMainButtonItems,
          this.updateProfileMenuItems,
          this.updateEventListenerItems,
          this.updateFileItems
        );
      };

      this.fileItems.set(key, {
        ...value,
        onClick,
        fileIcon,
        pluginId: plugin.id,
        pluginName: plugin.name,
        pluginSystem: plugin.system,
      });
    });
  };

  deactivateFileItems = (plugin) => {
    if (!plugin) return;

    const items = plugin.getFileItems && plugin.getFileItems();

    if (!items) return;

    Array.from(items).map(([key, value]) => {
      this.fileItems.delete(key);
    });
  };

  get pluginList() {
    return this.plugins;
  }

  get enabledPluginList() {
    return this.plugins.filter((p) => p.enabled);
  }

  get contextMenuItemsList() {
    const items = [];

    Array.from(this.contextMenuItems, ([key, value]) => {
      items.push({ key, value: { ...value } });
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get infoPanelItemsList() {
    const items = [];

    Array.from(this.infoPanelItems, ([key, value]) => {
      items.push({ key, value: { ...value } });
    });

    return items;
  }

  get profileMenuItemsList() {
    const items = [];

    Array.from(this.profileMenuItems, ([key, value]) => {
      items.push({
        key,
        value: {
          ...value,
        },
      });
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get mainButtonItemsList() {
    const items = [];

    Array.from(this.mainButtonItems, ([key, value]) => {
      items.push({
        key,
        value: {
          ...value,
        },
      });
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get eventListenerItemsList() {
    const items = [];

    Array.from(this.eventListenerItems, ([key, value]) => {
      items.push({
        key,
        value: {
          ...value,
        },
      });
    });

    if (items.length > 0) {
      return items;
    }

    return null;
  }

  get fileItemsList() {
    const items = [];

    Array.from(this.fileItems, ([key, value]) => {
      items.push({
        key,
        value: {
          ...value,
        },
      });
    });

    if (items.length > 0) {
      return items;
    }

    return null;
  }
}

export default PluginStore;
