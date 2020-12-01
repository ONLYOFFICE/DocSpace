import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import store from "studio/store";
import Frame from "studio/frame";


const LoginPage = () => {
  return (
    <Provider store={store}>
      <Frame page="login" />
    </Provider>
  );
};

ReactDOM.render(<LoginPage />, document.getElementById("app"));
