import { store, api } from "asc-web-common";
// const { setLicenseUpload } = store.wizard.actions;
// const { setIsConfirmLoaded } = store.confirm.actions;
import { setLicenseUpload, resetLicenseUploaded } from "../wizard/actions";
export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => dispatch(setLicenseUpload(res)));
  };
}
