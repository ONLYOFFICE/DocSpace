import React, { Component, useEffect } from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { withRouter } from "react-router";

import {  store, api,  } from "ASC.Web.Common";
import { Text, toastr } from "ASC.Web.Components";

import i18n from "../../../i18n";
import ForgotPasswordModalDialog from "./sub-components/forgot-password-modal-dialog";
import RegisterButton from "./sub-components/register-button";
import Header from "../../Header";
import LoginForm from "./sub-components/login-form";

const { login, setIsLoaded, reloadPortalSettings } = store.auth.actions;
const { getLanguage, isDesktopClient } = store.auth.selectors;
const { sendInstructionsToChangePassword } = api.people;

const LoginContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-gap: 30px;
  align-items: center;
  margin: 34px 32px 0 32px;
  height: min-content;
  width: 100%;

  .greeting-title {
    width: 100%;
    text-align: left;
  }
`;

const LoginFormWrapper = styled.div`
  display: grid;
  grid-template-rows: ${(props) =>
    props.enabledJoin
      ? props.isDesktop
        ? css`1fr 10px`
        : css`1fr 66px`
      : css`1fr`};
  width: 100%;
  height: calc(100vh-56px);
`;

class Form extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      isDisabled: false,
      isChecked: false,
      openDialog: false,
      email: "",
      emailError: false,
      errorText: "",
      socialButtons: [],
    };
  }

  onChangeEmail = (event) => {
    this.setState({ email: event.target.value, emailError: false });
  };

  onChangeCheckbox = () => this.setState({ isChecked: !this.state.isChecked });

  onClickForgot = () => {
    this.setState({
      openDialog: true,
      isDisabled: true,
      email: this.state.identifier,
    });
  };

  onSendPasswordInstructions = () => {
    if (!this.state.email.trim()) {
      this.setState({ emailError: true });
    } else {
      this.setState({ isLoading: true });
      sendInstructionsToChangePassword(this.state.email)
        .then(
          (res) => toastr.success(res),
          (message) => toastr.error(message)
        )
        .finally(this.onDialogClose());
    }
  };

  onDialogClose = () => {
    this.setState({
      openDialog: false,
      isDisabled: false,
      isLoading: false,
      email: "",
      emailError: false,
    });
  };

  onClickRegistration = () => {
    const { history } = this.props;
    history.push('/registration')
  }

  render() {
    const { t } = this.props;

    const {
      isLoading,
      isChecked,
      openDialog,
      email,
      emailError,
      socialButtons,
    } = this.state;

    return (
      <>
        <LoginContainer>
          <Text
            fontSize="23px"
            fontWeight={600}
            textAlign="center"
            className="greeting-title"
          >
            {t("LoginTitle")}
          </Text>

          <LoginForm
            className="auth-form-container"
            t={t}
            isChecked={isChecked}
            socialButtons={socialButtons}
            isLoading={isLoading}
            onChangeCheckbox={this.onChangeCheckbox}
            onClickForgot={this.onClickForgot}
          />

          {openDialog && (
            <ForgotPasswordModalDialog
              openDialog={openDialog}
              isLoading={isLoading}
              email={email}
              emailError={emailError}
              onChangeEmail={this.onChangeEmail}
              onSendPasswordInstructions={this.onSendPasswordInstructions}
              onDialogClose={this.onDialogClose}
              t={t}
            />
          )}
          <RegisterButton title={t("LoginRegistrationBtn")} onClick={this.onClickRegistration}/>
        </LoginContainer>
      </>
    );
  }
}

Form.propTypes = {
  login: PropTypes.func, //.isRequired,
  match: PropTypes.object, //.isRequired,
  hashSettings: PropTypes.object,
  reloadPortalSettings: PropTypes.func,
  setIsLoaded: PropTypes.func, //.isRequired,
  t: PropTypes.func, //.isRequired,
  i18n: PropTypes.object, //.isRequired,
  language: PropTypes.string, //.isRequired,
  socialButtons: PropTypes.array,
  organizationName: PropTypes.string,
  homepage: PropTypes.string,
  defaultPage: PropTypes.string,
};

Form.defaultProps = {
  identifier: "",
  password: "",
  email: "",
};

const FormWrapper = withTranslation()(Form);

const Login = (props) => {
  const { language, enabledJoin, isDesktop } = props;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return (
      <FormWrapper i18n={i18n} {...props} />

   
  );
};

Login.propTypes = {
  language: PropTypes.string, //.isRequired,
  isLoaded: PropTypes.bool,
  enabledJoin: PropTypes.bool,
  isDesktop: PropTypes.bool, //.isRequired,
};

function mapStateToProps(state) {
  const { isLoaded, settings, isAuthenticated } = state.auth;
  const {
    greetingSettings,
    organizationName,
    hashSettings,
    enabledJoin,
    defaultPage,
  } = settings;

  return {
    isAuthenticated,
    isLoaded,
    isDesktop: isDesktopClient(state),
    organizationName,
    language: getLanguage(state),
    hashSettings,
    enabledJoin,
    defaultPage,
  };
}

export default withRouter(Login); //connect(mapStateToProps, {
//login,
//setIsLoaded,
//reloadPortalSettings,
//})(withRouter(LoginForm));
