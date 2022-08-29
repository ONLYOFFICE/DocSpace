import {
  IPlugin,
  IProfileMenuPlugin,
  IProfileMenuItem,
  ISeparatorItem,
  PluginItems,
} from "docspace-plugin";

import pack from "../package.json";

import getItems from "./items";

// class name can be anything
// for connect more plugin type - add suitable interface at implements block
class ChangedName implements IPlugin, IProfileMenuPlugin {
  // this collection contains all the elements for the profile menu
  profileMenuItems: Map<string, IProfileMenuItem | ISeparatorItem> = new Map<
    string,
    IProfileMenuItem | ISeparatorItem
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
  // return Map<PluginItems, Map<string, IContextMenuItem | IMainButtonItem | IProfileMenuItem | ISeparatorItem>>
  activate(): Map<PluginItems, Map<string, IProfileMenuItem | ISeparatorItem>> {
    const pluginItems = new Map<
      PluginItems,
      Map<string, IProfileMenuItem | ISeparatorItem>
    >();

    pluginItems.set(PluginItems.PROFILE_MENU, this.activateProfileMenuItems());

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
      PluginItems.PROFILE_MENU,
      this.deactivateProfileMenuItems()
    );

    return pluginItems;
  }

  // method set to profile menu items new item
  addProfileMenuItem(item: IProfileMenuItem | ISeparatorItem): void {
    this.profileMenuItems.set(item.key, item);
  }
  // get profile menu items needed for activation
  activateProfileMenuItems(): Map<string, IProfileMenuItem | ISeparatorItem> {
    return this.profileMenuItems;
  }

  // get keys of profile menu items needed for deactivation
  deactivateProfileMenuItems(): string[] {
    const profileMenuItemsKeys: string[] = [];

    Array.from(this.profileMenuItems, ([key, value]) =>
      profileMenuItemsKeys.push(key)
    );

    return profileMenuItemsKeys;
  }
}

// create instance of the plugin
// instance name can be anything
// the main thing is to pass it to window.Plugins
const pluginInstance = new ChangedName();

const items: Array<IProfileMenuItem | ISeparatorItem> = getItems();

items.forEach((item) => pluginInstance.addProfileMenuItem(item));

//!!!don't touch it!!!
declare global {
  interface Window {
    Plugins: any;
  }
}

// if you want to change name of plugin at window.Plugins
// you should change output file name at webpack.config.js to same name
window.Plugins.ChangedName = pluginInstance || {};
