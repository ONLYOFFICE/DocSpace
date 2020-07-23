import React, { lazy,Suspense } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from './Layout';
import { Loader } from "asc-web-components";

const CommonSettings = lazy(() => import("./categories/common"));
const SecuritySettings = lazy(() => import("./categories/security"));
const CustomizationSettings = lazy(() => import("./categories/common/customization"));
const LanguageAndTimeZoneSettings = lazy(() => import("./categories/common/language-and-time-zone"));

const Settings = () => {
  const basePath = '/settings';
  
  return (
    <Layout key='1'>
      <Suspense fallback={<Loader className="pageLoader" type="rombs" size='40px' />}>
        <Switch>
         
          <Route
            exact
            path={[`${basePath}/common/customization`, `${basePath}/common`, basePath]}
            component={CustomizationSettings}
          />
          <Route
            exact
            path={[`${basePath}/common/customization/language-and-time-zone`]}
            component={LanguageAndTimeZoneSettings}
          />
          <Route
            path={`${basePath}/security`}
            component={SecuritySettings}
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
