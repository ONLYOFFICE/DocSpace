import React, { useState } from "react";
import styled from "styled-components";

import { Checkbox, Button, Link, Text } from "ASC.Web.Components";

import { PasswordField, TextField } from "../../../fields";


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
  isLoading,
  onChangeCheckbox,
  onClickForgot,
  onLogin,
}) => {
  const [hasError, setHasError] = useState(false);
  const [userName, setUserName] = useState({ val: "", isValid: false });
  const [password, setPassword] = useState({ val: "", isValid: false });

  const onSubmitHandler = () => {
    const isValid = checkingValid();

    if (!isValid) return;

    onLogin(userName.val, password.val);
  };

  const checkingValid = () => {
    let isValid = false;
    if (userName.isValid && password.isValid) isValid = true;
    setHasError(!isValid);
    return isValid;
  };

  const onChangeLogin = (val, isValid) => {
    setUserName({ val, isValid });
  };

  const onChangePassword = (val, isValid) => {
    setPassword({ val, isValid });
  };

  return (
    <StyledLoginForm>
      <TextField
        t={t}
        hasError={!userName.isValid && hasError}
        value={userName.val}
        isLoading={isLoading}
        id="email"
        type="email"
        isAutoFocussed
        placeholder="RegistrationEmailWatermark"
        onChangeValue={onChangeLogin}
      />
      <PasswordField
        t={t}
        hasError={!password.isValid && hasError}
        password={password.val}
        isLoading={isLoading}
        onChangePassword={onChangePassword}
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
