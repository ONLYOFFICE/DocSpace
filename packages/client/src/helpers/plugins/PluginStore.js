import { PluginType } from "./constants";

class PluginStore {
  plugins = null;
  contextMenuItems = null;
  mainButtonItems = null;
  profileMenuItems = null;

  constructor() {
    this.plugins = new Map();
    this.contextMenuItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();
  }

  installPlugin(id, plugin) {
    this.plugins.set(+id, plugin);

    if (plugin.isActive) {
      this.activatePlugin(+id);
    }
  }

  uninstallPlugin(id) {
    this.deactivatePlugin(id);
    this.plugins.delete(+id);
  }

  getInstalledPlugins() {
    return this.plugins;
  }

  activatePlugin(id) {
    this.plugins.get(+id).isActive = true;
    const pluginItems = this.plugins.get(+id).activate();

    if (!pluginItems) return;

    Array.from(pluginItems, ([key, value]) => {
      switch (key) {
        case PluginType.CONTEXT_MENU:
          Array.from(value, ([key, item]) =>
            this.addContextMenuItem(key, item)
          );
          return;
        case PluginType.ACTION_BUTTON:
          Array.from(value, ([key, item]) => this.addMainButtonItem(key, item));
          return;
        case PluginType.PROFILE_MENU:
          Array.from(value, ([key, item]) =>
            this.addProfileMenuItem(key, item)
          );
          return;
      }
    });
  }

  deactivatePlugin(id) {
    this.plugins.get(+id).isActive = false;
    const pluginItems = this.plugins.get(+id).deactivate();

    if (!pluginItems) return;

    Array.from(pluginItems, ([key, value]) => {
      switch (key) {
        case PluginType.CONTEXT_MENU:
          value.map((key) => this.deleteContextMenuItem(key));
          return;
        case PluginType.ACTION_BUTTON:
          value.map((key) => this.deleteMainButtonItem(key));
          return;
        case PluginType.PROFILE_MENU:
          value.map((key) => this.deleteProfileMenuItem(key));
          return;
      }
    });
  }

  addContextMenuItem(key, contextMenuItem) {
    this.contextMenuItems.set(key, contextMenuItem);
  }

  deleteContextMenuItem(key) {
    this.contextMenuItems.delete(key);
  }

  getContextMenuItems() {
    return this.contextMenuItems;
  }

  addMainButtonItem(key, mainButtonItem) {
    this.mainButtonItems.set(key, mainButtonItem);
  }

  deleteMainButtonItem(key) {
    this.mainButtonItems.delete(key);
  }

  getMainButtonItems() {
    return this.mainButtonItems;
  }

  addProfileMenuItem(key, profileMenuItem) {
    this.profileMenuItems.set(key, profileMenuItem);
  }

  deleteProfileMenuItem(key) {
    this.profileMenuItems.delete(key);
  }

  getProfileMenuItems() {
    return this.profileMenuItems;
  }
}

export default PluginStore;
