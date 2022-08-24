import { ContextMenuItemType } from "../../enums/ContextMenuItemType";

export interface IContextMenuItem {
  key: string;
  type: ContextMenuItemType;
  position: number;
  label: string;
  icon: string;
  onClick: (item: any | null) => void;
}
