import React from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";
import CustomizationSettings from "./customization";
import WhiteLabel from "./whitelabel";

const Common = ({ match }) => {
  const basePath = '/settings/common';

  return (
    <Switch>
      <Route
        exact
        path={[`${basePath}/customization`, '/common', match.path]}
        component={CustomizationSettings}
      />
      <Route
        exact
        path={`${basePath}/whitelabel`}
        component={WhiteLabel}
      />
    </Switch>
  );
};


export default withRouter(Common);
