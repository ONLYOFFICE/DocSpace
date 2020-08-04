import React, { lazy, Suspense } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from './Layout';
import { Loader } from "asc-web-components";

const CommonSettings = lazy(() => import("./categories/common"));
const SecuritySettings = lazy(() => import("./categories/security"));
const IntegrationSettings = lazy(() => import("./categories/integration"));

const Settings = () => {
  const basePath = '/settings';

  return (
    <Layout key='1'>
      <Suspense fallback={<Loader className="pageLoader" type="rombs" size='40px' />}>
        <Switch>
          <Route
            path={`${basePath}/security`}
            component={SecuritySettings}
          />
          <Route
            path={`${basePath}/integration`}
            component={IntegrationSettings}
          />
          <Route
            path={[`${basePath}/common`, basePath]}
            component={CommonSettings}
          />
          <Redirect
            to={{
              pathname: "/error/404",
            }}
          />
        </Switch>
      </Suspense>
    </Layout>
  );
};

export default withRouter(Settings);
