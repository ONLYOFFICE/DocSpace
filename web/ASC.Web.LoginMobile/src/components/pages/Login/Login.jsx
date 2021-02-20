import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { withRouter } from "react-router";

import { utils } from "ASC.Web.Common";
import { Text } from "ASC.Web.Components";

import i18n from "../../../i18n";
import ForgotPasswordModalDialog from "./sub-components/forgot-password-modal-dialog";
import RegisterButton from "./sub-components/register-button";
import LoginForm from "./sub-components/login-form";

import { fakeApi } from "LoginMobileApi";

const { tryRedirectTo } = utils;

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

const Form = ({t}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [isChecked, setIsChecked] = useState(false);
  const [openDialog, setOpenDialog] = useState(false);
  const [email, setEmail] = useState('');
  const [emailError, setEmailError] = useState(false);
  const [socialButtons, setSocialButtons] = useState([]);

  const onChangeEmail = (event) => {
    setEmail(event.target.value);
    setEmailError(false)
  };

  const onChangeCheckbox = () => setIsChecked(!isChecked);

  const onClickForgot = () => {
    setOpenDialog(true)
    // email: this.state.identifier,  replace to login-forn
  };

  const onSendPasswordInstructions = () => {
    if (!email.trim()) {
      setEmailError(true)
    } else {
      setIsLoading(true)
      fakeApi.sendInstructionsToChangePassword(email)
        .then( /*
          (res) => toastr.success(res),
          (message) => toastr.error(message)*/
          // TODO: indication errors 
        )
        .finally(onDialogClose());
    }
  };

  const onDialogClose = () => {
    setOpenDialog(false);
    setIsLoading(false);
    setEmail('');
    setEmailError(false);
  };

  const onClickRegistration = () => {
    const { history } = this.props;
    history.push("/registration");
  };

  const onLoginHandler = (userName, pass) => {
    setIsLoading(true)
    fakeApi
      .login(userName, pass)
      .then(() => {
        tryRedirectTo("/portal-selection");
      })
      .catch((err) => {
        //TODO: error handler
      })
      .finally(setIsLoading(false));
  };

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
            onChangeCheckbox={onChangeCheckbox}
            onClickForgot={onClickForgot}
            onLogin={onLoginHandler}
          />

          {openDialog && (
            <ForgotPasswordModalDialog
              openDialog={openDialog}
              isLoading={isLoading}
              email={email}
              emailError={emailError}
              onChangeEmail={onChangeEmail}
              onSendPasswordInstructions={onSendPasswordInstructions}
              onDialogClose={onDialogClose}
              t={t}
            />
          )}
          <RegisterButton
            title={t("LoginRegistrationBtn")}
            onClick={onClickRegistration}
          />
        </LoginContainer>
      </>
    );
  }


const FormWrapper = withTranslation()(Form);

const Login = (props) => {
  const language = "en"; //window.navigator.language;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return <FormWrapper i18n={i18n} {...props} />;
};

export default withRouter(Login);
