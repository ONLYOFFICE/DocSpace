import React from "react";
import { createBrowserRouter, Navigate } from "react-router-dom";
import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";
import PublicRoute from "@docspace/common/components/PublicRoute";
import ConfirmRoute from "./helpers/confirmRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import AppLoader from "@docspace/common/components/AppLoader";

import Root from "./Shell";
const Client = loadable(() => import("./Client"));
const PortalSettings = loadable(() => import("./pages/PortalSettings"));

const Home = loadable(() => import("./pages/Home"));
const AccountsHome = loadable(() => import("./pages/AccountsHome"));
const Settings = loadable(() => import("./pages/Settings"));
const Profile = loadable(() => import("./pages/Profile"));
const NotificationComponent = loadable(() => import("./pages/Notifications"));
const Confirm = loadable(() => import("./pages/Confirm"));

const CustomizationSettings = loadable(() =>
  import("./pages/PortalSettings/categories/common/index.js")
);
const LanguageAndTimeZoneSettings = loadable(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/language-and-time-zone"
  )
);
const WelcomePageSettings = loadable(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/welcome-page-settings"
  )
);
const DNSSettings = loadable(() =>
  import("./pages/PortalSettings/categories/common/Customization/dns-settings")
);
const PortalRenaming = loadable(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/portal-renaming"
  )
);
const WhiteLabel = loadable(() =>
  import("./pages/PortalSettings/categories/common/Branding/whitelabel")
);
const SecuritySettings = loadable(() =>
  import("./pages/PortalSettings/categories/security/index.js")
);
const TfaPage = loadable(() =>
  import("./pages/PortalSettings/categories/security/access-portal/tfa")
);
const PasswordStrengthPage = loadable(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/passwordStrength"
  )
);
const TrustedMailPage = loadable(() =>
  import("./pages/PortalSettings/categories/security/access-portal/trustedMail")
);
const IpSecurityPage = loadable(() =>
  import("./pages/PortalSettings/categories/security/access-portal/ipSecurity")
);
const AdminMessagePage = loadable(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/adminMessage"
  )
);
const SessionLifetimePage = loadable(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/sessionLifetime"
  )
);
const Integration = loadable(() =>
  import("./pages/PortalSettings/categories/integration")
);
const Payments = loadable(() =>
  import("./pages/PortalSettings/categories/payments")
);
const ThirdParty = loadable(() =>
  import(
    "./pages/PortalSettings/categories/integration/ThirdPartyServicesSettings"
  )
);
const SingleSignOn = loadable(() =>
  import("./pages/PortalSettings/categories/integration/SingleSignOn")
);
const DeveloperTools = loadable(() =>
  import("./pages/PortalSettings/categories/developer-tools/index.js")
);
const Backup = loadable(() =>
  import("./pages/PortalSettings/categories/data-management/index")
);
const DeleteDataPage = loadable(() =>
  import("./pages/PortalSettings/categories/delete-data")
);
const RestoreBackup = loadable(() =>
  import(
    "./pages/PortalSettings/categories/data-management/backup/restore-backup/index"
  )
);

const FormGallery = loadable(() => import("./pages/FormGallery"));
const About = loadable(() => import("./pages/About"));
const Wizard = loadable(() => import("./pages/Wizard"));
const PreparationPortal = loadable(() => import("./pages/PreparationPortal"));
const PortalUnavailable = loadable(() => import("./pages/PortalUnavailable"));
const ErrorUnavailable = loadable(() => import("./pages/Errors/Unavailable"));

const ActivateUserForm = loadable(() =>
  import("./pages/Confirm/sub-components/activateUser")
);
const CreateUserForm = loadable(() =>
  import("./pages/Confirm/sub-components/createUser")
);
const ChangePasswordForm = loadable(() =>
  import("./pages/Confirm/sub-components/changePassword")
);
const ActivateEmailForm = loadable(() =>
  import("./pages/Confirm/sub-components/activateEmail")
);
const ChangeEmailForm = loadable(() =>
  import("./pages/Confirm/sub-components/changeEmail")
);
const ChangePhoneForm = loadable(() =>
  import("./pages/Confirm/sub-components/changePhone")
);
const ProfileRemoveForm = loadable(() =>
  import("./pages/Confirm/sub-components/profileRemove")
);
const ChangeOwnerForm = loadable(() =>
  import("./pages/Confirm/sub-components/changeOwner")
);
const TfaAuthForm = loadable(() =>
  import("./pages/Confirm/sub-components/tfaAuth")
);
const TfaActivationForm = loadable(() =>
  import("./pages/Confirm/sub-components/tfaActivation")
);
const RemovePortal = loadable(() =>
  import("./pages/Confirm/sub-components/removePortal")
);
const DeactivatePortal = loadable(() =>
  import("./pages/Confirm/sub-components/deactivatePortal")
);
const ContinuePortal = loadable(() =>
  import("./pages/Confirm/sub-components/continuePortal")
);
const Auth = loadable(() => import("./pages/Confirm/sub-components/auth"));

const Error404 = loadable(() => import("client/Error404"));
const Error401 = loadable(() => import("client/Error401"));

const confirmRoutes = [
  {
    path: "LinkInvite",
    element: (
      <ConfirmRoute forUnauthorized>
        <CreateUserForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "Activation",
    element: (
      <ConfirmRoute forUnauthorized>
        <ActivateUserForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "EmailActivation",
    element: (
      <ConfirmRoute>
        <ActivateEmailForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "EmailChange",
    element: (
      <ConfirmRoute>
        <ChangeEmailForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "PasswordChange",
    element: (
      <ConfirmRoute forUnauthorized>
        <ChangePasswordForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "ProfileRemove",
    element: (
      <ConfirmRoute>
        <ProfileRemoveForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "PhoneActivation",
    element: (
      <ConfirmRoute>
        <ChangePhoneForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "PortalOwnerChange",
    element: (
      <ConfirmRoute>
        <ChangeOwnerForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "TfaAuth",
    element: (
      <ConfirmRoute>
        <TfaAuthForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "TfaActivation",
    element: (
      <ConfirmRoute>
        <TfaActivationForm />
      </ConfirmRoute>
    ),
  },
  {
    path: "PortalRemove",
    element: (
      <ConfirmRoute>
        <RemovePortal />
      </ConfirmRoute>
    ),
  },
  {
    path: "PortalSuspend",
    element: (
      <ConfirmRoute>
        <DeactivatePortal />
      </ConfirmRoute>
    ),
  },
  {
    path: "PortalContinue",
    element: (
      <ConfirmRoute>
        <ContinuePortal />
      </ConfirmRoute>
    ),
  },
  {
    path: "Auth",
    element: (
      <ConfirmRoute forUnauthorized>
        <Auth />
      </ConfirmRoute>
    ),
  },
];

const NotFoundError = () => {
  return (
    <PrivateRoute>
      <ErrorBoundary>
        <Error404 />
      </ErrorBoundary>
    </PrivateRoute>
  );
};

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    errorElement: <NotFoundError />,
    children: [
      {
        path: "/",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <Client />
            </ErrorBoundary>
          </PrivateRoute>
        ),
        errorElement: <NotFoundError />,
        children: [
          {
            index: true,
            element: (
              <PrivateRoute>
                <Navigate to="/rooms/shared" replace />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms",
            element: (
              <PrivateRoute>
                <Navigate to="/rooms/shared" replace />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/personal",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/personal/filter",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "files/trash",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "files/trash/filter",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/shared",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/shared/filter",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/archived",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/archived/filter",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "products/files",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "accounts",
            element: (
              <PrivateRoute restricted withManager>
                <Navigate to="/accounts/filter" replace />
              </PrivateRoute>
            ),
          },
          {
            path: "accounts/filter",
            element: (
              <PrivateRoute restricted withManager>
                <AccountsHome />
              </PrivateRoute>
            ),
          },
          {
            path: "accounts/view/@self",
            element: (
              <PrivateRoute>
                <Profile />
              </PrivateRoute>
            ),
          },
          {
            path: "accounts/view/@self/notification",
            element: (
              <PrivateRoute>
                <NotificationComponent />
              </PrivateRoute>
            ),
          },
          {
            path: "settings",
            element: (
              <PrivateRoute withCollaborator restricted>
                <Navigate to="/settings/common" replace />
              </PrivateRoute>
            ),
          },
          {
            path: "settings/common",
            element: (
              <PrivateRoute withCollaborator restricted>
                <Settings />
              </PrivateRoute>
            ),
          },
          {
            path: "settings/admin",
            element: (
              <PrivateRoute withCollaborator restricted>
                <Settings />
              </PrivateRoute>
            ),
          },
        ],
      },
      {
        path: "portal-settings/",
        element: (
          <PrivateRoute restricted>
            <ErrorBoundary>
              <PortalSettings />
            </ErrorBoundary>
          </PrivateRoute>
        ),
        errorElement: <NotFoundError />,
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
          // {
          //   path: "common/whitelabel",
          //   element: <WhiteLabel />,
          // },
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
            element: <Navigate to="integration/third-party-services" />,
          },
          {
            path: "integration/third-party-services",
            element: <ThirdParty />,
          },
          {
            path: "integration/single-sign-on",
            element: <SingleSignOn />,
          },
          {
            path: "payments/portal-payments",
            element: <Payments />,
          },
          {
            path: "developer",
            element: <Navigate to="developer/tools" />,
          },
          {
            path: "developer/tools",
            element: <DeveloperTools />,
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
        ],
      },
      {
        path: "/Products/Files/",
        caseSensitive: true,
        element: <Navigate to="/rooms/shared" replace />,
      },
      {
        path: "/form-gallery/:folderId",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <FormGallery />
            </ErrorBoundary>
          </PrivateRoute>
        ),
      },
      {
        path: "/wizard",
        element: (
          <PublicRoute>
            <ErrorBoundary>
              <Wizard />
            </ErrorBoundary>
          </PublicRoute>
        ),
      },
      {
        path: "/about",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <About />
            </ErrorBoundary>
          </PrivateRoute>
        ),
      },
      {
        path: "confirm.aspx",
        element: (
          <ErrorBoundary>
            <Confirm />
          </ErrorBoundary>
        ),
        errorElement: <NotFoundError />,
        children: [...confirmRoutes],
      },
      {
        path: "confirm",
        element: (
          <ErrorBoundary>
            <Confirm />
          </ErrorBoundary>
        ),
        errorElement: <NotFoundError />,
        children: [...confirmRoutes],
      },
      {
        path: "/portal-unavailable",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <PortalUnavailable />
            </ErrorBoundary>
          </PrivateRoute>
        ),
      },
      {
        path: "/unavailable",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <ErrorUnavailable />
            </ErrorBoundary>
          </PrivateRoute>
        ),
      },
      {
        path: "/preparation-portal",
        element: (
          <PublicRoute>
            <ErrorBoundary>
              <PreparationPortal />
            </ErrorBoundary>
          </PublicRoute>
        ),
      },
      {
        path: "/error401",
        element: (
          <PrivateRoute>
            <ErrorBoundary>
              <Error401 />
            </ErrorBoundary>
          </PrivateRoute>
        ),
      },
    ],
  },
]);

export default router;
