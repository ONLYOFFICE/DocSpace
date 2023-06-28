import { PluginType } from "./constants";

export const wrapPluginItem = (pluginName, key, item, pluginType, frame) => {
  if (item?.onClick) {
    const plugin = frame.contentWindow.Plugins[pluginName];

    const onClick = (value) => {
      switch (pluginType) {
        case PluginType.CONTEXT_MENU:
          plugin.contextMenuItems.get(key).onClick(value);
          break;
        case PluginType.ACTION_BUTTON:
          plugin.mainButtonItems.get(key).onClick(value);
          break;
        case PluginType.PROFILE_MENU:
          plugin.profileMenuItems.get(key).onClick(value);
          break;
      }
    };

    return { ...item, onClick };
  }

  return item;
};
