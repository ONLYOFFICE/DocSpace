import toastr from "@docspace/components/toast/toastr";
import i18n from "./i18n";

function success(data, title, timeout, withCross, centerPosition) {
  return toastr.success(
    data,
    title ? title : i18n.t("Done"),
    timeout,
    withCross,
    centerPosition
  );
}

function error(data, title, timeout, withCross, centerPosition) {
  return toastr.error(
    data,
    title ? title : i18n.t("Common:Warning"),
    timeout,
    withCross,
    centerPosition
  );
}

function warning(data, title, timeout, withCross, centerPosition) {
  return toastr.warning(
    data,
    title ? title : i18n.t("Alert"),
    timeout,
    withCross,
    centerPosition
  );
}

function info(data, title, timeout, withCross, centerPosition) {
  return toastr.info(
    data,
    title ? title : i18n.t("Common:Info"),
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
