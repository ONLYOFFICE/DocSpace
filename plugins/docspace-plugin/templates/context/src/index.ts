import {
  IPlugin,
  IContextMenuPlugin,
  IContextMenuItem,
  ISeparatorItem,
  PluginItems,
} from "docspace-plugin";

import pack from "../package.json";

import getItems from "./items";

// class name can be anything
// for connect more plugin type - add suitable interface at implements block
class ChangedName implements IPlugin, IContextMenuPlugin {
  // this collection contains all the elements for the context menu
  contextMenuItems: Map<string, IContextMenuItem | ISeparatorItem> = new Map<
    string,
    IContextMenuItem | ISeparatorItem
  >();
  // this method return plugin name
  // by default from package.json file
  getPluginName(): string {
    return pack.name;
  }

  // this method return plugin version
  // by default from package.json file
  getPluginVersion(): string {
    return pack.version;
  }

  // this method will be called when the plugin will be activated
  // here you can add event listeners and etc.
  // also here you must call activation methods for items of other plugins
  // if method return null - activated only event listeners
  // if you want activate other plugins:
  // return Map<PluginItems, Map<string, IContextMenuItem | IProfileMenuItem | IMainButtonItem | ISeparatorItem>>
  activate(): Map<PluginItems, Map<string, IContextMenuItem | ISeparatorItem>> {
    const pluginItems = new Map<
      PluginItems,
      Map<string, IContextMenuItem | ISeparatorItem>
    >();

    pluginItems.set(PluginItems.CONTEXT_MENU, this.activateContextMenuItems());

    return pluginItems;
  }

  // this method will be called when the plugin will be deactivated
  // here you can remove event listeners and etc.
  // also here you must call activation methods for items of other plugins
  // if method return null - deactivated only event listeners
  // if you want deactivate other plugins:
  // return Map<PluginItems, string[]>,
  // where string[] - array of items key of plugin
  deactivate(): Map<PluginItems, string[]> {
    const pluginItems = new Map<PluginItems, string[]>();

    pluginItems.set(
      PluginItems.CONTEXT_MENU,
      this.deactivateContextMenuItems()
    );

    return pluginItems;
  }

  // method set to context menu items new item
  addContextMenuItem(item: IContextMenuItem | ISeparatorItem): void {
    this.contextMenuItems.set(item.key, item);
  }

  // get context menu items needed for activation
  activateContextMenuItems(): Map<string, IContextMenuItem | ISeparatorItem> {
    return this.contextMenuItems;
  }

  // get keys of context menu items needed for deactivation
  deactivateContextMenuItems(): string[] {
    const contextMenuItemsKeys: string[] = [];

    Array.from(this.contextMenuItems, ([key, value]) =>
      contextMenuItemsKeys.push(key)
    );

    return contextMenuItemsKeys;
  }
}

// create instance of the plugin
// instance name can be anything
// the main thing is to pass it to window.Plugins
const pluginInstance = new ChangedName();

const items = getItems();

items.forEach((item) => {
  pluginInstance.addContextMenuItem(item);
});

//!!!don't touch it!!!
declare global {
  interface Window {
    Plugins: any;
  }
}

// if you want to change name of plugin at window.Plugins
// you should change output file name at webpack.config.js to same name
window.Plugins.ChangedName = pluginInstance || {};
