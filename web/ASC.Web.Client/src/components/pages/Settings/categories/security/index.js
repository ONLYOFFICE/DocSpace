import React, { lazy, Suspense } from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";
import { combineUrl } from "@appserver/common/utils";
import AppServerConfig from "@appserver/common/constants/AppServerConfig";

const AccessRightsSettings = lazy(() => import("./accessRights"));

const PROXY_BASE_URL = combineUrl(
  AppServerConfig.proxyURL,
  "/settings/security"
);
const ACCESS_RIGHTS_URLS = [
  combineUrl(PROXY_BASE_URL, "/accessrights"),
  combineUrl(PROXY_BASE_URL, "/accessrights/owner"),
];
const ADMINS_URLS = [
  combineUrl(PROXY_BASE_URL, "/accessrights/admins"),
  PROXY_BASE_URL,
];
const MODULES_URLS = [
  combineUrl(PROXY_BASE_URL, "/accessrights/modules"),
  PROXY_BASE_URL,
];

const Security = () => {
  return (
    <Suspense fallback={null}>
      <Switch>
        <Route
          exact
          path={ACCESS_RIGHTS_URLS}
          component={AccessRightsSettings}
          selectedTab={0}
        />
        <Route
          exact
          path={ADMINS_URLS}
          component={AccessRightsSettings}
          selectedTab={1}
        />
        {/* <Route
          exact
          path={MODULES_URLS}
          component={AccessRightsSettings}
          selectedTab={2}
        /> */}
      </Switch>
    </Suspense>
  );
};

export default withRouter(Security);
