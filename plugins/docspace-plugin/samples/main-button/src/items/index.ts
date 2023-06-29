import { IMainButtonItem, ISeparatorItem } from "docspace-plugin";

const onClick = () => {
  alert("User clicked on main button item from plugin");
};

const getItems = (): Array<IMainButtonItem | ISeparatorItem> => {
  const items: Array<IMainButtonItem | ISeparatorItem> = [];

  const separatorItemFirst: ISeparatorItem = {
    key: "hello-All-separator-1",
    position: 1,
    isSeparator: true,
  };

  const clickItem: IMainButtonItem = {
    key: "hello-all-item",
    position: 2,
    label: "Hello all",
    icon: "https://cdn-icons-png.flaticon.com/512/7491/7491793.png",
    onClick: onClick,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "hello-All-separator-2",
    position: 3,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(clickItem);
  items.push(separatorItemSecond);

  return items;
};

export default getItems;
