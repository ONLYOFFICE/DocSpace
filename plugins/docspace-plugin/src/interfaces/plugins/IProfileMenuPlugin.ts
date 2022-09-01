import { IProfileMenuItem } from "../items/IProfileMenuItem";
import { ISeparatorItem } from "../items/ISeparatorItem";

export interface IProfileMenuPlugin {
  profileMenuItems: Map<string, IProfileMenuItem | ISeparatorItem>;

  addProfileMenuItem(item: IProfileMenuItem | ISeparatorItem): void;

  activateProfileMenuItems(): Map<string, IProfileMenuItem | ISeparatorItem>;

  deactivateProfileMenuItems(): string[];
}
