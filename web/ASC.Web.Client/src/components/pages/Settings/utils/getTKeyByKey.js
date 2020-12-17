export const getTKeyByKey = (key, treeData, depth = 0) => {
  let newKey = key.slice(0, 1 + 2 * depth);
  const item = treeData.find((item) => item.key === newKey);
  if (key === newKey) {
    return item.tKey;
  } else {
    const depthLevel = depth + 1;
    return getTKeyByKey(key, item.children, depthLevel);
  }
};
