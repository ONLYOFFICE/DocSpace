import { checkForRoot } from "./checkForRoot";
import { selectFirstChildItem } from "./selectFirstChildItem";

export const selectKeyOfTreeElement = (value, settingsTree) => {
  const selectedKey = value[0];
  const isRootElementSelected = checkForRoot(selectedKey, settingsTree);

  if (isRootElementSelected) {
    const firstChildren = selectFirstChildItem(selectedKey[0], settingsTree);
    return firstChildren;
  } else {
    return value;
  }
};
