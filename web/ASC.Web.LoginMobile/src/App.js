import React from "react";
import { Provider } from "react-redux";
import Login from "./Login";
import Header from "./sub-components/header-login-mobile";

import "./custom.scss";

const App = () => {
  return (
    <>
      <Header /> <Login />
    </>
  );
};

export default App;
