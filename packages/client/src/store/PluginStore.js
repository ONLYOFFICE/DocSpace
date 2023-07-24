import { makeAutoObservable } from "mobx";
import { cloneDeep } from "lodash";

import api from "@docspace/common/api";

import defaultConfig from "PUBLIC_DIR/scripts/config.json";

import {
  PluginFileType,
  PluginScopes,
} from "SRC_DIR/helpers/plugins/constants";

let { api: apiConf, proxy: proxyConf } = defaultConfig;
let { orign: apiOrigin, prefix: apiPrefix } = apiConf;
let { url: proxyURL } = proxyConf;

const origin =
  window.DocSpaceConfig?.api?.origin || apiOrigin || window.location.origin;
const proxy = window.DocSpaceConfig?.proxy?.url || proxyURL;
const prefix = window.DocSpaceConfig?.api?.prefix || apiPrefix;

class PluginStore {
  authStore = null;

  plugins = null;

  contextMenuItems = null;
  infoPanelItems = null;
  mainButtonItems = null;
  profileMenuItems = null;

  pluginFrame = null;

  isInit = false;

  settingsPluginDialogVisible = false;
  currentSettingsDialogPlugin = null;
  isAdminSettingsDialog = false;

  constructor(authStore) {
    this.authStore = authStore;

    this.plugins = [];

    this.contextMenuItems = new Map();
    this.infoPanelItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();

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

      const { displayName } = await api.people.getUserById(plugin.createBy);

      newPlugin.createBy = displayName;
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

    if (plugin.enabled && plugin.onLoadCallback) {
      plugin.onLoadCallback();
    }

    if (plugin.scopes.includes(PluginScopes.API) && plugin.enabled) {
      plugin.setAPI(origin, proxy, prefix);
    }

    if (
      plugin &&
      plugin.scopes.includes(PluginScopes.ContextMenu) &&
      plugin.contextMenuItems &&
      plugin.enabled
    ) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.set(key, value);
      });
    }

    if (
      plugin &&
      plugin.scopes.includes(PluginScopes.InfoPanel) &&
      plugin.infoPanelItems &&
      plugin.enabled
    ) {
      Array.from(plugin.infoPanelItems).map(([key, value]) => {
        this.infoPanelItems.set(key, value);
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

    plugin.enabled = false;

    if (
      plugin &&
      plugin.scopes.includes(PluginScopes.ContextMenu) &&
      plugin.contextMenuItems
    ) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.delete(key);
      });
    }

    if (
      plugin &&
      plugin.scopes.includes(PluginScopes.InfoPanel) &&
      plugin.infoPanelItems
    ) {
      Array.from(plugin.infoPanelItems).map(([key, value]) => {
        this.infoPanelItems.delete(key);
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

  getContextMenuKeysByType = (type, fileExst) => {
    if (!this.contextMenuItems) return;

    const itemsMap = Array.from(this.contextMenuItems);
    const keys = [];

    switch (type) {
      case PluginFileType.Files:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Files)) {
            if (item.fileExt.includes(fileExst)) {
              keys.push(item.key);
            }
          }
        });
      case PluginFileType.Folders:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Folders)) {
            keys.push(item.key);
          }
        });
        break;
      case PluginFileType.Rooms:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType.includes(PluginFileType.Rooms)) {
            keys.push(item.key);
          }
        });
        break;
      default:
        itemsMap.forEach(([key, item]) => {
          if (item.fileType === PluginFileType.All) {
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
        Array.from(plugin.getContextMenuItems(), ([key, value]) =>
          items.push({ key, value: { ...value, pluginId: plugin.id } })
        );
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

    this.plugins.forEach((plugin) => {
      if (plugin.scopes.includes(PluginScopes.InfoPanel)) {
        Array.from(plugin.getInfoPanelItems(), ([key, value]) =>
          items.push({ key, value: { ...value, pluginId: plugin.id } })
        );
      }
    });

    return items;
  }
}

export default PluginStore;
