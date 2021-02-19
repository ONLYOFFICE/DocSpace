import React, { useState } from "react";
import styled from "styled-components";

import { Checkbox, Button, Link, Text } from "ASC.Web.Components";
import { utils } from "ASC.Web.Common";

import { PasswordField, TextField } from "../../../fields";
import { fakeApi } from "LoginMobileApi";

const { tryRedirectTo } = utils;

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
  const [hasError, setHasError] = useState(false);

  const onSubmitHandler = () => {
    errorText && setErrorText("");
    let isError = false;
    const cleanUserName = userName.trim();

    if (!cleanUserName) {
      isError = true;
      setUserNameValid(false);
      setErrorText(t("RequiredFieldMessage"));
      setHasError(isError);
    }

    const cleanPass = password.trim();

    if (!cleanPass) {
      isError = true;
      setPasswordValid(false);
      setErrorText(t("RequiredFieldMessage"));
      setHasError(isError);
    }

    if (isError) return false;

    setIsLoading(true);

    fakeApi
      .login(cleanUserName, cleanPass)
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

  const onChangeLogin = (value, isValid) => {
    setUserName(value);
    setUserNameValid(isValid);
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
    setPasswordValid(true);
  };

  return (
    <StyledLoginForm>
      <TextField
        t={t}
        hasError={!userNameValid}
        errorText={errorText}
        value={userName}
        isLoading={isLoading}
        id="email"
        type="email"
        placeholder="RegistrationEmailWatermark"
        onChangeValue={onChangeLogin}
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
