import { BreadCrumb, Security } from "./FilesSelector.types";

export const PAGE_COUNT = 100;

export const defaultBreadCrumb: BreadCrumb = {
  label: "DocSpace",
  id: 0,
  isRoom: false,
};

export const SHOW_LOADER_TIMER = 500;
export const MIN_LOADER_TIMER = 500;

export const getHeaderLabel = (
  t: any,
  isCopy?: boolean,
  isRestoreAll?: boolean,
  isMove?: boolean
) => {
  if (isMove) return t("Common:MoveTo");
  if (isCopy) return t("Common:Copy");
  if (isRestoreAll) return t("Common:Restore");

  return t("Common:Save");
};

export const getAcceptButtonLabel = (
  t: any,
  isCopy?: boolean,
  isRestoreAll?: boolean,
  isMove?: boolean
) => {
  if (isMove) return t("Translations:MoveHere");
  if (isCopy) return t("Translations:CopyHere");
  if (isRestoreAll) return t("Common:RestoreHere");

  return t("Common:SaveHereButton");
};

export const getIsDisabled = (
  isFirstLoad: boolean,
  sameId?: boolean,
  isRooms?: boolean,
  isRoot?: boolean,
  isCopy?: boolean,
  isMove?: boolean,
  isRestoreAll?: boolean,
  isRequestRunning?: boolean,
  security?: Security
) => {
  if (isFirstLoad) return true;
  if (isRequestRunning) return true;
  if (sameId && !isCopy) return true;
  if (isRooms) return true;
  if (isRoot) return true;
  if (isCopy) return !security?.CopyTo;
  if (isMove || isRestoreAll) return !security?.MoveTo;

  return false;
};
