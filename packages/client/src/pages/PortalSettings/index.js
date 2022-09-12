import React, { lazy, Suspense, useEffect } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from "./Layout";
import { combineUrl } from "@docspace/common/utils";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import { inject, observer } from "mobx-react";
const SecuritySettings = lazy(() => import("./categories/security/index.js"));

const TfaPage = lazy(() => import("./categories/security/access-portal/tfa"));
const PasswordStrengthPage = lazy(() =>
  import("./categories/security/access-portal/passwordStrength")
);
const TrustedMailPage = lazy(() =>
  import("./categories/security/access-portal/trustedMail")
);
const IpSecurityPage = lazy(() =>
  import("./categories/security/access-portal/ipSecurity")
);
const AdminMessagePage = lazy(() =>
  import("./categories/security/access-portal/adminMessage")
);
const SessionLifetimePage = lazy(() =>
  import("./categories/security/access-portal/sessionLifetime")
);

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

const DNSSettings = lazy(() =>
  import("./categories/common/settingsCustomization/dns-settings")
);

const PortalRenaming = lazy(() =>
  import("./categories/common/settingsCustomization/portal-renaming")
);
const TeamTemplate = lazy(() => import("./categories/common/team-template"));

const Integration = lazy(() => import("./categories/integration"));
const ThirdParty = lazy(() =>
  import("./categories/integration/ThirdPartyServicesSettings")
);
const SingleSignOn = lazy(() =>
  import("./categories/integration/SingleSignOn")
);

const Backup = lazy(() => import("./categories/data-management/backup"));

const RestoreBackup = lazy(() =>
  import("./categories/data-management/backup/restore-backup/index")
);
const WhiteLabel = lazy(() =>
  import("./categories/common/settingsBranding/whitelabel")
);

const Branding = lazy(() => import("./categories/common/branding"));
const PROXY_BASE_URL = combineUrl(AppServerConfig.proxyURL, "/portal-settings");

const COMMON_URLS = [
  PROXY_BASE_URL,
  combineUrl(PROXY_BASE_URL, "/common"),
  combineUrl(PROXY_BASE_URL, "/common/customization"),
  combineUrl(PROXY_BASE_URL, "/common/branding"),
  combineUrl(PROXY_BASE_URL, "/common/appearance"),
];

const CUSTOMIZATION_URLS = [
  combineUrl(PROXY_BASE_URL, "/common/customization"),
  combineUrl(PROXY_BASE_URL, "/common"),
  PROXY_BASE_URL,
];

const BACKUP_URLS = [
  PROXY_BASE_URL,
  combineUrl(PROXY_BASE_URL, "/backup"),
  combineUrl(PROXY_BASE_URL, "/backup/data-backup"),
  combineUrl(PROXY_BASE_URL, "/backup/auto-backup"),
];

const RESTORE_DATA_URL = combineUrl(PROXY_BASE_URL, "/restore");

const LTZ_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/language-and-time-zone"
);
const WELCOME_PAGE_SETTINGS_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/welcome-page-settings"
);

const DNS_SETTINGS = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/dns-settings"
);

const PORTAL_RENAMING = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/portal-renaming"
);
const TEAM_TEMPLATE_URL = combineUrl(
  PROXY_BASE_URL,
  "/common/customization/team-template"
);
const WHITELABEL_URL = combineUrl(PROXY_BASE_URL, "/common/whitelabel");
const SECURITY_URLS = [
  combineUrl(PROXY_BASE_URL, "/security/access-rights"),
  combineUrl(PROXY_BASE_URL, "/security/access-portal"),
  combineUrl(PROXY_BASE_URL, "/security/login-history"),
  combineUrl(PROXY_BASE_URL, "/security/audit-trail"),
];
const TFA_PAGE_URL = combineUrl(PROXY_BASE_URL, "/security/access-portal/tfa");
const PASSWORD_PAGE_URL = combineUrl(
  PROXY_BASE_URL,
  "/security/access-portal/password"
);
const TRUSTED_MAIL_PAGE_URL = combineUrl(
  PROXY_BASE_URL,
  "/security/access-portal/trusted-mail"
);
const IP_SECURITY_PAGE_URL = combineUrl(
  PROXY_BASE_URL,
  "/security/access-portal/ip"
);
const ADMIN_MESSAGE_PAGE_URL = combineUrl(
  PROXY_BASE_URL,
  "/security/access-portal/admin-message"
);
const SESSION_LIFETIME_PAGE_URL = combineUrl(
  PROXY_BASE_URL,
  "/security/access-portal/lifetime"
);

const ADMINS_URL = combineUrl(PROXY_BASE_URL, "/security/access-rights/admins");

const INTEGRATION_URLS = [
  combineUrl(PROXY_BASE_URL, "/integration/third-party-services"),
  combineUrl(PROXY_BASE_URL, "/integration/single-sign-on"),
  combineUrl(PROXY_BASE_URL, "/integration/portal-integration"),
  combineUrl(PROXY_BASE_URL, "/integration/plugins"),
];

const THIRD_PARTY_URL = combineUrl(
  PROXY_BASE_URL,
  "/integration/third-party-services"
);

const SSO_URL = combineUrl(PROXY_BASE_URL, "/integration/single-sign-on");

const ERROR_404_URL = combineUrl(AppServerConfig.proxyURL, "/error/404");

const Settings = (props) => {
  const { loadBaseInfo } = props;

  useEffect(() => {
    loadBaseInfo();
  }, []);

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
          <Route exact path={DNS_SETTINGS} component={DNSSettings} />
          <Route exact path={PORTAL_RENAMING} component={PortalRenaming} />
          <Route exact path={WHITELABEL_URL} component={WhiteLabel} />
          <Route exact path={TEAM_TEMPLATE_URL} component={TeamTemplate} />

          <Route exact path={SECURITY_URLS} component={SecuritySettings} />

          <Route exact path={TFA_PAGE_URL} component={TfaPage} />
          <Route
            exact
            path={PASSWORD_PAGE_URL}
            component={PasswordStrengthPage}
          />
          <Route
            exact
            path={TRUSTED_MAIL_PAGE_URL}
            component={TrustedMailPage}
          />
          <Route exact path={IP_SECURITY_PAGE_URL} component={IpSecurityPage} />
          <Route
            exact
            path={ADMIN_MESSAGE_PAGE_URL}
            component={AdminMessagePage}
          />
          <Route
            exact
            path={SESSION_LIFETIME_PAGE_URL}
            component={SessionLifetimePage}
          />

          <Route exact path={INTEGRATION_URLS} component={Integration} />

          <Route exact path={THIRD_PARTY_URL} component={ThirdParty} />
          <Route exact path={SSO_URL} component={SingleSignOn} />
          <Route exact path={BACKUP_URLS} component={Backup} />
          <Route path={RESTORE_DATA_URL} component={RestoreBackup} />
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

export default inject(({ common }) => {
  return {
    loadBaseInfo: async () => {
      await common.initSettings();
    },
  };
})(withRouter(observer(Settings)));
