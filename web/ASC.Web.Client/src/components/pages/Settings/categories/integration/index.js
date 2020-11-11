import React, { lazy, Suspense } from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";
import { Loader } from "asc-web-components";

const ThirdPartyServices = lazy(() => import("./thirdPartyServicesSettings"));

const Integration = ({ match }) => {
  const basePath = "/settings/integration";

  return (
    <Suspense
      fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
    >
      <Switch>
        <Route
          exact
          path={[
            `${basePath}/third-party-services`,
            "/integration",
            match.path,
          ]}
          component={ThirdPartyServices}
        />
      </Switch>
    </Suspense>
  );
};

export default withRouter(Integration);
