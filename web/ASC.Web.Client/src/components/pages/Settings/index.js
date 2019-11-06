import React, { lazy } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from './Layout';

const CommonSettings = lazy(() => import("./categories/common"));
const SecuritySettings = lazy(() => import("./categories/security"));


const Settings = () => {

  const basePath = '/settings';
  return (
    <Layout key='1'>
      <Switch>

        <Route
          path={`${basePath}/security`}
          component={SecuritySettings}
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
    </Layout>
  );
};

export default withRouter(Settings);
