import React, { lazy, Suspense } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from "./Layout";
import { combineUrl } from "@appserver/common/utils";
import AppServerConfig from "@appserver/common/constants/AppServerConfig";

const SecuritySettings = lazy(() => import("./categories/security/index.js"));
const Admins = lazy(() => import("./categories/security/access-rights/admins"));
const TfaPage = lazy(() => import("./categories/security/access-portal/tfa"));

const CommonSettings = lazy(() => import("./categories/common/index.js"));

const CustomizationSettings = lazy(() =>
  import("./categories/common/customization")
);
const LanguageAndTimeZoneSettings = lazy(() =>
  import("./categories/common/settingsCustomization/language-and-time-zone")
);
const WelcomePageSettings = lazy(() =>
  import("./categories/common/settingsCustomization/welcome-page-settings")
);
const TeamTemplate = lazy(() => import("./categories/common/team-template"));
const ThirdPartyServices = lazy(() =>
  import("./categories/integration/thirdPartyServicesSettings")
);
const DataManagementSettings = lazy(() =>
  import("./categories/data-management/backup")
);
const AutomaticBackup = lazy(() =>
  import("./categories/data-management/backup/auto-backup")
);
const ManualBackup = lazy(() =>
  import("./categories/data-management/backup/manual-backup")
);
const RestoreBackup = lazy(() =>
  import("./categories/data-management/backup/restore-backup")
);

const WhiteLabel = lazy(() => import("./categories/common/whitelabel"));

const PROXY_BASE_URL = combineUrl(AppServerConfig.proxyURL, "/settings");

const COMMON_URLS = [
  combineUrl(PROXY_BASE_URL, "/common/customization"),
  combineUrl(PROXY_BASE_URL, "/common/whitelabel"),
];

const CUSTOMIZATION_URLS = [
  combineUrl(PROXY_BASE_URL, "/common/customization"),
  combineUrl(PROXY_BASE_URL, "/common"),
  PROXY_BASE_URL,
];
const LTZ_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/language-and-time-zone"
);
const WELCOME_PAGE_SETTINGS_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/welcome-page-settings"
);
const TEAM_TEMPLATE_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/team-template"
);
const WHITELABEL_URL = combineUrl(PROXY_BASE_URL, "/common/whitelabel");
const SECURITY_URLS = [
  combineUrl(PROXY_BASE_URL, "/security/access-rights"),
  combineUrl(PROXY_BASE_URL, "/security/access-portal"),
];
const TFA_PAGE_URL = combineUrl(PROXY_BASE_URL, "/security/access-portal/tfa");

const ADMINS_URL = combineUrl(PROXY_BASE_URL, "/security/access-rights/admins");
const THIRD_PARTY_URL = combineUrl(
  PROXY_BASE_URL,
  "/integration/third-party-services"
);
const DATA_MANAGEMENT_URL = combineUrl(
  PROXY_BASE_URL,
  "/datamanagement/backup"
);

const ERROR_404_URL = combineUrl(AppServerConfig.proxyURL, "/error/404");

const Settings = () => {
  return (
    <Layout key="1">
      <Suspense fallback={null}>
        <Switch>
          <Route exact path={COMMON_URLS} component={CommonSettings} />

          {/* <Route
            exact
            path={CUSTOMIZATION_URLS}
            component={CustomizationSettings}
          /> */}
          <Route exact path={LTZ_URL} component={LanguageAndTimeZoneSettings} />
          <Route
            exact
            path={WELCOME_PAGE_SETTINGS_URL}
            component={WelcomePageSettings}
          />
          <Route exact path={WHITELABEL_URL} component={WhiteLabel} />
          <Route exact path={TEAM_TEMPLATE_URL} component={TeamTemplate} />

          <Route exact path={SECURITY_URLS} component={SecuritySettings} />
          <Route path={ADMINS_URL} component={Admins} />
          <Route exact path={TFA_PAGE_URL} component={TfaPage} />

          <Route exact path={THIRD_PARTY_URL} component={ThirdPartyServices} />
          <Route
            exact
            path={DATA_MANAGEMENT_URL}
            component={DataManagementSettings}
          />

          <Redirect
            to={{
              pathname: ERROR_404_URL,
            }}
          />
        </Switch>
      </Suspense>
    </Layout>
  );
};

export default withRouter(Settings);
