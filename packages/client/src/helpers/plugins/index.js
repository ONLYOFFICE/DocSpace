import PluginStore from "./PluginStore";

import api from "./api";

import { loadScript } from "@docspace/common/utils";
import { PluginContextMenuItemType } from "./constants";

export const initPluginStore = async () => {
  window.PluginStore = new PluginStore();

  const plugins = await api.getPlugins();

  plugins.forEach((plugin) => initPlugin(plugin));
};

export const initPlugin = (plugin, callback) => {
  const onLoad = () => {
    const newPlugin = eval(`${plugin.name}`);
    newPlugin.isActive = plugin.isActive;
    newPlugin.id = plugin.id;

    window.PluginStore.installPlugin(plugin.id, newPlugin);

    callback && callback(newPlugin);
  };

  loadScript(
    `/static/scripts/${plugin.filename}`,
    `${plugin.name}-${plugin.id}`,
    onLoad
  );
};

export const activatePlugin = async (id, status) => {
  if (status === "true") {
    window.PluginStore.activatePlugin(id);
  } else {
    window.PluginStore.deactivatePlugin(id);
  }

  const plugin = await api.activatePlugin(id);

  return plugin;
};

export const getContextMenuKeysByType = (type) => {
  const itemsMap = Array.from(window.PluginStore.getContextMenuItems());
  const keys = [];

  switch (type) {
    case PluginContextMenuItemType.Files:
      itemsMap.forEach((item) => {
        if (item[1].type === PluginContextMenuItemType.Files) {
          keys.push(item[0]);
        }
      });
      break;
    case PluginContextMenuItemType.Folders:
      itemsMap.forEach((item) => {
        if (item[1].type === PluginContextMenuItemType.Folders) {
          keys.push(item[0]);
        }
      });
      break;
    case PluginContextMenuItemType.Rooms:
      itemsMap.forEach((item) => {
        if (item[1].type === PluginContextMenuItemType.Rooms) {
          keys.push(item[0]);
        }
      });
      break;
    case PluginContextMenuItemType.All:
      itemsMap.forEach((item) => {
        if (item[1].type === PluginContextMenuItemType.All) {
          keys.push(item[0]);
        }
      });
      break;
    default:
      return null;
  }

  if (keys.length === 0) return null;

  return keys;
};

export const getContextMenuItems = () => {
  const itemsMap = window.PluginStore.getContextMenuItems();
  const items = [];

  Array.from(itemsMap, ([key, value]) => items.push({ key, value }));

  if (items.length > 0) {
    items.sort((a, b) => a.value.position < b.value.position);

    return items;
  }

  return null;
};

export const getMainButtonItems = () => {
  const itemsMap = window.PluginStore.getMainButtonItems();
  const items = [];

  Array.from(itemsMap, ([key, value]) => items.push({ key, value }));

  if (items.length > 0) {
    items.sort((a, b) => a.value.position < b.value.position);

    return items;
  }

  return null;
};

export const getProfileMenuItems = () => {
  const itemsMap = window.PluginStore.getProfileMenuItems();
  const items = [];

  Array.from(itemsMap, ([key, value]) => items.push({ key, value }));

  if (items.length > 0) {
    items.sort((a, b) => a.value.position < b.value.position);

    return items;
  }

  return null;
};
