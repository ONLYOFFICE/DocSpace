import {
  IPlugin,
  IMainButtonPlugin,
  IMainButtonItem,
  ISeparatorItem,
  PluginItems,
} from "docspace-plugin";

import pack from "../package.json";

import getItems from "./items";

// class name can be anything
// for connect more plugin type - add suitable interface at implements block
class ChangedName implements IPlugin, IMainButtonPlugin {
  // this collection contains all the elements for the main button
  mainButtonItems: Map<string, IMainButtonItem | ISeparatorItem> = new Map<
    string,
    IMainButtonItem | ISeparatorItem
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
  activate(): Map<PluginItems, Map<string, IMainButtonItem | ISeparatorItem>> {
    const pluginItems = new Map<
      PluginItems,
      Map<string, IMainButtonItem | ISeparatorItem>
    >();

    pluginItems.set(PluginItems.ACTION_BUTTON, this.activateMainButtonItems());

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
      PluginItems.ACTION_BUTTON,
      this.deactivateMainButtonItems()
    );

    return pluginItems;
  }

  // method set to main button items new item
  addMainButtonItem(item: IMainButtonItem | ISeparatorItem): void {
    this.mainButtonItems.set(item.key, item);
  }
  // get main button items needed for activation
  activateMainButtonItems(): Map<string, IMainButtonItem | ISeparatorItem> {
    return this.mainButtonItems;
  }

  // get keys of main button items needed for deactivation
  deactivateMainButtonItems(): string[] {
    const mainButtonItemsKeys: string[] = [];

    Array.from(this.mainButtonItems, ([key, value]) =>
      mainButtonItemsKeys.push(key)
    );

    return mainButtonItemsKeys;
  }
}

// create instance of the plugin
// instance name can be anything
// the main thing is to pass it to window.Plugins
const pluginInstance = new ChangedName();

const items: Array<IMainButtonItem | ISeparatorItem> = getItems();

items.forEach((item) => pluginInstance.addMainButtonItem(item));

//!!!don't touch it!!!
declare global {
  interface Window {
    Plugins: any;
  }
}

// if you want to change name of plugin at window.Plugins
// you should change output file name at webpack.config.js to same name
window.Plugins.ChangedName = pluginInstance || {};
