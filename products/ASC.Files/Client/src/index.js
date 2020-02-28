import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import store from "./store/store";
import { fetchMyFolder, fetchRootFolders } from "./store/files/actions";
import config from "../package.json";
import "./custom.scss";
import App from "./App";

import * as serviceWorker from "./serviceWorker";
import { store as commonStore, constants, ErrorBoundary } from "asc-web-common";
//import { getFilterByLocation } from "./helpers/converters";
const { setIsLoaded, getUserInfo, setCurrentProductId, setCurrentProductHomePage, getPortalPasswordSettings, getPortalCultures } = commonStore.auth.actions;
const { AUTH_KEY } = constants;

const token = localStorage.getItem(AUTH_KEY);

if (token) {
  getUserInfo(store.dispatch)
    .then(() => getPortalPasswordSettings(store.dispatch))
    .then(() => getPortalCultures(store.dispatch))
    //.then(() => fetchGroups(store.dispatch))
    .then(() => fetchMyFolder(store.dispatch))
    .then(() => fetchRootFolders(store.dispatch))
    .then(() => {
      // var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
      // const match = window.location.pathname.match(re);

      // if (match && match.length > 0) {
      //   const newFilter = getFilterByLocation(window.location);
      //   return fetchPeople(newFilter, store.dispatch);
      // }

      return Promise.resolve();
    })
    .then(() => {
      store.dispatch(setCurrentProductHomePage(config.homepage));
      store.dispatch(setCurrentProductId("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"));
      store.dispatch(setIsLoaded(true));
    });
}
else {
  store.dispatch(setIsLoaded(true));
};

ReactDOM.render(
  <Provider store={store}>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </Provider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
