import {
  IContextMenuItem,
  ISeparatorItem,
  ContextMenuItemType,
} from "docspace-plugin";

const onHelloClick = () => {
  window.alert("Hello from all");
};

const getAllItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const separatorItemFirst: ISeparatorItem = {
    key: "hello-All-separator-1",
    position: 7,
    type: ContextMenuItemType.All,
    isSeparator: true,
  };

  const helloItem: IContextMenuItem = {
    key: "hello-all-item",
    type: ContextMenuItemType.All,
    position: 8,
    label: "Hello all",
    icon: "https://cdn-icons-png.flaticon.com/512/3898/3898671.png",
    onClick: onHelloClick,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "hello-All-separator-2",
    position: 9,
    type: ContextMenuItemType.All,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(helloItem);
  items.push(separatorItemSecond);

  return items;
};

export default getAllItems;
