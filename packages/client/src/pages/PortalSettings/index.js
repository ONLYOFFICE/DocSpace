import React, { lazy } from "react";
import { Route, Routes, Navigate } from "react-router-dom";
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

const DeveloperTools = lazy(() =>
  import("./categories/developer-tools/index.js")
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

const Backup = lazy(() => import("./categories/data-management/index"));

const RestoreBackup = lazy(() =>
  import("./categories/data-management/backup/restore-backup/index")
);
const DeleteDataPage = lazy(() => import("./categories/delete-data"));

const WhiteLabel = lazy(() =>
  import("./categories/common/Branding/whitelabel")
);

const Branding = lazy(() => import("./categories/common/branding"));

const ERROR_404_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/error/404"
);

const Settings = () => {
  return (
    <Layout key="1">
      <Panels />
      <Routes>
        <Route path={"/*"} element={<CustomizationSettings />} />
        <Route path={"/customization/*"} element={<CustomizationSettings />} />
        <Route
          path={"/customization/general/language-and-time-zone"}
          element={<LanguageAndTimeZoneSettings />}
        />
        <Route
          path={"/customization/general/welcome-page-settings"}
          element={<WelcomePageSettings />}
        />

        <Route
          path={"/customization/general/dns-settings"}
          element={<DNSSettings />}
        />
        <Route
          path={"/customization/general/portal-renaming"}
          element={<PortalRenaming />}
        />
        <Route path={"/common/whitelabel"} element={<WhiteLabel />} />
        <Route path={"/security/*"} element={<SecuritySettings />} />
        <Route path={"security/access-portal"} element={<SecuritySettings />} />
        <Route path={"/security/access-portal/tfa"} element={<TfaPage />} />
        <Route
          path={"/security/access-portal/password"}
          element={<PasswordStrengthPage />}
        />
        <Route
          path={"/security/access-portal/trusted-mail"}
          element={<TrustedMailPage />}
        />
        <Route
          path={"/security/access-portal/ip"}
          element={<IpSecurityPage />}
        />
        <Route
          path={"/security/access-portal/admin-message"}
          element={<AdminMessagePage />}
        />
        <Route
          path={"/security/access-portal/lifetime"}
          element={<SessionLifetimePage />}
        />
        <Route path={"/integration/*"} element={<Integration />} />
        <Route path={"/payments/portal-payments"} element={<Payments />} />
        <Route
          path={"/integration/third-party-services"}
          element={<ThirdParty />}
        />
        <Route
          path={"/integration/single-sign-on"}
          element={<SingleSignOn />}
        />
        <Route path={"/developer/*"} element={<DeveloperTools />} />
        <Route path={"/backup/*"} element={<Backup />} />
        <Route path={"/delete-data/*"} element={<DeleteDataPage />} />
        <Route path={"/restore/*"} element={<RestoreBackup />} />
        <Route element={<Navigate to={ERROR_404_URL} replace />} />
      </Routes>
    </Layout>
  );
};

export default Settings;
