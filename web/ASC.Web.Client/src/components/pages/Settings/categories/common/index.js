import React, { lazy, Suspense } from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";
import { Loader } from "asc-web-components";

const CustomizationSettings = lazy(() => import("./customization"));
// const WhiteLabel = lazy(() => import("./whitelabel"));

const Common = ({ match }) => {
  const basePath = '/settings/common';

  return (
    <Suspense fallback={<Loader className="pageLoader" type="rombs" size='40px' />}>
      <Switch>
        <Route
          exact
          path={[`${basePath}/customization`, '/common', match.path]}
          component={CustomizationSettings}
        />
        {/* <Route
          exact
          path={`${basePath}/whitelabel`}
          component={WhiteLabel}
        /> */}
      </Switch>
    </Suspense>
  );
};


export default withRouter(Common);
