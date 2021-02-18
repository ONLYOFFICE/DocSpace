import React, { useState } from "react";
import styled from "styled-components";

import { Checkbox, Button, Link, Text } from "ASC.Web.Components";
import { checkPwd, utils } from "ASC.Web.Common";

import { PasswordField, EmailField } from "../../../fields";
import { fakeApi } from "LoginMobileApi";

const { createPasswordHash, tryRedirectTo } = utils;

const StyledLoginForm = styled("form")`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;

  .login-forgot-wrapper {
    display: grid;
    grid-template-columns: 1fr 1fr;

    .login-checkbox {
      float: left;

      .checkbox {
        margin-right: 8px;
      }
      span {
        font-size: 12px;
      }
    }

    .login-link {
      margin-left: auto;
    }
  }

  .login-button {
    margin-top: 32px;
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
`;

const LoginForm = ({
  t,
  isChecked,
  socialButtons,
  onKeyPress,
  onChangeCheckbox,
  onClickForgot,
}) => {
  const [errorText, setErrorText] = useState("");
  const [userName, setUserName] = useState("");
  const [userNameValid, setUserNameValid] = useState(true);
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [isLoading, setIsLoading] = useState(false);

  const onSubmitHandler = () => {
    errorText && setErrorText("");
    let hasError = false;

    const clearUserName = userName.trim();

    if (!clearUserName) {
      hasError = true;
      setUserNameValid(!hasError);
    }

    const clearPass = password.trim();

    if (!clearPass) {
      hasError = true;
      setPasswordValid(!hasError);
    }

    if (hasError) return false;

    setIsLoading(true);
    //const hash = createPasswordHash(pass, fakeApi.fakeHashSettings);

    // checkPwd(); //?

    fakeApi
      .login(clearUserName, clearPass)
      .then(() => {
        tryRedirectTo("/portal-selection");
      })
      .catch((err) => {
        setErrorText(err);
        setPasswordValid(false);
        setUserNameValid(false);
      })
      .finally(setIsLoading(false));
  };

  const onChangeLogin = (e) => {
    setUserName(e.target.value);
    !userNameValid && setUserNameValid(true);
    errorText && setErrorText("");
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
    !passwordValid && setPasswordValid(true);
    errorText && setErrorText("");
  };

  return (
    <StyledLoginForm>
      <EmailField
        t={t}
        userNameValid={userNameValid}
        errorText={errorText}
        userName={userName}
        isLoading={isLoading}
        onChangeLogin={onChangeLogin}
        onKeyPress={onKeyPress}
      />
      <PasswordField
        t={t}
        passwordValid={passwordValid}
        errorText={errorText}
        password={password}
        isLoading={isLoading}
        onChangePassword={onChangePassword}
        onKeyPress={onKeyPress}
      />
      <div className="login-forgot-wrapper">
        <Checkbox
          className="login-checkbox"
          isChecked={isChecked}
          onChange={onChangeCheckbox}
          label={<Text fontSize="13px">{t("Remember")}</Text>}
        />
        <Link
          fontSize="13px"
          color="#316DAA"
          className="login-link"
          type="page"
          isHovered={false}
          onClick={onClickForgot}
        >
          {t("ForgotPassword")}
        </Link>
      </div>

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
        onClick={onSubmitHandler}
      />

      {socialButtons.length ? (
        <Box displayProp="flex" alignItems="center">
          <div className="login-bottom-border"></div>
          <Text className="login-bottom-text" color="#A3A9AE">
            {t("Or")}
          </Text>
          <div className="login-bottom-border"></div>
        </Box>
      ) : null}
    </StyledLoginForm>
  );
};

export default LoginForm;
