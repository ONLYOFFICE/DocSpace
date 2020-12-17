export const checkPropertyByLink = (linkArr, data, property, index = 0) => {
  const length = linkArr.length;
  const currentElement = linkArr[index];
  const item = data.find((item) => item.link === currentElement);

  if ((index === length - 1 || !item.children) && item) {
    return item[property];
  } else if (!item) {
    return false;
  } else {
    const newIndex = index + 1;
    return checkPropertyByLink(linkArr, item.children, property, newIndex);
  }
};
