import {
  IContextMenuItem,
  ContextMenuItemType,
  ISeparatorItem,
} from "docspace-plugin";

const onHelloClick = () => {
  window.alert("Hello from folder");
};

const getFoldersItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const helloItem: IContextMenuItem = {
    key: "hello-folder-item",
    type: ContextMenuItemType.Folders,
    position: 3,
    label: "Hello folder",
    icon: "https://cdn-icons-png.flaticon.com/512/3898/3898671.png",
    onClick: onHelloClick,
  };

  const separatorItemFirst: ISeparatorItem = {
    key: "hello-folders-separator-1",
    type: ContextMenuItemType.Folders,
    position: 2,
    isSeparator: true,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "hello-folders-separator-2",
    type: ContextMenuItemType.Folders,
    position: 4,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(separatorItemSecond);
  items.push(helloItem);

  return items;
};

export default getFoldersItems;
