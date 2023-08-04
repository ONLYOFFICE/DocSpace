import React from "react";
import { Navigate } from "react-router-dom";
import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";

import Error404 from "SRC_DIR/pages/Errors/404";

import { generalRoutes } from "./general";

const PortalSettings = loadable(() => import("../pages/PortalSettings"));

const CustomizationSettings = loadable(() =>
  import("../pages/PortalSettings/categories/common/index.js")
);
const LanguageAndTimeZoneSettings = loadable(() =>
  import(
    "../pages/PortalSettings/categories/common/Customization/language-and-time-zone"
  )
);
const WelcomePageSettings = loadable(() =>
  import(
    "../pages/PortalSettings/categories/common/Customization/welcome-page-settings"
  )
);
const DNSSettings = loadable(() =>
  import("../pages/PortalSettings/categories/common/Customization/dns-settings")
);
const PortalRenaming = loadable(() =>
  import(
    "../pages/PortalSettings/categories/common/Customization/portal-renaming"
  )
);
const WhiteLabel = loadable(() =>
  import("../pages/PortalSettings/categories/common/Branding/whitelabel")
);
const SecuritySettings = loadable(() =>
  import("../pages/PortalSettings/categories/security/index.js")
);
const TfaPage = loadable(() =>
  import("../pages/PortalSettings/categories/security/access-portal/tfa")
);
const PasswordStrengthPage = loadable(() =>
  import(
    "../pages/PortalSettings/categories/security/access-portal/passwordStrength"
  )
);
const TrustedMailPage = loadable(() =>
  import(
    "../pages/PortalSettings/categories/security/access-portal/trustedMail"
  )
);
const IpSecurityPage = loadable(() =>
  import("../pages/PortalSettings/categories/security/access-portal/ipSecurity")
);
const AdminMessagePage = loadable(() =>
  import(
    "../pages/PortalSettings/categories/security/access-portal/adminMessage"
  )
);
const SessionLifetimePage = loadable(() =>
  import(
    "../pages/PortalSettings/categories/security/access-portal/sessionLifetime"
  )
);
const Integration = loadable(() =>
  import("../pages/PortalSettings/categories/integration")
);
const Payments = loadable(() =>
  import("../pages/PortalSettings/categories/payments")
);
const ThirdParty = loadable(() =>
  import(
    "../pages/PortalSettings/categories/integration/ThirdPartyServicesSettings"
  )
);
const SingleSignOn = loadable(() =>
  import("../pages/PortalSettings/categories/integration/SingleSignOn")
);
const DeveloperTools = loadable(() =>
  import("../pages/PortalSettings/categories/developer-tools/index.js")
);
const WebhookHistory = loadable(() =>
  import(
    "../pages/PortalSettings/categories/developer-tools/Webhooks/WebhookHistory"
  )
);
const WebhookDetails = loadable(() =>
  import(
    "../pages/PortalSettings/categories/developer-tools/Webhooks/WebhookEventDetails"
  )
);
const Backup = loadable(() =>
  import("../pages/PortalSettings/categories/data-management/index")
);
const DeleteDataPage = loadable(() =>
  import("../pages/PortalSettings/categories/delete-data")
);
const RestoreBackup = loadable(() =>
  import(
    "../pages/PortalSettings/categories/data-management/backup/restore-backup/index"
  )
);
const Bonus = loadable(() => import("../pages/Bonus"));

const PortalSettingsRoutes = {
  path: "portal-settings/",
  element: (
    <PrivateRoute restricted>
      <ErrorBoundary>
        <PortalSettings />
      </ErrorBoundary>
    </PrivateRoute>
  ),
  errorElement: <Error404 />,
  children: [
    {
      index: true,
      element: <Navigate to="customization/general" />,
    },
    {
      path: "customization",
      element: <Navigate to="customization/general" />,
    },
    {
      path: "customization/general",
      element: <CustomizationSettings />,
    },
    {
      path: "customization/branding",
      element: <CustomizationSettings />,
    },
    {
      path: "customization/appearance",
      element: <CustomizationSettings />,
    },
    {
      path: "customization/general/language-and-time-zone",
      element: <LanguageAndTimeZoneSettings />,
    },
    {
      path: "customization/general/welcome-page-settings",
      element: <WelcomePageSettings />,
    },
    {
      path: "customization/general/dns-settings",
      element: <DNSSettings />,
    },
    {
      path: "customization/general/portal-renaming",
      element: <PortalRenaming />,
    },
    {
      path: "common/whitelabel",
      element: <WhiteLabel />,
    },
    {
      path: "security",
      element: <Navigate to="security/access-portal" />,
    },
    {
      path: "security/access-portal",
      element: <SecuritySettings />,
    },
    {
      path: "security/login-history",
      element: <SecuritySettings />,
    },
    {
      path: "security/audit-trail",
      element: <SecuritySettings />,
    },
    {
      path: "security/access-portal/tfa",
      element: <TfaPage />,
    },
    {
      path: "security/access-portal/password",
      element: <PasswordStrengthPage />,
    },
    {
      path: "security/access-portal/trusted-mail",
      element: <TrustedMailPage />,
    },
    {
      path: "security/access-portal/ip",
      element: <IpSecurityPage />,
    },
    {
      path: "security/access-portal/admin-message",
      element: <AdminMessagePage />,
    },
    {
      path: "security/access-portal/lifetime",
      element: <SessionLifetimePage />,
    },
    {
      path: "integration",
      element: <Integration />,
    },
    {
      path: "integration/third-party-services",
      element: <Integration />,
    },
    {
      path: "integration/single-sign-on",
      element: <Integration />,
    },
    {
      path: "integration/portal-integration",
      element: <Integration />,
    },
    {
      path: "integration/smtp-settings",
      element: <Integration />,
    },
    {
      path: "payments/portal-payments",
      element: <Payments />,
    },
    {
      path: "developer-tools",
      element: <Navigate to="javascript-sdk" />,
    },
    {
      path: "developer-tools/api",
      element: <DeveloperTools />,
    },
    {
      path: "developer-tools/javascript-sdk",
      element: <DeveloperTools />,
    },
    {
      path: "developer-tools/webhooks",
      element: <DeveloperTools />,
    },
    {
      path: "developer-tools/webhooks/:id",
      element: <WebhookHistory />,
    },
    {
      path: "developer-tools/webhooks/:id/:eventId",
      element: <WebhookDetails />,
    },
    {
      path: "backup",
      element: <Navigate to="backup/data-backup" />,
    },
    {
      path: "backup/data-backup",
      element: <Backup />,
    },
    {
      path: "backup/auto-backup",
      element: <Backup />,
    },
    {
      path: "delete-data",
      element: <Navigate to="delete-data/deletion" />,
    },
    {
      path: "delete-data/deletion",
      element: <DeleteDataPage />,
    },
    {
      path: "delete-data/deactivation",
      element: <DeleteDataPage />,
    },
    {
      path: "restore",
      element: <Navigate to="restore/restore-backup" />,
    },
    {
      path: "restore/restore-backup",
      element: <RestoreBackup />,
    },
    {
      path: "bonus",
      element: <Bonus />,
    },
    ...generalRoutes,
  ],
};

export default PortalSettingsRoutes;
