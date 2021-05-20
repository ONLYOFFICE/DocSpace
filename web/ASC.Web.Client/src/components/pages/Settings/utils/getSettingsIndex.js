export const getSettingsIndex = (settingsTree, currentParam, groupIndex) => {
  const currentCategories =
    groupIndex || groupIndex === 0
      ? settingsTree[groupIndex].children
      : settingsTree;

  for (let i = 0; i < currentCategories.length; i++) {
    if (currentParam && currentCategories[i].link === currentParam) {
      return i;
    }
  }
};
