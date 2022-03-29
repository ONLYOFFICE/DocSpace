import React, { lazy, Suspense } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from "./Layout";
import { combineUrl } from "@appserver/common/utils";
import AppServerConfig from "@appserver/common/constants/AppServerConfig";

const SecuritySettings = lazy(() =>
  import("./categories/security/access-rights")
);
const Admins = lazy(() =>
  import("./categories/security/sub-components/admins")
);

const AccessPortal = lazy(() => import("./categories/security/access-portal"));
const TfaPage = lazy(() => import("./categories/security/sub-components/tfa"));

const CustomizationSettings = lazy(() =>
  import("./categories/common/customization")
);
const LanguageAndTimeZoneSettings = lazy(() =>
  import("./categories/common/language-and-time-zone")
);
const CustomTitles = lazy(() => import("./categories/common/custom-titles"));
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
//const WhiteLabel = lazy(() => import("./categories/common/whitelabel"));
const PROXY_BASE_URL = combineUrl(AppServerConfig.proxyURL, "/settings");

const CUSTOMIZATION_URLS = [
  combineUrl(PROXY_BASE_URL, "/common/customization"),
  combineUrl(PROXY_BASE_URL, "/common"),
  PROXY_BASE_URL,
];
const LTZ_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/language-and-time-zone"
);
const CUSTOM_TITLE_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/custom-titles"
);
const TEAM_TEMPLATE_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/team-template"
);
//const WHITELABEL_URL = combineUrl(PROXY_BASE_URL, "/common/whitelabel");
const SECURITY_URL = combineUrl(PROXY_BASE_URL, "/security/access-rights");
const ACCESS_PORTAL_URL = combineUrl(PROXY_BASE_URL, "/security/access-portal");
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
          <Route
            exact
            path={CUSTOMIZATION_URLS}
            component={CustomizationSettings}
          />
          <Route exact path={LTZ_URL} component={LanguageAndTimeZoneSettings} />
          <Route exact path={CUSTOM_TITLE_URL} component={CustomTitles} />
          <Route exact path={TEAM_TEMPLATE_URL} component={TeamTemplate} />
          {/* <Route
            exact
            path={WHITELABEL_URL}
            component={WhiteLabel}
          /> */}
          <Route exact path={SECURITY_URL} component={SecuritySettings} />
          <Route path={ADMINS_URL} component={Admins} />

          <Route exact path={ACCESS_PORTAL_URL} component={AccessPortal} />
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
