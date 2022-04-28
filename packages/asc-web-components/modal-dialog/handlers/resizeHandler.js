import { getCurrentSizeName } from "../../utils/device";

export const getCurrentDisplayType = (
  propsDisplayType,
  propsDisplayTypeDetailed
) => {
  if (!propsDisplayTypeDetailed) return propsDisplayType;

  const detailedDisplayType = propsDisplayTypeDetailed[getCurrentSizeName()];
  if (detailedDisplayType) return detailedDisplayType;

  return propsDisplayType;
};
