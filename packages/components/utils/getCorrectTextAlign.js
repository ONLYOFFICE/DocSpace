/*
 * Returns correct text-align value depending on interface direction (ltr/rtl)
 */
const getCorrectTextAlign = (currentTextAlign, interfaceDirection) => {
  if (interfaceDirection === "ltr") return currentTextAlign;

  switch (currentTextAlign) {
    case "left":
      return "right";
    case "right":
      return "left";
    default:
      return currentTextAlign;
  }
};

export default getCorrectTextAlign;
