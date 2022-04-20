import { getCurrentSizeName } from "../../utils/device";

export const getCurrentDisplayType = (
  propsDisplayType,
  propsDisplayTypeDetailed
) => {
  const detailedDisplayType = propsDisplayTypeDetailed[getCurrentSizeName()];
  if (detailedDisplayType) return detailedDisplayType;
  return propsDisplayType;
};
