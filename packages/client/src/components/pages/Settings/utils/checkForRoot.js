export const checkForRoot = (key, treeData, depth = 0) => {
  let newKey = key.slice(0, 1 + 2 * depth);
  const item = treeData.find((item) => item.key === newKey);
  if (key === newKey) {
    return item.children ? true : false;
  } else {
    const depthLevel = depth + 1;
    return checkForRoot(key, item.children, depthLevel);
  }
};
