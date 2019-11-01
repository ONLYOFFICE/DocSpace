import React, { lazy } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";

const AccessRightsSettings = lazy(() => import("./accessRights"));

const Security = () => {
  const basePath = '/settings/security';

  return (
    <Switch>
      <Route
        exact
        path={[`${basePath}/access-rights`, basePath]}
        component={AccessRightsSettings}
      />

      <Redirect
        to={{
          pathname: "/error/404",
        }}
      />

    </Switch>
  );
};


export default withRouter(Security);
