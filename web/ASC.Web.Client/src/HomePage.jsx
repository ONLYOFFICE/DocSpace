import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";

import Frame from "./Frame";
import store from "./store/store";

//import "bootstrap/dist/css/bootstrap.min.css";

const HomePage = () => (
  <Provider store={store}>
    <Frame page="home" />
  </Provider>
);

ReactDOM.render(<HomePage />, document.getElementById("app"));
