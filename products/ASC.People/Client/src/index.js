import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import Cookies from "universal-cookie";
import setAuthorizationToken from "./store/services/setAuthorizationToken";
import { AUTH_KEY } from "./helpers/constants";
import store from "./store/store";
import "./custom.scss";
import App from "./App";
import i18n from "./i18n";
import {I18nextProvider} from "react-i18next";

import * as serviceWorker from "./serviceWorker";
import { setIsLoaded, getUserInfo } from "./store/auth/actions";

var token = new Cookies().get(AUTH_KEY);

if (token) {
  setAuthorizationToken(token);
  store.dispatch(getUserInfo);
}
else {
  store.dispatch(setIsLoaded(true));
}

ReactDOM.render(
  <I18nextProvider i18n={i18n}>
  <Provider store={store}>
    <App />
  </Provider>
  </I18nextProvider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
