import React from "react";
import ReactDOM from "react-dom";
import "./custom.scss";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { ErrorBoundary, store as commonStore } from "asc-web-common";
import { Provider as MobxProvider } from "mobx-react";
import PaymentStore from "./store/PaymentStore";
import WizardStore from "./store/WizardStore";
import SettingsSetupStore from "./store/SettingsSetupStore";

const { authStore } = commonStore;
const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore();

ReactDOM.render(
  <MobxProvider
    auth={authStore}
    payments={paymentStore}
    wizard={wizardStore}
    setup={setupStore}
  >
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </MobxProvider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA

serviceWorker.register();
