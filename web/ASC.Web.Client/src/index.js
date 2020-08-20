import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import axios from "axios";
import store from "./store/store";
import "./custom.scss";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import {
  store as commonStore,
  constants,
  history,
  ErrorBoundary
} from "asc-web-common";

const {
  getUser,
  getPortalSettings,
  getModules,
  setIsLoaded
} = commonStore.auth.actions;

const { AUTH_KEY } = constants;

const token = localStorage.getItem(AUTH_KEY);

if (!token) {
  getPortalSettings(store.dispatch)
    .then(() => store.dispatch(setIsLoaded(true)))
    .catch(e => history.push(`/login/error=${e}`));
} else if (!window.location.pathname.includes("confirm/EmailActivation")) {
  const requests = [
    getUser(store.dispatch),
    getPortalSettings(store.dispatch),
    getModules(store.dispatch)
  ];

  axios
    .all(requests)
    .then(() => store.dispatch(setIsLoaded(true)))
    .catch(e => history.push(`/login/error=${e}`));
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
