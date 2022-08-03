class PluginStore {
  plugins = null;
  contextMenuItems = null;
  mainButtonItems = null;

  constructor() {
    this.plugins = new Map();
    this.contextMenuItems = new Map();
    this.mainButtonItems = new Map();
    this.profileMenuItems = new Map();
  }

  installPlugin(id, plugin) {
    plugin.init(this);

    this.plugins.set(+id, plugin);

    if (plugin.isActive) {
      this.activatePlugin(+id);
    }
  }

  uninstallPlugin(id) {
    this.plugins.delete(+id);
  }

  getInstalledPlugins() {
    return this.plugins;
  }

  activatePlugin(id) {
    this.plugins.get(+id).isActive = true;
    this.plugins.get(+id).activate();
  }

  deactivatePlugin(id) {
    this.plugins.get(+id).isActive = false;
    this.plugins.get(+id).deactivate();
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
