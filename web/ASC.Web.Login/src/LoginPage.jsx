import React from "react";
import ReactDOM from "react-dom";

import LoginContent from "./LoginContent.jsx";

const LoginPage = () => {
  return (
    <LoginContent />
    //   <Provider store={store}>
    //     <Frame page="checkout" />
    //   </Provider>
  );
};

ReactDOM.render(<LoginPage />, document.getElementById("app"));
