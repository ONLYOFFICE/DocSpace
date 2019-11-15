import React, { lazy } from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";

const AccessRightsSettings = lazy(() => import("./accessRights"));

const Security = () => {
  const basePath = "/settings/security";

  return (
    <Switch>
      <Route
        exact
        path={[`${basePath}/accessrights`, basePath, `${basePath}/accessrights/owner`]}
        component={AccessRightsSettings}
        selectedTab={0}
      />
      <Route
        exact
        path={[`${basePath}/accessrights/admins`, basePath]}
        component={AccessRightsSettings}
        selectedTab={1}
      />
      <Route
        exact
        path={[`${basePath}/accessrights/modules`, basePath]}
        component={AccessRightsSettings}
        selectedTab={2}
      />
    </Switch>
  );
};

export default withRouter(Security);
