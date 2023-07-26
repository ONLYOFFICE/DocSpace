import React from "react";
import loadable from "@loadable/component";

import ConfirmRoute from "../helpers/confirmRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";

import Error404 from "SRC_DIR/pages/Errors/404";

const Confirm = loadable(() => import("../pages/Confirm"));

const ActivateUserForm = loadable(() =>
  import("../pages/Confirm/sub-components/activateUser")
);
const CreateUserForm = loadable(() =>
  import("../pages/Confirm/sub-components/createUser")
);
const ChangePasswordForm = loadable(() =>
  import("../pages/Confirm/sub-components/changePassword")
);
const ActivateEmailForm = loadable(() =>
  import("../pages/Confirm/sub-components/activateEmail")
);
const ChangeEmailForm = loadable(() =>
  import("../pages/Confirm/sub-components/changeEmail")
);
const ChangePhoneForm = loadable(() =>
  import("../pages/Confirm/sub-components/changePhone")
);
const ProfileRemoveForm = loadable(() =>
  import("../pages/Confirm/sub-components/profileRemove")
);
const ChangeOwnerForm = loadable(() =>
  import("../pages/Confirm/sub-components/changeOwner")
);
const TfaAuthForm = loadable(() =>
  import("../pages/Confirm/sub-components/tfaAuth")
);
const TfaActivationForm = loadable(() =>
  import("../pages/Confirm/sub-components/tfaActivation")
);
const RemovePortal = loadable(() =>
  import("../pages/Confirm/sub-components/removePortal")
);
const DeactivatePortal = loadable(() =>
  import("../pages/Confirm/sub-components/deactivatePortal")
);
const ContinuePortal = loadable(() =>
  import("../pages/Confirm/sub-components/continuePortal")
);
const Auth = loadable(() => import("../pages/Confirm/sub-components/auth"));

const confirmRoutes = [
  {
    path: "EmpInvite",
    element: (
      <ConfirmRoute forUnauthorized>
        <CreateUserForm />
      </ConfirmRoute>
    ),
  },
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

const ConfirmParentRoutes = [
  {
    path: "confirm.aspx",
    element: (
      <ErrorBoundary>
        <Confirm />
      </ErrorBoundary>
    ),
    errorElement: <Error404 />,
    children: [...confirmRoutes],
  },
  {
    path: "confirm",
    element: (
      <ErrorBoundary>
        <Confirm />
      </ErrorBoundary>
    ),
    errorElement: <Error404 />,
    children: [...confirmRoutes],
  },
];

export default ConfirmParentRoutes;
