import React, { Suspense, lazy } from "react";
import { connect } from "react-redux";
import { Redirect, Route, Switch } from "react-router-dom";
import { Loader } from "asc-web-components";
import PublicRoute from "../../../helpers/publicRoute";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import ChangeEmailForm from "./sub-components/changeEmail";
import CreateUserForm from "./sub-components/createUser";

// const CreateUserForm = lazy(() => import("./sub-components/createUser"));
const ChangePasswordForm = lazy(() => import("./sub-components/changePassword"));
const ActivateEmailForm = lazy(() => import("./sub-components/activateEmail"));
// const ChangeEmailForm = lazy(() => import("./sub-components/changeEmail"));
const ChangePhoneForm = lazy(() => import("./sub-components/changePhone"));

const Confirm = ({ match, language }) => {

  i18n.changeLanguage(language);

  //console.log("Confirm render");
  return (
    <I18nextProvider i18n={i18n}>
      <Suspense
        fallback={<Loader className="pageLoader" type="rombs" size={40} />}
      >
        <Switch>
          <PublicRoute
            path={[`${match.path}/LinkInvite`, `${match.path}/Activation`]}
            component={CreateUserForm}
          />
          <Route
            exact
            path={`${match.path}/EmailActivation`}
            component={ActivateEmailForm}
          />
          <Route
            exact
            path={`${match.path}/EmailChange`}
            component={ChangeEmailForm}
          />
          <Route
            exact
            path={`${match.path}/PasswordChange`}
            component={ChangePasswordForm}
          />
          <Route
            exact
            path={`${match.path}/PhoneActivation`}
            component={ChangePhoneForm}
          />
          <Redirect to={{ pathname: "/" }} />
        </Switch>
      </Suspense>
    </I18nextProvider >
  );
};

function mapStatToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture,
  };
}

export default connect(mapStatToProps)(Confirm);
