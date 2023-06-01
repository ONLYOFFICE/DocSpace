/* Returns correct four values style (margin/padding etc) depending on interface direction (ltr/rtl)
 * Not suitable for border-radius! */
const getCorrectFourValuesStyle = (styleStr, interfaceDirection) => {
  if (interfaceDirection === "ltr") return styleStr;

  const styleArr = styleStr.split(" ");
  if (styleArr.length !== 4) return styleStr;

  const styleRightValue = styleArr[1];
  const styleLeftValue = styleArr[3];

  styleArr[1] = styleLeftValue;
  styleArr[3] = styleRightValue;

  return styleArr.join(" ");
};

export default getCorrectFourValuesStyle;
