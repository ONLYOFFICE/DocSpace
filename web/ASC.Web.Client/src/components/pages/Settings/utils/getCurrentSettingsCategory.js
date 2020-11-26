import { getSettingsIndex } from "./getSettingsIndex";
export const getCurrentSettingsCategory = (arrayOfParams, settingsTree) => {
  let key = "0-0";
  let groupIndex = null;
  let categoryIndex = 0;

  for (let i = 0; i < arrayOfParams.length || i < 2; i++) {
    const currentParam = arrayOfParams[i];

    if (!currentParam) continue;

    switch (i) {
      case 0:
        groupIndex = getSettingsIndex(settingsTree, currentParam);
        key =
          groupIndex || groupIndex === 0
            ? settingsTree[groupIndex].key + "-0"
            : key;
        break;

      case 1:
        categoryIndex = getSettingsIndex(
          settingsTree,
          currentParam,
          groupIndex
        );
        key =
          (categoryIndex || categoryIndex === 0) &&
          (groupIndex || groupIndex === 0)
            ? settingsTree[groupIndex].children[categoryIndex].key
            : key;
        break;

      default:
        break;
    }
  }

  return key;
};
