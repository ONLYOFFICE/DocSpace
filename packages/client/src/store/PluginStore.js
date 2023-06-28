import { makeAutoObservable } from "mobx";

import api from "@docspace/common/api";

import {
  PluginType,
  PluginContextMenuItemType,
} from "SRC_DIR/helpers/plugins/constants";
import { wrapPluginItem } from "SRC_DIR/helpers/plugins/utils";

class PluginStore {
  plugins = null;

  contextMenuItems = null;
  mainButtonItems = null;
  profileMenuItems = null;

  pluginFrame = null;

  isInit = false;

  constructor() {
    this.plugins = new Map();

    this.contextMenuItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();

    makeAutoObservable(this);
  }

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

    const plugins = await api.plugins.getPlugins();

    plugins.forEach((plugin) => this.initPlugin(plugin));
  };

  initPlugin = (plugin, callback) => {
    const onLoad = () => {
      const newPlugin = this.pluginFrame.contentWindow.Plugins[plugin.name];

      newPlugin.isActive = plugin.isActive;
      newPlugin.id = plugin.id;
      newPlugin.name = plugin.name;

      this.installPlugin(plugin.id, newPlugin);
      callback && callback(newPlugin);
    };

    const onError = () => {};

    const frameDoc = this.pluginFrame.contentDocument;

    const script = frameDoc.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", `${plugin.name}-${plugin.id}`);

    if (onLoad) script.onload = onLoad.bind(this);
    if (onError) script.onerror = onError.bind(this);

    script.src = `/static/plugins/${plugin.filename}`;
    script.async = true;

    frameDoc.body.appendChild(script);
  };

  installPlugin = (id, plugin) => {
    this.plugins.set(+id, plugin);

    if (plugin.isActive) {
      this.activatePlugin(+id);
    }
  };

  uninstallPlugin = async (id) => {
    this.deactivatePlugin(id);
    this.plugins.delete(+id);
    await api.plugins.deletePlugin(id);
  };

  getInstalledPlugins = () => {
    return this.plugins;
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

  activatePlugin = (id) => {
    this.plugins.get(+id).isActive = true;
    const pluginItems = this.plugins.get(+id).activate();

    if (!pluginItems) return;

    Array.from(pluginItems, ([key, value]) => {
      switch (key) {
        case PluginType.CONTEXT_MENU:
          Array.from(value, ([key, item]) => {
            const newItem = wrapPluginItem(
              this.plugins.get(+id).name,
              key,
              item,
              PluginType.CONTEXT_MENU,
              this.pluginFrame
            );

            this.addContextMenuItem(key, newItem);
          });
          break;

        case PluginType.ACTION_BUTTON:
          Array.from(value, ([key, item]) => {
            const newItem = wrapPluginItem(
              this.plugins.get(+id).name,
              key,
              item,
              PluginType.ACTION_BUTTON,
              this.pluginFrame
            );

            this.addMainButtonItem(key, newItem);
          });
          break;

        case PluginType.PROFILE_MENU:
          Array.from(value, ([key, item]) => {
            const newItem = wrapPluginItem(
              this.plugins.get(+id).name,
              key,
              item,
              PluginType.PROFILE_MENU,
              this.pluginFrame
            );
            this.addProfileMenuItem(key, newItem);
          });
          break;
      }
    });
  };

  deactivatePlugin = (id) => {
    this.plugins.get(+id).isActive = false;
    const pluginItems = this.plugins.get(+id).deactivate();

    if (!pluginItems) return;

    Array.from(pluginItems, ([key, value]) => {
      switch (key) {
        case PluginType.CONTEXT_MENU:
          value.map((key) => this.deleteContextMenuItem(key));
          break;
        case PluginType.ACTION_BUTTON:
          value.map((key) => this.deleteMainButtonItem(key));
          break;
        case PluginType.PROFILE_MENU:
          value.map((key) => this.deleteProfileMenuItem(key));
          break;
      }
    });
  };

  addContextMenuItem = (key, contextMenuItem) => {
    this.contextMenuItems.set(key, contextMenuItem);
  };

  deleteContextMenuItem = (key) => {
    this.contextMenuItems.delete(key);
  };

  getContextMenuKeysByType = (type) => {
    if (!this.contextMenuItemsList) return;

    const itemsMap = Array.from(this.contextMenuItemsList);
    const keys = [];

    switch (type) {
      case PluginContextMenuItemType.Files:
        itemsMap.forEach((item) => {
          if (item.value.type === PluginContextMenuItemType.Files) {
            keys.push(item.key);
          }
        });
        break;
      case PluginContextMenuItemType.Folders:
        itemsMap.forEach((item) => {
          if (item.value.type === PluginContextMenuItemType.Folders) {
            keys.push(item.key);
          }
        });
        break;
      case PluginContextMenuItemType.Rooms:
        itemsMap.forEach((item) => {
          if (item.value.type === PluginContextMenuItemType.Rooms) {
            keys.push(item.key);
          }
        });
        break;
      case PluginContextMenuItemType.All:
        itemsMap.forEach((item) => {
          if (item.value.type === PluginContextMenuItemType.All) {
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

  addMainButtonItem = (key, mainButtonItem) => {
    this.mainButtonItems.set(key, mainButtonItem);
  };

  deleteMainButtonItem = (key) => {
    this.mainButtonItems.delete(key);
  };

  addProfileMenuItem = (key, profileMenuItem) => {
    this.profileMenuItems.set(key, profileMenuItem);
  };

  deleteProfileMenuItem = (key) => {
    this.profileMenuItems.delete(key);
  };

  get contextMenuItemsList() {
    const items = [];

    Array.from(this.contextMenuItems, ([key, value]) =>
      items.push({ key, value })
    );

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get mainButtonItemsList() {
    const items = [];

    Array.from(this.mainButtonItems, ([key, value]) =>
      items.push({ key, value })
    );

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }

  get profileMenuItemsList() {
    const items = [];

    Array.from(this.profileMenuItems, ([key, value]) =>
      items.push({ key, value })
    );

    if (items.length > 0) {
      items.sort((a, b) => a.value.position < b.value.position);

      return items;
    }

    return null;
  }
}

export default PluginStore;
