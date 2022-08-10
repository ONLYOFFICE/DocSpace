export const getKeyByLink = (linkArr, data, index = 0) => {
  const length = linkArr.length;
  const currentElement = linkArr[index];
  const item = data.find((item) => item.link === currentElement);

  if (index === length - 1 && item) {
    return item.key;
  } else if (!item || !item.children) {
    return "0";
  } else {
    const newIndex = index + 1;
    return getKeyByLink(linkArr, item.children, newIndex);
  }
};
