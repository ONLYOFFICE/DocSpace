import React, { Suspense, lazy } from "react";
import { Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import { PublicRoute } from "../../../helpers/publicRoute";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";

const CreateUserForm = lazy(() => import("./sub-components/createUser"));
const ChangePasswordForm = lazy(() => import("./sub-components/changePassword"));
const ActivateEmailForm = lazy(() => import("./sub-components/activateEmail"));
const ChangePhoneForm = lazy(() => import("./sub-components/changePhone"));

const Confirm = ({ match }) => {
  //console.log("Confirm render");

  return (
      <I18nextProvider i18n={i18n}>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size={40} />}
        >
          <Switch>
            <PublicRoute
              path={`${match.path}/type=LinkInvite`}
              component={CreateUserForm}
            />
            <PublicRoute
              exact
              path={`${match.path}/type=EmailActivation`}
              component={ActivateEmailForm}
            />
            <PublicRoute
              exact
              path={`${match.path}/type=PasswordChange`}
              component={ChangePasswordForm}
            />
            <PublicRoute
              exact
              path={`${match.path}/type=PhoneActivation`}
              component={ChangePhoneForm}
            />
            <Redirect to={{ pathname: "/" }} />
          </Switch>
        </Suspense>
      </I18nextProvider>
  );
};

export default Confirm;
