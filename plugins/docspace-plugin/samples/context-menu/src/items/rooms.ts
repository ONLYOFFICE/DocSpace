import {
  IContextMenuItem,
  ContextMenuItemType,
  ISeparatorItem,
} from "docspace-plugin";

const onHelloClick = () => {
  window.alert("Hello from room");
};

const getRoomsItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const helloItem: IContextMenuItem = {
    key: "hello-room-item",
    type: ContextMenuItemType.Rooms,
    position: 1,
    label: "Hello room",
    icon: "https://cdn-icons-png.flaticon.com/512/3898/3898671.png",
    onClick: onHelloClick,
  };

  const separatorItemFirst: ISeparatorItem = {
    key: "hello-rooms-separator-1",
    type: ContextMenuItemType.Rooms,
    position: 0,
    isSeparator: true,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "hello-rooms-separator-2",
    type: ContextMenuItemType.Rooms,
    position: 2,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(separatorItemSecond);
  items.push(helloItem);

  return items;
};

export default getRoomsItems;
