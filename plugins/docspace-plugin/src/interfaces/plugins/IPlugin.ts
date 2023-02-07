import { IContextMenuItem } from "../items/IContextMenuItem";
import { IMainButtonItem } from "../items/IMainButtonItem";
import { IProfileMenuItem } from "../items/IProfileMenuItem";
import { ISeparatorItem } from "../items/ISeparatorItem";

import { PluginItems } from "../../enums/Plugins";

export interface IPlugin {
  getPluginName(): string;
  getPluginVersion(): string;

  activate(): Map<
    PluginItems,
    Map<
      string,
      IMainButtonItem | IContextMenuItem | IProfileMenuItem | ISeparatorItem
    >
  > | null;

  deactivate(): Map<PluginItems, string[]> | null;
}
