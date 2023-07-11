import { makeAutoObservable } from "mobx";
import { cloneDeep } from "lodash";

import api from "@docspace/common/api";

import defaultConfig from "PUBLIC_DIR/scripts/config.json";

import {
  PluginType,
  PluginContextMenuItemType,
} from "SRC_DIR/helpers/plugins/constants";

let { api: apiConf, proxy: proxyConf } = defaultConfig;
let { orign: apiOrigin, prefix: apiPrefix, timeout: apiTimeout } = apiConf;
let { url: proxyURL } = proxyConf;

const origin =
  window.DocSpaceConfig?.api?.origin || apiOrigin || window.location.origin;
const proxy = window.DocSpaceConfig?.proxy?.url || proxyURL;
const prefix = window.DocSpaceConfig?.api?.prefix || apiPrefix;

class PluginStore {
  plugins = null;

  contextMenuItems = null;
  mainButtonItems = null;
  profileMenuItems = null;

  pluginFrame = null;

  isInit = false;

  settingsPluginDialogVisible = false;
  currentSettingsDialogPlugin = null;
  isAdminSettingsDialog = false;

  constructor() {
    this.plugins = [];

    this.contextMenuItems = new Map();
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

  setPluginFrame = (frame) => {
    this.pluginFrame = frame;

    this.pluginFrame.contentWindow.Plugins = {};
  };

  setIsInit = (isInit) => {
    this.isInit = isInit;
  };

  updatePlugins = async () => {
    this.plugins = [];

    const plugins = await api.plugins.getPlugins();

    plugins.forEach((plugin) => this.initPlugin(plugin));
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

  initPlugin = (plugin, callback) => {
    const onLoad = () => {
      const newPlugin = cloneDeep({
        ...plugin,
        ...this.pluginFrame.contentWindow.Plugins[plugin.plugin.split(".")[0]],
      });

      if (newPlugin.apiScope) {
        newPlugin.setAPI(origin, proxy, prefix);
      }

      this.installPlugin(newPlugin);

      newPlugin.onLoadCallback();

      callback && callback(newPlugin);
    };

    const onError = () => {};

    const frameDoc = this.pluginFrame.contentDocument;

    const script = frameDoc.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", `${plugin.name}-${plugin.id}`);

    if (onLoad) script.onload = onLoad.bind(this);
    if (onError) script.onerror = onError.bind(this);

    script.src = `/static/plugins/${plugin.plugin}`;
    script.async = true;

    frameDoc.body.appendChild(script);
  };

  installPlugin = (plugin) => {
    this.plugins.push(plugin);

    if (plugin && plugin.contextMenuItems && plugin.isActive) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.set(key, value);
      });
    }
  };

  uninstallPlugin = async (id) => {
    this.deactivatePlugin(id);
    const pluginIdx = this.plugins.findIndex((p) => p.id === id);
    if (pluginIdx > 0) {
    }
    await api.plugins.deletePlugin(id);
  };

  getInstalledPlugins = () => {
    return this.plugins;
  };

  activatePlugin = async (id) => {
    const plugin = this.plugins.find((p) => p.id === id);

    plugin.isActive = true;

    if (plugin && plugin.contextMenuItems) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.set(key, value);
      });
    }
  };

  deactivatePlugin = async (id) => {
    const plugin = this.plugins.find((p) => p.id === id);

    plugin.isActive = false;

    if (plugin && plugin.contextMenuItems) {
      Array.from(plugin.contextMenuItems).map(([key, value]) => {
        this.contextMenuItems.delete(key);
      });
    }
  };

  changePluginStatus = async (id, status) => {
    if (status === "true") {
      this.activatePlugin(id);
    } else {
      this.deactivatePlugin(id);
    }

    const plugin = await api.plugins.activatePlugin(id);

    return plugin;
  };

  getContextMenuKeysByType = (type, fileExst) => {
    if (!this.contextMenuItems) return;

    const itemsMap = Array.from(this.contextMenuItems);
    const keys = [];

    switch (type) {
      case PluginContextMenuItemType.Files:
        itemsMap.forEach(([key, item]) => {
          if (item.type === PluginContextMenuItemType.Files) {
            if (item.fileExt === "all" || item.fileExt.includes(fileExst)) {
              keys.push(item.key);
            }
          }
        });
        break;
      case PluginContextMenuItemType.Folders:
        itemsMap.forEach(([key, item]) => {
          if (item.type === PluginContextMenuItemType.Folders) {
            keys.push(item.key);
          }
        });
        break;
      case PluginContextMenuItemType.Rooms:
        itemsMap.forEach(([key, item]) => {
          if (item.type === PluginContextMenuItemType.Rooms) {
            keys.push(item.key);
          }
        });
        break;
      case PluginContextMenuItemType.All:
        itemsMap.forEach(([key, item]) => {
          if (item.type === PluginContextMenuItemType.All) {
            keys.push(item.key);
          }
        });
        break;
      default:
        return null;
    }

    if (keys.length === 0) return null;

    return keys;
  };

  get pluginList() {
    return this.plugins;
  }

  get contextMenuItemsList() {
    const items = [];

    this.plugins.forEach((plugin) => {
      Array.from(plugin.getContextMenuItems(), ([key, value]) =>
        items.push({ key, value: { ...value, pluginId: plugin.id } })
      );
    });

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }
}

export default PluginStore;
