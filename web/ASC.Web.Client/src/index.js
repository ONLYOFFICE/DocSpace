import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import store from "./store/store";
import "./custom.scss";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { ErrorBoundary, store as commonStore } from "asc-web-common";
import { Provider as MobxProvider } from "mobx-react";

const { authStore } = commonStore;

ReactDOM.render(
  <Provider store={store}>
    <MobxProvider store={authStore}>
      <ErrorBoundary>
        <App />
      </ErrorBoundary>
    </MobxProvider>
  </Provider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA

serviceWorker.register();
