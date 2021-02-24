//import "./wdyr";

import React from "react";
import ReactDOM from "react-dom";
import "./custom.scss";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { ErrorBoundary, store as commonStore } from "asc-web-common";
import { Provider as MobxProvider } from "mobx-react";
import PeopleStore from "./store/PeopleStore";

const { authStore } = commonStore;
const peopleStore = new PeopleStore();

ReactDOM.render(
  <MobxProvider auth={authStore} peopleStore={peopleStore}>
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
