import { size } from "../../utils/device";

const getCurrentSizeName = () => {
  const innerWidth = window.innerWidth;
  return innerWidth > size.tablet
    ? "desktop"
    : innerWidth <= size.tablet && innerWidth > size.smallTablet
    ? "tablet"
    : innerWidth <= size.smallTablet && innerWidth > size.mobile
    ? "smallTablet"
    : "mobile";
};

export const getCurrentDisplayType = (
  propsDisplayType,
  propsDisplayTypeDetailed
) => {
  if (!propsDisplayTypeDetailed) return propsDisplayType;

  const detailedDisplayType = propsDisplayTypeDetailed[getCurrentSizeName()];

  if (detailedDisplayType) return detailedDisplayType;
  return propsDisplayType;
};
