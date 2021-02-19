import React, { Component, useEffect } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { withRouter } from "react-router";

import {  api  } from "ASC.Web.Common";
import { Text, toastr } from "ASC.Web.Components";

import i18n from "../../../i18n";
import ForgotPasswordModalDialog from "./sub-components/forgot-password-modal-dialog";
import RegisterButton from "./sub-components/register-button";
import LoginForm from "./sub-components/login-form";

const { sendInstructionsToChangePassword } = api.people; //?

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

const FormWrapper = withTranslation()(Form);

const Login = (props) => {
  const language = 'en'//window.navigator.language;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return (
      <FormWrapper i18n={i18n} {...props} />

   
  );
};

export default withRouter(Login);
