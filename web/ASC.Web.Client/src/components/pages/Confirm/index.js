import React, { Suspense, lazy } from "react";
import { Route, Switch } from "react-router-dom";
import ConfirmRoute from "../../../helpers/confirmRoute";
import { Error404, PageLayout, Loaders } from "asc-web-common";
import { inject, observer } from "mobx-react";

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

const Confirm = ({ match, isLoaded }) => {
  //console.log("Confirm render");
  return !isLoaded ? (
    <PageLayout>
      <PageLayout.SectionBody>
        <Loaders.Rectangle height="90vh" />
      </PageLayout.SectionBody>
    </PageLayout>
  ) : (
    <Suspense fallback={null}>
      <Switch>
        <ConfirmRoute
          forUnauthorized
          path={`${match.path}/LinkInvite`}
          component={CreateUserForm}
        />
        <ConfirmRoute
          forUnauthorized
          path={`${match.path}/Activation`}
          component={ActivateUserForm}
        />
        <ConfirmRoute
          exact
          path={`${match.path}/EmailActivation`}
          component={ActivateEmailForm}
        />
        <ConfirmRoute
          exact
          path={`${match.path}/EmailChange`}
          component={ChangeEmailForm}
        />
        <ConfirmRoute
          forUnauthorized
          path={`${match.path}/PasswordChange`}
          component={ChangePasswordForm}
        />
        <ConfirmRoute
          exact
          path={`${match.path}/ProfileRemove`}
          component={ProfileRemoveForm}
        />
        <Route
          exact
          path={`${match.path}/PhoneActivation`}
          component={ChangePhoneForm}
        />
        <Route
          exact
          path={`${match.path}/ownerchange`}
          component={ChangeOwnerForm}
        />
        <Route component={Error404} />
      </Switch>
    </Suspense>
  );
};

export default inject(({ auth }) => {
  const { isLoaded } = auth;
  return {
    isLoaded,
  };
})(observer(Confirm));
