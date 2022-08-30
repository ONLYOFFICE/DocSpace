import { IContextMenuItem } from "../items/IContextMenuItem";
import { ISeparatorItem } from "../items/ISeparatorItem";

export interface IContextMenuPlugin {
  contextMenuItems: Map<string, IContextMenuItem | ISeparatorItem>;

  addContextMenuItem(item: IContextMenuItem | ISeparatorItem): void;

  activateContextMenuItems(): Map<string, IContextMenuItem | ISeparatorItem>;

  deactivateContextMenuItems(): string[];
}
