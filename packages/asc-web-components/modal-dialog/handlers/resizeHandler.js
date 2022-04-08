import { getModalType } from "../../utils/device";

export const getCurrentDisplayType = (
  stateDisplayType,
  propsDisplayType,
  onResize
) => {
  if (propsDisplayType !== "auto") return false;

  const newType = getTypeByWidth(propsDisplayType);
  if (newType === stateDisplayType) return false;

  onResize && onResize(newType);
  return newType;
};

export const getTypeByWidth = (propsDisplayType) => {
  if (propsDisplayType !== "auto") return propsDisplayType;
  return getModalType();
};

export const popstate = (onClose) => {
  window.removeEventListener("popstate", popstate, false);
  onClose();
  window.history.go(1);
};
