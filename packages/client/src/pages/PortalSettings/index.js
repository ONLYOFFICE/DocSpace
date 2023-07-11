import React, { lazy, Suspense, useEffect } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import { withRouter } from "react-router";
import Layout from "./Layout";
import { combineUrl } from "@docspace/common/utils";
import Panels from "../../components/FilesPanels";

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

const CustomizationSettings = lazy(() =>
  import("./categories/common/index.js")
);

const LanguageAndTimeZoneSettings = lazy(() =>
  import("./categories/common/Customization/language-and-time-zone")
);
const WelcomePageSettings = lazy(() =>
  import("./categories/common/Customization/welcome-page-settings")
);

const DNSSettings = lazy(() =>
  import("./categories/common/Customization/dns-settings")
);

const PortalRenaming = lazy(() =>
  import("./categories/common/Customization/portal-renaming")
);

const Integration = lazy(() => import("./categories/integration"));
const Payments = lazy(() => import("./categories/payments"));
const ThirdParty = lazy(() =>
  import("./categories/integration/ThirdPartyServicesSettings")
);
const SingleSignOn = lazy(() =>
  import("./categories/integration/SingleSignOn")
);
const SMTPSettings = lazy(() =>
  import("./categories/integration/SMTPSettings")
);
const Backup = lazy(() => import("./categories/data-management/index"));

const RestoreBackup = lazy(() =>
  import("./categories/data-management/backup/restore-backup/index")
);

const Bonus = lazy(() => import("../Bonus"));

const DeleteDataPage = lazy(() => import("./categories/delete-data"));

const WhiteLabel = lazy(() =>
  import("./categories/common/Branding/whitelabel")
);

const DeveloperTools = lazy(() => import("./categories/developer-tools"));

const JavascriptSDK = lazy(() =>
  import("./categories/developer-tools/JavascriptSDK")
);

const Api = lazy(() => import("./categories/developer-tools/Api"));

const Branding = lazy(() => import("./categories/common/branding"));

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const CUSTOMIZATION_URLS = [
  PROXY_BASE_URL,
  combineUrl(PROXY_BASE_URL, "/customization"),
  combineUrl(PROXY_BASE_URL, "/customization/general"),
  combineUrl(PROXY_BASE_URL, "/customization/branding"),
  combineUrl(PROXY_BASE_URL, "/customization/appearance"),
];

const DEVELOPER_URLS = [
  PROXY_BASE_URL,
  combineUrl(PROXY_BASE_URL, "/developer-tools"),
  combineUrl(PROXY_BASE_URL, "/developer-tools/api"),
  combineUrl(PROXY_BASE_URL, "/developer-tools/javascript-sdk"),
];

const API_URLS = combineUrl(PROXY_BASE_URL, "/developer-tools/api");

const SDK_URLS = combineUrl(PROXY_BASE_URL, "/developer-tools/javascript-sdk");

const BACKUP_URLS = [
  PROXY_BASE_URL,
  combineUrl(PROXY_BASE_URL, "/backup"),
  combineUrl(PROXY_BASE_URL, "/backup/data-backup"),
  combineUrl(PROXY_BASE_URL, "/backup/auto-backup"),
];

const RESTORE_DATA_URL = combineUrl(PROXY_BASE_URL, "/restore");

const LTZ_URL = combineUrl(
  PROXY_BASE_URL,
  "/customization/general/language-and-time-zone"
);
const WELCOME_PAGE_SETTINGS_URL = combineUrl(
  PROXY_BASE_URL,
  "/customization/general/welcome-page-settings"
);

const DNS_SETTINGS = combineUrl(
  PROXY_BASE_URL,
  "/customization/general/dns-settings"
);

const PORTAL_RENAMING = combineUrl(
  PROXY_BASE_URL,
  "/customization/general/portal-renaming"
);
const TEAM_TEMPLATE_URL = combineUrl(
  PROXY_BASE_URL,
  "/customization/general/team-template"
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
  combineUrl(PROXY_BASE_URL, "/integration/smtp-settings"),
];

const PAYMENTS_URL = combineUrl(PROXY_BASE_URL, "/payments/portal-payments");

const THIRD_PARTY_URL = combineUrl(
  PROXY_BASE_URL,
  "/integration/third-party-services"
);

const SSO_URL = combineUrl(PROXY_BASE_URL, "/integration/single-sign-on");

const SMTP_Settings = combineUrl(PROXY_BASE_URL, "/integration/smtp-settings");

const bonus_Settings = combineUrl(combineUrl(PROXY_BASE_URL, "/bonus"));

const DELETE_DATA_URLS = [
  combineUrl(PROXY_BASE_URL, "/delete-data/deletion"),
  combineUrl(PROXY_BASE_URL, "/delete-data/deactivation"),
];

const ERROR_404_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/error/404"
);

const Settings = () => {
  return (
    <Layout key="1">
      <Panels />
      <Suspense fallback={null}>
        <Switch>
          <Route
            exact
            path={CUSTOMIZATION_URLS}
            component={CustomizationSettings}
          />
          <Route exact path={LTZ_URL} component={LanguageAndTimeZoneSettings} />
          <Route
            exact
            path={WELCOME_PAGE_SETTINGS_URL}
            component={WelcomePageSettings}
          />
          <Route exact path={DNS_SETTINGS} component={DNSSettings} />
          <Route exact path={PORTAL_RENAMING} component={PortalRenaming} />
          <Route exact path={WHITELABEL_URL} component={WhiteLabel} />
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
          <Route exact path={PAYMENTS_URL} component={Payments} />
          <Route exact path={THIRD_PARTY_URL} component={ThirdParty} />
          <Route exact path={SSO_URL} component={SingleSignOn} />
          <Route exact path={SMTP_Settings} component={SMTPSettings} />
          <Route exact path={DEVELOPER_URLS} component={DeveloperTools} />
          <Route exact path={SDK_URLS} component={JavascriptSDK} />
          <Route exact path={API_URLS} component={Api} />
          <Route exact path={BACKUP_URLS} component={Backup} />
          <Route exact path={DELETE_DATA_URLS} component={DeleteDataPage} />
          <Route path={RESTORE_DATA_URL} component={RestoreBackup} />
          <Route path={bonus_Settings} component={Bonus} />
          <Redirect to={{ pathname: ERROR_404_URL }} />
        </Switch>
      </Suspense>
    </Layout>
  );
};

export default withRouter(Settings);
