import { IPlugin } from "docspace-plugin";

import pack from "../package.json";

// class name can be anything
// for connect more plugin type - add suitable interface at implements block
class ChangedName implements IPlugin {
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
    window.alert("Plugin was activated");

    return null;
  }

  // this method will be called when the plugin will be deactivated
  // here you can remove event listeners and etc.
  // also here you must call activation methods for items of other plugins
  deactivate(): null {
    return null;
  }
}

// create instance of the plugin
// instance name can be anything
// the main thing is to pass it to window.Plugins
const pluginInstance = new ChangedName();

//!!!don't touch it!!!
declare global {
  interface Window {
    Plugins: any;
  }
}

// if you want to change name of plugin at window.Plugins
// you should change output file name at webpack.config.js to same name
window.Plugins.ChangedName = pluginInstance || {};
