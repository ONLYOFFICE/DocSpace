import React, { lazy } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";

const CustomizationSettings = lazy(() => import("./customization"));

const Common = ({ match }) => {
  const basePath = '/settings/common';

  return (
    <Switch>
      <Route
        exact
        path={[`${basePath}/customization`, '/common', match.path]}
        component={CustomizationSettings}
      />

      <Redirect
        to={{
          pathname: "/error/404",
        }}
      />

    </Switch>
  );
};


export default withRouter(Common);
