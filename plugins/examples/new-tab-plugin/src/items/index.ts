import {
  IContextMenuItem,
  ISeparatorItem,
  ContextMenuItemType,
} from "docspace-plugin";

const openFolderNewTab = (item: any) => {
  const { id, title }: any = item;

  const res = confirm(
    `Are you sure want to open the ${title} folder in a new tab`
  );

  if (res) {
    open(
      `rooms/personal/filter?withSubfolders=true&folder=${id}&page=1&sortby=DateAndTime&sortorder=descending`
    );
  }
};

const openRoomNewTab = (item: any) => {
  const { id, title }: any = item;

  const res = confirm(
    `Are you sure want to open the ${title} room in a new tab`
  );

  if (res) {
    open(
      `rooms/shared/${id}/filter?withSubfolders=true&folder=11&page=1&sortby=DateAndTime&sortorder=descending`
    );
  }
};

const getItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const folderNewTab: IContextMenuItem = {
    key: "new-tab-folder-item",
    type: ContextMenuItemType.Folders,
    position: 1,
    label: "Open new tab",
    icon: "images/folder.react.svg",
    onClick: openFolderNewTab,
  };

  const roomNewTab: IContextMenuItem = {
    key: "new-tab-room-item",
    type: ContextMenuItemType.Rooms,
    position: 0,
    label: "Open new tab",
    icon: "images/folder.react.svg",
    onClick: openRoomNewTab,
  };

  const separatorItem: ISeparatorItem = {
    key: "separator-item-room",
    type: ContextMenuItemType.Rooms,
    position: 1,
    isSeparator: true,
  };

  items.push(folderNewTab);

  items.push(roomNewTab);
  items.push(separatorItem);

  return items;
};

export default getItems;
