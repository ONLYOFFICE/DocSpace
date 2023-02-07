import { ContextMenuItemType } from "../../enums/ContextMenuItemType";

export interface ISeparatorItem {
  key: string;
  position: number;
  isSeparator: boolean;
  type?: ContextMenuItemType;
}
