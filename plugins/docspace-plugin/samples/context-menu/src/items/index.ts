import { IContextMenuItem, ISeparatorItem } from "docspace-plugin";

import getAllItems from "./all";
import getFilesItems from "./files";
import getFoldersItems from "./folders";
import getRoomsItems from "./rooms";

const getItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const allFiles: Array<IContextMenuItem | ISeparatorItem> = getAllItems();
  const filesItems: Array<IContextMenuItem | ISeparatorItem> = getFilesItems();
  const foldersItems: Array<IContextMenuItem | ISeparatorItem> =
    getFoldersItems();
  const roomsFolders: Array<IContextMenuItem | ISeparatorItem> =
    getRoomsItems();

  if (allFiles.length > 0) items.push(...allFiles);
  if (filesItems.length > 0) items.push(...filesItems);
  if (foldersItems.length > 0) items.push(...foldersItems);
  if (roomsFolders.length > 0) items.push(...roomsFolders);

  return items;
};

export default getItems;
