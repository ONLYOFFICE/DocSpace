import authStore from "@appserver/common/store/AuthStore";
//import store from "../store/store";

//const { getCurrentProduct } = commonStore.auth.selectors;

export const setDocumentTitle = (subTitle = null) => {
  // const state = store.getState();
  // const { auth: commonState } = state;
  const { isAuthenticated, settingsStore, product: currentModule } = authStore;
  const { organizationName } = settingsStore;

  let title;
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
