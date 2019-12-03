import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import store from "./store/store";
import { fetchGroups, fetchPeople } from "./store/people/actions";
import config from "../package.json";
import "./custom.scss";
import App from "./App";

import * as serviceWorker from "./serviceWorker";
import { store as commonStore, constants } from "asc-web-common";
import { getFilterByLocation } from "./helpers/converters";
const { setIsLoaded, getUserInfo, setCurrentProductId, setCurrentProductHomePage, getPortalPasswordSettings, getPortalCultures, getPortalInviteLinks } = commonStore.auth.actions;
const { AUTH_KEY } = constants;

const token = localStorage.getItem(AUTH_KEY);

if (token) {
  getUserInfo(store.dispatch)
  .then(() => getPortalPasswordSettings(store.dispatch))
  .then(() => getPortalCultures(store.dispatch))
  .then(() => store.dispatch(getPortalInviteLinks()))
  .then(() => fetchGroups(store.dispatch))
  .then(() => {
    var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    const match = window.location.pathname.match(re);

    if (match && match.length > 0) {
      const newFilter = getFilterByLocation(window.location);
      return fetchPeople(newFilter, store.dispatch);
    }

    return Promise.resolve();
  })
  .then(() => { 
    store.dispatch(setCurrentProductHomePage(config.homepage));
    store.dispatch(setCurrentProductId("f4d98afd-d336-4332-8778-3c6945c81ea0"));
    store.dispatch(setIsLoaded(true));
  });
}
else { 
  store.dispatch(setIsLoaded(true));
};

ReactDOM.render(
  <Provider store={store}>
    <App />
  </Provider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
