import { IProfileMenuItem, ISeparatorItem } from "docspace-plugin";

const onJSDocClick = () => {
  const result = confirm("Open js documentation?");
  if (result) open("https://learn.javascript.ru/");
};

const onPythonDocClick = () => {
  const result = confirm("Open js documentation?");
  if (result) open("https://www.python.org/");
};

const getItems = (): Array<IProfileMenuItem | ISeparatorItem> => {
  const items: Array<IProfileMenuItem | ISeparatorItem> = [];

  const separatorItemFirst: ISeparatorItem = {
    key: "profile-separator-1",
    position: 1,
    isSeparator: true,
  };

  const jsDocItem: IProfileMenuItem = {
    key: "js-doc-item",
    position: 2,
    label: "JS documentation",
    icon: "https://cdn-icons-png.flaticon.com/512/5968/5968509.png",
    onClick: onJSDocClick,
  };

  const pythonDocItem: IProfileMenuItem = {
    key: "python-doc-item",
    position: 2,
    label: "Python documentation",
    icon: "https://cdn-icons-png.flaticon.com/512/919/919852.png",
    onClick: onPythonDocClick,
  };

  const separatorItemSecond: ISeparatorItem = {
    key: "profile-separator-2",
    position: 4,
    isSeparator: true,
  };

  items.push(separatorItemFirst);
  items.push(jsDocItem);
  items.push(pythonDocItem);
  items.push(separatorItemSecond);

  return items;
};

export default getItems;
