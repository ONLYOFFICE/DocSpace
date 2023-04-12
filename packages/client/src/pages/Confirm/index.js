import React, { lazy } from "react";
import { Routes, Route } from "react-router-dom";
import ConfirmRoute from "../../helpers/confirmRoute";

const ActivateUserForm = lazy(() => import("./sub-components/activateUser"));
const CreateUserForm = lazy(() => import("./sub-components/createUser"));
const ChangePasswordForm = lazy(() =>
  import("./sub-components/changePassword")
);
const ActivateEmailForm = lazy(() => import("./sub-components/activateEmail"));
const ChangeEmailForm = lazy(() => import("./sub-components/changeEmail"));
const ChangePhoneForm = lazy(() => import("./sub-components/changePhone"));
const ProfileRemoveForm = lazy(() => import("./sub-components/profileRemove"));
const ChangeOwnerForm = lazy(() => import("./sub-components/changeOwner"));
const TfaAuthForm = lazy(() => import("./sub-components/tfaAuth"));
const TfaActivationForm = lazy(() => import("./sub-components/tfaActivation"));
const RemovePortal = lazy(() => import("./sub-components/removePortal"));
const DeactivatePortal = lazy(() =>
  import("./sub-components/deactivatePortal")
);
const ContinuePortal = lazy(() => import("./sub-components/continuePortal"));
const Auth = lazy(() => import("./sub-components/auth"));
const Error404 = lazy(() => import("../Errors/404"));

const Confirm = () => {
  //console.log("Confirm render");

  return (
    <Routes>
      <Route
        path={`LinkInvite`}
        element={
          <ConfirmRoute forUnauthorized>
            <CreateUserForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`Activation`}
        element={
          <ConfirmRoute forUnauthorized>
            <ActivateUserForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`EmailActivation`}
        element={
          <ConfirmRoute>
            <ActivateEmailForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`EmailChange`}
        element={
          <ConfirmRoute>
            <ChangeEmailForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PasswordChange`}
        element={
          <ConfirmRoute forUnauthorized>
            <ChangePasswordForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`ProfileRemove`}
        element={
          <ConfirmRoute>
            <ProfileRemoveForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PhoneActivation`}
        element={
          <ConfirmRoute>
            <ChangePhoneForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PortalOwnerChange`}
        element={
          <ConfirmRoute>
            <ChangeOwnerForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`TfaAuth`}
        element={
          <ConfirmRoute>
            <TfaAuthForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`TfaActivation`}
        element={
          <ConfirmRoute>
            <TfaActivationForm />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PortalRemove`}
        element={
          <ConfirmRoute>
            <RemovePortal />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PortalSuspend`}
        element={
          <ConfirmRoute>
            <DeactivatePortal />
          </ConfirmRoute>
        }
      />

      <Route
        path={`PortalContinue`}
        element={
          <ConfirmRoute>
            <ContinuePortal />
          </ConfirmRoute>
        }
      />

      <Route
        path={`Auth`}
        element={
          <ConfirmRoute forUnauthorized>
            <Auth />
          </ConfirmRoute>
        }
      />
    </Routes>
  );
};

export default Confirm;
