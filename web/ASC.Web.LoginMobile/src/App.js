import React from "react";
import { Router, Switch, Redirect, Route } from "react-router-dom";

import { history } from "ASC.Web.Common";

import { Login, Registration, PortalSelection } from "./components";
import "./custom.scss";

const App = () => {
  return (
    <Router history={history}>
      <Switch>
        <Redirect exact from="/" to="/login" />
        <Route exact path="/login" component={Login} />
        <Route exact path="/portal-selection" component={PortalSelection} />
        <Route exact path="/registration" component={Registration} />
      </Switch>
    </Router>
  );
};

export default App;
