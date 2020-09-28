import { toastr as Toastr } from "asc-web-components";
import i18n from "./i18n";

const toastr = {
    clear: clear,
    error: error,
    info: info,
    success: success,
    warning: warning
};

const getTitleTranslation = title => {
    return(i18n.t(title))
}

function success(data, title, timeout, withCross, centerPosition) {   
    return Toastr.success(data, title ? title : getTitleTranslation("Done"), timeout, withCross, centerPosition);
}

function error(data, title, timeout, withCross, centerPosition) {
    return Toastr.error(data, title ? title : getTitleTranslation("Warning"), timeout, withCross, centerPosition);
}

function warning(data, title, timeout, withCross, centerPosition) {
    return Toastr.warning(data, title ? title : getTitleTranslation("Alert"), timeout, withCross, centerPosition);
}

function info(data, title, timeout, withCross, centerPosition) {
    return Toastr.info(data, title ? title : getTitleTranslation("Info"), timeout, withCross, centerPosition);
}

function clear() {
    return Toastr.clear();
}

export default toastr;
