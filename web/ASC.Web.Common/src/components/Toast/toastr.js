import { toastr as Toastr } from "asc-web-components";
import i18n from "./i18n";
import { LANGUAGE } from "../../constants";
import { changeLanguage } from "../../utils";
const toastr = {
  clear: clear,
  error: error,
  info: info,
  success: success,
  warning: warning,
};

const getTitleTranslation = (title) => {
  const currentLng = localStorage.getItem(LANGUAGE);
  if (i18n.language !== currentLng) changeLanguage(i18n, currentLng);
  return i18n.t(title);
};

function success(data, title, timeout, withCross, centerPosition) {
  return Toastr.success(
    data,
    title ? title : getTitleTranslation("Done"),
    timeout,
    withCross,
    centerPosition
  );
}

function error(data, title, timeout, withCross, centerPosition) {
  return Toastr.error(
    data,
    title ? title : getTitleTranslation("Warning"),
    timeout,
    withCross,
    centerPosition
  );
}

function warning(data, title, timeout, withCross, centerPosition) {
  return Toastr.warning(
    data,
    title ? title : getTitleTranslation("Alert"),
    timeout,
    withCross,
    centerPosition
  );
}

function info(data, title, timeout, withCross, centerPosition) {
  return Toastr.info(
    data,
    title ? title : getTitleTranslation("Info"),
    timeout,
    withCross,
    centerPosition
  );
}

function clear() {
  return Toastr.clear();
}

export default toastr;
