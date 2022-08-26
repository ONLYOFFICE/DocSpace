import {
  IContextMenuItem,
  ISeparatorItem,
  ContextMenuItemType,
} from "docspace-plugin";

const onHelloClick = () => {
  window.alert("Hello from file");
};

const getFilesItems = (): Array<IContextMenuItem | ISeparatorItem> => {
  const items: Array<IContextMenuItem | ISeparatorItem> = [];

  const separatorItemFirst: ISeparatorItem = {
    key: "hello-files-separator-1",
    position: 4,
    type: ContextMenuItemType.Files,
    isSeparator: true,
  };

  const helloItem: IContextMenuItem = {
    key: "hello-files-item",
    type: ContextMenuItemType.Files,
    position: 5,
    label: "Hello file",
    icon: "https://cdn-icons-png.flaticon.com/512/3898/3898671.png",
    onClick: onHelloClick,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "hello-files-separator-2",
    position: 6,
    type: ContextMenuItemType.Files,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(helloItem);
  items.push(separatorItemSecond);

  return items;
};

export default getFilesItems;
