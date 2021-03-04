import React, { useEffect, useState } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Link from "@appserver/components/link";
import toastr from "@appserver/components/toast/toastr";
import Checkbox from "@appserver/components/checkbox";
//import HelpButton from "@appserver/components/help-button";
import PasswordInput from "@appserver/components/password-input";
import FieldContainer from "@appserver/components/field-container";
import PageLayout from "@appserver/common/components/PageLayout";
import ForgotPasswordModalDialog from "./sub-components/forgot-password-modal-dialog";
import Register from "./sub-components/register-container";
import { checkPwd } from "@appserver/common/desktop";
import { sendInstructionsToChangePassword } from "@appserver/common/api/people";
import { createPasswordHash, tryRedirectTo } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import i18n from "./i18n";
import { I18nextProvider, useTranslation } from "react-i18next";

const LoginContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 120px auto 0 auto;
  max-width: 960px;

  @media (max-width: 768px) {
    padding: 0 16px;
    max-width: 475px;
  }
  @media (max-width: 375px) {
    margin: 72px auto 0 auto;
    max-width: 311px;
  }

  .greeting-title {
    width: 100%;

    @media (max-width: 768px) {
      text-align: left;
    }
    @media (max-width: 375px) {
      font-size: 23px;
    }
  }

  .auth-form-container {
    margin: 32px 213px 0 213px;
    width: 311px;

    @media (max-width: 768px) {
      margin: 32px 0 0 0;
      width: 100%;
    }
    @media (max-width: 375px) {
      margin: 32px 0 0 0;
      width: 100%;
    }

    .login-forgot-wrapper {
      height: 36px;
      padding: 14px 0;

      .login-checkbox-wrapper {
        display: flex;

        .login-checkbox {
          float: left;
          span {
            font-size: 12px;
          }
        }
      }

      .login-link {
        line-height: 18px;
        margin-left: auto;
      }
    }

    .login-button {
      margin-bottom: 16px;
    }

    .login-button-dialog {
      margin-right: 8px;
    }

    .login-bottom-border {
      width: 100%;
      height: 1px;
      background: #eceef1;
    }

    .login-bottom-text {
      margin: 0 8px;
    }
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

const Form = (props) => {
  const [identifierValid, setIdentifierValid] = useState(true);
  const [identifier, setIdentifier] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isDisabled, setIsDisabled] = useState(false);

  const [passwordValid, setPasswordValid] = useState(true);
  const [password, setPassword] = useState("");
  const [isChecked, setIsChecked] = useState(false);
  const [openDialog, setOpenDialog] = useState(false);
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [socialButtons, setSocialButtons] = useState([]);

  const {
    login,
    hashSettings,
    isDesktop,
    defaultPage,
    match,
    organizationName,
    greetingTitle,
  } = props;
  const { error, confirmedEmail } = match.params;
  const { t } = useTranslation("Login");

  const onChangeLogin = (event) => {
    setIdentifier(event.target.value);
    !identifierValid && setIdentifierValid(true);
    errorText && setErrorText("");
  };

  const onChangePassword = (event) => {
    setPassword(event.target.value);
    !passwordValid && setPasswordValid(true);
    errorText && setErrorText("");
  };

  const onChangeEmail = (event) => {
    setEmail(event.target.value);
    setEmailError(false);
  };

  const onChangeCheckbox = () => setIsChecked(!isChecked);

  const onClick = () => {
    setOpenDialog(true);
    setIsDisabled(true);
    setEmail(identifier);
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      !isDisabled ? onSubmit() : onSendPasswordInstructions();
    }
  };

  const onSendPasswordInstructions = () => {
    if (!email.trim()) {
      setEmailError(true);
    } else {
      setIsLoading(true);
      sendInstructionsToChangePassword(email)
        .then(
          (res) => toastr.success(res),
          (message) => toastr.error(message)
        )
        .finally(onDialogClose());
    }
  };

  const onDialogClose = () => {
    setOpenDialog(false);
    setIsDisabled(false);
    setIsLoading(false);
    setEmail("");
    setEmailError(false);
  };

  const onSubmit = () => {
    errorText && setErrorText("");
    let hasError = false;

    const userName = identifier.trim();

    if (!userName) {
      hasError = true;
      setIdentifierValid(!hasError);
    }

    const pass = password.trim();

    if (!pass) {
      hasError = true;
      setPasswordValid(!hasError);
    }

    if (hasError) return false;

    setIsLoading(true);
    const hash = createPasswordHash(pass, hashSettings);

    isDesktop && checkPwd();

    login(userName, hash)
      .then(() => tryRedirectTo(defaultPage))
      .catch((error) => {
        setErrorText(error);
        setIdentifierValid(!error);
        setPasswordValid(!error);
        setIsLoading(false);
      });
  };

  useEffect(() => {
    document.title = `${t("Authorization")} – ${organizationName}`; //TODO: implement the setDocumentTitle() utility in ASC.Web.Common

    error && setErrorText(error);
    confirmedEmail && setIdentifier(confirmedEmail);
    window.addEventListener("keyup", onKeyPress);

    return () => {
      window.removeEventListener("keyup", onKeyPress);
    };
  }, []);

  const settings = {
    minLength: 6,
    upperCase: false,
    digits: false,
    specSymbols: false,
  };

  //console.log("Login render");

  return (
    <>
      <LoginContainer>
        <Text
          fontSize="32px"
          fontWeight={600}
          textAlign="center"
          className="greeting-title"
        >
          {greetingTitle}
        </Text>

        <form className="auth-form-container">
          <FieldContainer
            isVertical={true}
            labelVisible={false}
            hasError={!identifierValid}
            errorMessage={errorText ? errorText : t("RequiredFieldMessage")} //TODO: Add wrong login server error
          >
            <TextInput
              id="login"
              name="login"
              hasError={!identifierValid}
              value={identifier}
              placeholder={t("RegistrationEmailWatermark")}
              size="large"
              scale={true}
              isAutoFocussed={true}
              tabIndex={1}
              isDisabled={isLoading}
              autoComplete="username"
              onChange={onChangeLogin}
              onKeyDown={onKeyPress}
            />
          </FieldContainer>
          <FieldContainer
            isVertical={true}
            labelVisible={false}
            hasError={!passwordValid}
            errorMessage={errorText ? "" : t("RequiredFieldMessage")} //TODO: Add wrong password server error
          >
            <PasswordInput
              simpleView={true}
              passwordSettings={settings}
              id="password"
              inputName="password"
              placeholder={t("Password")}
              type="password"
              hasError={!passwordValid}
              inputValue={password}
              size="large"
              scale={true}
              tabIndex={1}
              isDisabled={isLoading}
              autoComplete="current-password"
              onChange={onChangePassword}
              onKeyDown={onKeyPress}
            />
          </FieldContainer>
          <div className="login-forgot-wrapper">
            <div className="login-checkbox-wrapper">
              <Checkbox
                className="login-checkbox"
                isChecked={isChecked}
                onChange={onChangeCheckbox}
                label={<Text fontSize="13px">{t("Remember")}</Text>}
              />
              {/*<HelpButton
                  className="login-tooltip"
                  helpButtonHeaderContent={t("CookieSettingsTitle")}
                  tooltipContent={
                    <Text fontSize="12px">{t("RememberHelper")}</Text>
                  }
                />*/}
              <Link
                fontSize="13px"
                color="#316DAA"
                className="login-link"
                type="page"
                isHovered={false}
                onClick={onClick}
              >
                {t("ForgotPassword")}
              </Link>
            </div>
          </div>

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

          <Button
            id="button"
            className="login-button"
            primary
            size="large"
            scale={true}
            label={isLoading ? t("LoadingProcessing") : t("LoginButton")}
            tabIndex={1}
            isDisabled={isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />

          {confirmedEmail && (
            <Text isBold={true} fontSize="16px">
              {t("MessageEmailConfirmed")} {t("MessageAuthorize")}
            </Text>
          )}
          {/* TODO: old error indication
            
            <Text fontSize="14px" color="#c30">
              {errorText}
            </Text> */}

          {socialButtons.length ? (
            <Box displayProp="flex" alignItems="center">
              <div className="login-bottom-border"></div>
              <Text className="login-bottom-text" color="#A3A9AE">
                {t("Or")}
              </Text>
              <div className="login-bottom-border"></div>
            </Box>
          ) : null}
        </form>
      </LoginContainer>
    </>
  );
};

Form.propTypes = {
  login: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  hashSettings: PropTypes.object,
  greetingTitle: PropTypes.string.isRequired,
  socialButtons: PropTypes.array,
  organizationName: PropTypes.string,
  homepage: PropTypes.string,
  defaultPage: PropTypes.string,
  isDesktop: PropTypes.bool,
};

Form.defaultProps = {
  identifier: "",
  password: "",
  email: "",
};

const LoginForm = (props) => {
  const { enabledJoin, isDesktop } = props;

  return (
    <LoginFormWrapper enabledJoin={enabledJoin} isDesktop={isDesktop}>
      <PageLayout>
        <PageLayout.SectionBody>
          <Form {...props} />
        </PageLayout.SectionBody>
      </PageLayout>
      <Register />
    </LoginFormWrapper>
  );
};

LoginForm.propTypes = {
  isLoaded: PropTypes.bool,
  enabledJoin: PropTypes.bool,
  isDesktop: PropTypes.bool.isRequired,
};

const Login = inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded, login } = auth;
  const {
    greetingSettings: greetingTitle,
    organizationName,
    hashSettings,
    enabledJoin,
    defaultPage,
    isDesktopClient: isDesktop,
  } = settingsStore;

  return {
    isAuthenticated,
    isLoaded,
    organizationName,
    greetingTitle,
    hashSettings,
    enabledJoin,
    defaultPage,
    isDesktop,
    login,
  };
})(withRouter(observer(LoginForm)));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Login {...props} />
  </I18nextProvider>
);
