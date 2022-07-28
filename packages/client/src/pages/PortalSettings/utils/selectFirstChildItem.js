export const selectFirstChildItem = (rootKey, data) => {
  const item = data.find((item) => item.key[0] === rootKey);
  if (item.children) {
    return selectFirstChildItem(rootKey, item.children);
  } else {
    return [data[0].key];
  }
};
