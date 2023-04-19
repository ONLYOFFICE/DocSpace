import React from "react";
import { createBrowserRouter, Navigate } from "react-router-dom";

import PrivateRoute from "@docspace/common/components/PrivateRoute";
import PublicRoute from "@docspace/common/components/PublicRoute";
import ConfirmRoute from "./helpers/confirmRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import AppLoader from "@docspace/common/components/AppLoader";

import Root from "./Shell";
const Client = React.lazy(() => import("./Client"));
const PortalSettings = React.lazy(() => import("./pages/PortalSettings"));

const Home = React.lazy(() => import("./pages/Home"));
const AccountsHome = React.lazy(() => import("./pages/AccountsHome"));
const Settings = React.lazy(() => import("./pages/Settings"));
const Profile = React.lazy(() => import("./pages/Profile"));
const NotificationComponent = React.lazy(() => import("./pages/Notifications"));
const Confirm = React.lazy(() => import("./pages/Confirm"));

const CustomizationSettings = React.lazy(() =>
  import("./pages/PortalSettings/categories/common/index.js")
);
const LanguageAndTimeZoneSettings = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/language-and-time-zone"
  )
);
const WelcomePageSettings = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/welcome-page-settings"
  )
);
const DNSSettings = React.lazy(() =>
  import("./pages/PortalSettings/categories/common/Customization/dns-settings")
);
const PortalRenaming = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/common/Customization/portal-renaming"
  )
);
const WhiteLabel = React.lazy(() =>
  import("./pages/PortalSettings/categories/common/Branding/whitelabel")
);
const SecuritySettings = React.lazy(() =>
  import("./pages/PortalSettings/categories/security/index.js")
);
const TfaPage = React.lazy(() =>
  import("./pages/PortalSettings/categories/security/access-portal/tfa")
);
const PasswordStrengthPage = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/passwordStrength"
  )
);
const TrustedMailPage = React.lazy(() =>
  import("./pages/PortalSettings/categories/security/access-portal/trustedMail")
);
const IpSecurityPage = React.lazy(() =>
  import("./pages/PortalSettings/categories/security/access-portal/ipSecurity")
);
const AdminMessagePage = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/adminMessage"
  )
);
const SessionLifetimePage = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/security/access-portal/sessionLifetime"
  )
);
const Integration = React.lazy(() =>
  import("./pages/PortalSettings/categories/integration")
);
const Payments = React.lazy(() =>
  import("./pages/PortalSettings/categories/payments")
);
const ThirdParty = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/integration/ThirdPartyServicesSettings"
  )
);
const SingleSignOn = React.lazy(() =>
  import("./pages/PortalSettings/categories/integration/SingleSignOn")
);
const DeveloperTools = React.lazy(() =>
  import("./pages/PortalSettings/categories/developer-tools/index.js")
);
const Backup = React.lazy(() =>
  import("./pages/PortalSettings/categories/data-management/index")
);
const DeleteDataPage = React.lazy(() =>
  import("./pages/PortalSettings/categories/delete-data")
);
const RestoreBackup = React.lazy(() =>
  import(
    "./pages/PortalSettings/categories/data-management/backup/restore-backup/index"
  )
);

const FormGallery = React.lazy(() => import("./pages/FormGallery"));
const About = React.lazy(() => import("./pages/About"));
const Wizard = React.lazy(() => import("./pages/Wizard"));
const PreparationPortal = React.lazy(() => import("./pages/PreparationPortal"));
const PortalUnavailable = React.lazy(() => import("./pages/PortalUnavailable"));
const ErrorUnavailable = React.lazy(() => import("./pages/Errors/Unavailable"));

const ActivateUserForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/activateUser")
);
const CreateUserForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/createUser")
);
const ChangePasswordForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/changePassword")
);
const ActivateEmailForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/activateEmail")
);
const ChangeEmailForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/changeEmail")
);
const ChangePhoneForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/changePhone")
);
const ProfileRemoveForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/profileRemove")
);
const ChangeOwnerForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/changeOwner")
);
const TfaAuthForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/tfaAuth")
);
const TfaActivationForm = React.lazy(() =>
  import("./pages/Confirm/sub-components/tfaActivation")
);
const RemovePortal = React.lazy(() =>
  import("./pages/Confirm/sub-components/removePortal")
);
const DeactivatePortal = React.lazy(() =>
  import("./pages/Confirm/sub-components/deactivatePortal")
);
const ContinuePortal = React.lazy(() =>
  import("./pages/Confirm/sub-components/continuePortal")
);
const Auth = React.lazy(() => import("./pages/Confirm/sub-components/auth"));

const Error404 = React.lazy(() => import("client/Error404"));
const Error401 = React.lazy(() => import("client/Error401"));

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
      <React.Suspense fallback={<AppLoader />}>
        <ErrorBoundary>
          <Error404 />
        </ErrorBoundary>
      </React.Suspense>
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
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <Client />
              </ErrorBoundary>
            </React.Suspense>
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
            path: "rooms/personal/*",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "files/trash/*",
            element: (
              <PrivateRoute restricted withManager withCollaborator>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/shared/*",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "rooms/archived/*",
            element: (
              <PrivateRoute>
                <Home />
              </PrivateRoute>
            ),
          },
          {
            path: "products/files/",
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
        path: "/portal-settings/",
        element: (
          <PrivateRoute restricted>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <PortalSettings />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
        errorElement: <NotFoundError />,
        children: [
          {
            index: true,
            element: <CustomizationSettings />,
          },
          {
            path: "customization",
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
            path: "security/*",
            element: <SecuritySettings />,
          },
          {
            path: "security/access-portal",
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
            path: "integration/*",
            element: <Integration />,
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
            path: "developer/*",
            element: <DeveloperTools />,
          },
          {
            path: "backup/*",
            element: <Backup />,
          },
          {
            path: "delete-data/*",
            element: <DeleteDataPage />,
          },
          {
            path: "restore/*",
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
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <FormGallery />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
      },
      {
        path: "/wizard",
        element: (
          <PublicRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <Wizard />
              </ErrorBoundary>
            </React.Suspense>
          </PublicRoute>
        ),
      },
      {
        path: "/about",
        element: (
          <PrivateRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <About />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
      },
      {
        path: "/confirm.aspx/*",
        element: (
          <React.Suspense fallback={<AppLoader />}>
            <ErrorBoundary>
              <Confirm />
            </ErrorBoundary>
          </React.Suspense>
        ),
        errorElement: <NotFoundError />,
        children: [...confirmRoutes],
      },
      {
        path: "/confirm/*",
        element: (
          <React.Suspense fallback={<AppLoader />}>
            <ErrorBoundary>
              <Confirm />
            </ErrorBoundary>
          </React.Suspense>
        ),
        errorElement: <NotFoundError />,
        children: [...confirmRoutes],
      },
      {
        path: "/portal-unavailable",
        element: (
          <PrivateRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <PortalUnavailable />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
      },
      {
        path: "/unavailable",
        element: (
          <PrivateRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <ErrorUnavailable />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
      },
      {
        path: "/preparation-portal",
        element: (
          <PublicRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <PreparationPortal />
              </ErrorBoundary>
            </React.Suspense>
          </PublicRoute>
        ),
      },
      {
        path: "/error401",
        element: (
          <PrivateRoute>
            <React.Suspense fallback={<AppLoader />}>
              <ErrorBoundary>
                <Error401 />
              </ErrorBoundary>
            </React.Suspense>
          </PrivateRoute>
        ),
      },
    ],
  },
]);

export default router;
