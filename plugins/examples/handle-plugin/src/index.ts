import { Events, IPlugin } from "docspace-plugin";

import pack from "../package.json";

import createHandler from "./handlers/create";
import renameHandler from "./handlers/rename";
import createRoomHandler from "./handlers/createRoom";

// class name can be anything
// for connect more plugin type - add suitable interface at implements block
class Handleplugin implements IPlugin {
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
  activate(): null {
    window.addEventListener(Events.CREATE, createHandler);
    window.addEventListener(Events.RENAME, renameHandler);
    window.addEventListener(Events.ROOM_CREATE, createRoomHandler);

    return null;
  }

  // this method will be called when the plugin will be deactivated
  // here you can remove event listeners and etc.
  // also here you must call activation methods for items of other plugins
  deactivate(): null {
    window.removeEventListener(Events.CREATE, createHandler);
    window.removeEventListener(Events.RENAME, renameHandler);
    window.removeEventListener(Events.ROOM_CREATE, createRoomHandler);

    return null;
  }
}

// create instance of the plugin
// instance name can be anything
// the main thing is to pass it to window.Plugins
const handleplugin = new Handleplugin();

//!!!don't touch it!!!
declare global {
  interface Window {
    Plugins: any;
  }
}

// if you want to change name of plugin at window.Plugins
// you should change output file name at webpack.config.js to same name
window.Plugins.Handleplugin = handleplugin || {};
