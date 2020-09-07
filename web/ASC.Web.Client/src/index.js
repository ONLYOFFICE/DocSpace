import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import axios from "axios";
import store from "./store/store";
import "./custom.scss";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { store as commonStore, constants, ErrorBoundary } from "asc-web-common";

const {
  getUser,
  getPortalSettings,
  getModules,
  setIsLoaded
} = commonStore.auth.actions;

const { AUTH_KEY } = constants;

const token = localStorage.getItem(AUTH_KEY);

const requests = [];

if (!token) {
  requests.push(getPortalSettings(store.dispatch));
} else if (!window.location.pathname.includes("confirm/EmailActivation")) {
  requests.push(getUser(store.dispatch));
  requests.push(getPortalSettings(store.dispatch));
  requests.push(getModules(store.dispatch));
}

if (requests.length > 0) {
  axios
    .all(requests)
    .catch(e => {
      console.log("INIT REQUESTS FAILED", e);
    })
    .finally(() => store.dispatch(setIsLoaded(true)));
}

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
