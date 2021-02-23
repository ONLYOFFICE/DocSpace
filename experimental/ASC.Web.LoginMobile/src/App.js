import React from "react";
import { Router, Switch, Redirect, Route } from "react-router-dom";

import { history, Layout, Main } from "ASC.Web.Common";

import Header from "./components/Header";
import { Login, Registration, PortalSelection } from "./components";
import "./custom.scss";

const App = () => {
  return (
    <>
      <Header />
      <Main>
        <Router history={history}>
          <Switch>
            <Route exact path="/login" component={Login} />
            <Route exact path="/portal-selection" component={PortalSelection} />
            <Route exact path="/registration" component={Registration} />
            <Redirect from="/" to="/login" />
          </Switch>
        </Router>
      </Main>
    </>
  );
};

export default App;
