import React, { Suspense, lazy, useEffect } from "react";
import { Route, Switch } from "react-router-dom";
import ConfirmRoute from "../../../helpers/confirmRoute";
import { I18nextProvider } from "react-i18next";
import { Error404, utils, store, PageLayout, Loaders } from "asc-web-common";
import { createI18N } from "../../../helpers/i18n";
import { connect } from "react-redux";

const { getIsLoaded } = store.auth.selectors;

const i18n = createI18N({
  page: "Confirm",
  localesPath: "pages/Confirm",
});

const { changeLanguage } = utils;

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
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  //console.log("Confirm render");
  return (
    !isLoaded ? <PageLayout>
    <PageLayout.SectionBody>
      <Loaders.Rectangle height="90vh"/>
    </PageLayout.SectionBody>
  </PageLayout> :

    <I18nextProvider i18n={i18n}>
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
            exact
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
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    isLoaded: getIsLoaded(state)
  };
}

export default connect(mapStateToProps)(Confirm);
