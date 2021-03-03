import toastr from "@appserver/components/toast/toastr";
import i18n from "./i18n";

const getTitleTranslation = (title) => {
  return i18n.t(title);
};

function success(data, title, timeout, withCross, centerPosition) {
  return toastr.success(
    data,
    title ? title : getTitleTranslation("Done"),
    timeout,
    withCross,
    centerPosition
  );
}

function error(data, title, timeout, withCross, centerPosition) {
  return toastr.error(
    data,
    title ? title : getTitleTranslation("Warning"),
    timeout,
    withCross,
    centerPosition
  );
}

function warning(data, title, timeout, withCross, centerPosition) {
  return toastr.warning(
    data,
    title ? title : getTitleTranslation("Alert"),
    timeout,
    withCross,
    centerPosition
  );
}

function info(data, title, timeout, withCross, centerPosition) {
  return toastr.info(
    data,
    title ? title : getTitleTranslation("Info"),
    timeout,
    withCross,
    centerPosition
  );
}

function clear() {
  return toastr.clear();
}

export default {
  clear: clear,
  error: error,
  info: info,
  success: success,
  warning: warning,
};
