import React, { Suspense, lazy } from "react";
import { Redirect, Route } from "react-router-dom";
import { Loader } from "asc-web-components";
import PublicRoute from "../../../helpers/publicRoute";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
// import ChangeEmailForm from "./sub-components/changeEmail";

const CreateUserForm = lazy(() => import("./sub-components/createUser"));
const ChangePasswordForm = lazy(() => import("./sub-components/changePassword"));
const ActivateEmailForm = lazy(() => import("./sub-components/activateEmail"));
const ChangeEmailForm = lazy(() => import("./sub-components/changeEmail"));
const ChangePhoneForm = lazy(() => import("./sub-components/changePhone"));

const ConfirmType = (props) => {
  switch (props.type) {
    case 'LinkInvite':
      return <PublicRoute
        component={CreateUserForm}
      />;
    case 'EmailActivation':
      return <Route
        component={ActivateEmailForm}
      />;
    case 'EmailChange':
      return <Route
        component={ChangeEmailForm}
      />;
    case 'PasswordChange':
      return <Route
        component={ChangePasswordForm}
      />;
    case 'PhoneActivation':
      return <Route
        component={ChangePhoneForm}
      />;
    default:
      return <Redirect to={{ pathname: "/" }} />;
  }
}
class Confirm extends React.Component {

  constructor(props) {
    const queryParams = props.location.search.slice(1).split('&');
    const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
    const linkParams = Object.fromEntries(arrayOfQueryParams);
    super(props);
    this.state = {
      type: linkParams.type
    };
  }

  render() {
    //console.log("Confirm render");
    return (
      <I18nextProvider i18n={i18n}>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size={40} />}
        >
          <ConfirmType type={this.state.type} />
        </Suspense>
      </I18nextProvider >
    );
  };
}

export default Confirm;
