import { IMainButtonItem } from "../items/IMainButtonItem";
import { ISeparatorItem } from "../items/ISeparatorItem";

export interface IMainButtonPlugin {
  mainButtonItems: Map<string, IMainButtonItem | ISeparatorItem>;

  addMainButtonItem(item: IMainButtonItem | ISeparatorItem): void;

  activateMainButtonItems(): Map<string, IMainButtonItem | ISeparatorItem>;

  deactivateMainButtonItems(): string[];
}
