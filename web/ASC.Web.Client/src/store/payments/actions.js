import { api, store } from "asc-web-common";
// const { setLicenseUpload } = store.wizard.actions;
// const { setIsConfirmLoaded } = store.confirm.actions;
import { setLicenseUpload } from "../wizard/actions";
const { setTimezones, setPortalCultures } = store.auth.actions;

export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => dispatch(setLicenseUpload(res)));
  };
}

// export function getPortalCultures() {
//   return (dispatch) => {
//     return api.settings.getPortalCultures().then((cultures) => {
//       dispatch(setPortalCultures(cultures));
//     });
//   };
// }

// export function getPortalTimezones() {
//   return (dispatch) => {
//     return api.settings.getPortalTimezones().then((timezones) => {
//       dispatch(setTimezones(timezones));
//     });
//   };
// }
