import { store as commonStore } from "asc-web-common";
import store from "../store/store";

const { getCurrentProduct } = commonStore.auth.selectors;

export const setDocumentTitle = (subTitle = null) => {
  const state = store.getState();
  const { auth: commonState } = state;

  const { isAuthenticated, settings } = commonState;
  const { organizationName } = settings;

  let title;

  const currentModule = getCurrentProduct(state);

  if (subTitle) {
    if (isAuthenticated && currentModule) {
      title = subTitle + " - " + currentModule.title;
    } else {
      title = subTitle + " - " + organizationName;
    }
  } else if (currentModule && organizationName) {
    title = currentModule.title + " - " + organizationName;
  } else {
    title = organizationName;
  }

  document.title = title;
};
