import React, { lazy, Suspense } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from "./Layout";

const SecuritySettings = lazy(() => import("./categories/security"));
const CustomizationSettings = lazy(() =>
  import("./categories/common/customization")
);
const LanguageAndTimeZoneSettings = lazy(() =>
  import("./categories/common/language-and-time-zone")
);
const CustomTitles = lazy(() => import("./categories/common/custom-titles"));
const ThirdPartyServices = lazy(() =>
  import("./categories/integration/thirdPartyServicesSettings")
);

//const WhiteLabel = lazy(() => import("./categories/common/whitelabel"));

const Settings = () => {
  const basePath = "/settings";

  return (
    <Layout key="1">
      <Suspense fallback={null}>
        <Switch>
          <Route
            exact
            path={[
              `${basePath}/common/customization`,
              `${basePath}/common`,
              basePath,
            ]}
            component={CustomizationSettings}
          />
          <Route
            exact
            path={[`${basePath}/common/customization/language-and-time-zone`]}
            component={LanguageAndTimeZoneSettings}
          />
          <Route
            exact
            path={[`${basePath}/common/customization/custom-titles`]}
            component={CustomTitles}
          />
          {/* <Route
            exact
            path={`${basePath}/common/whitelabel`}
            component={WhiteLabel}
          /> */}
          <Route path={`${basePath}/security`} component={SecuritySettings} />
          <Route
            exact
            path={`${basePath}/integration/third-party-services`}
            component={ThirdPartyServices}
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
