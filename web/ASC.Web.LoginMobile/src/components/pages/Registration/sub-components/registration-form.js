import React, { useState } from "react";
import styled from "styled-components";

import { Button } from "ASC.Web.Components";

import { EmailField, PasswordField, TextField } from "../../../fields";

const StyledRegistrationForm = styled("form")`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  margin-top: 6px;

  .registration-button {
    margin-top: 28px;
  }
`;

const RegistrationForm = ({ t, isLoading, onJoin }) => {
  const [portalName, setPortalName] = useState({
    val: "",
    isValid: false,
  });

  const [email, setEmail] = useState({ val: "", isValid: false });

  const [firstName, setFirstName] = useState({
    val: "",
    isValid: false,
  });

  const [lastName, setLastName] = useState({
    val: "",
    isValid: false,
  });

  const [password, setPassword] = useState({
    val: "",
    isValid: false,
  });

  const [hasError, setHasError] = useState(false);

  const onChangePortalNameHandler = (val, isValid) => {
    setPortalName({ val, isValid });
  };

  const onChangeEmailHandler = (val, isValid) => {
    setEmail({ val, isValid });
  };

  const onChangeFirstNameHandler = (val, isValid) => {
    setFirstName({ val, isValid });
  };

  const onChangeLastNameHandler = (val, isValid) => {
    setLastName({ val, isValid });
  };

  const onChangePasswordHandler = (val, isValid) => {
    setPassword({ val, isValid });
  };

  const onSubmitHandler = () => {
    const isValid = checkingValid();
    if (!isValid) return;
    onJoin(
      portalName.val,
      email.val,
      firstName.val,
      lastName.val,
      password.val
    );
  };

  const checkingValid = () => {
    let isValid = false;
    if (
      portalName.isValid &&
      email.isValid &&
      firstName.isValid &&
      lastName.isValid &&
      password.isValid
    ) {
      isValid = true;
    }
    setHasError(!isValid);
    return isValid;
  };

  return (
    <StyledRegistrationForm onSubmit={onSubmitHandler}>
      <TextField
        id="portal-name"
        className="fields"
        t={t}
        placeholder={t('PlaceholderPortalName')}
        isAutoFocussed
        value={portalName.val}
        hasError={!portalName.isValid && hasError}
        onChangeValue={onChangePortalNameHandler}
      />
      <EmailField
        className="fields"
        t={t}
        value={email.val}
        hasError={!email.isValid && hasError}
        onChangeEmail={onChangeEmailHandler}
      />
      <TextField
        id="first-name"
        className="fields"
        t={t}
        placeholder={t('PlaceholderFirstName')}
        value={firstName.val}
        hasError={!firstName.isValid && hasError}
        onChangeValue={onChangeFirstNameHandler}
      />
      <TextField
        id="last-name"
        className="fields"
        t={t}
        placeholder={t('PlaceholderLastName')}
        value={lastName.val}
        hasError={!lastName.isValid && hasError}
        onChangeValue={onChangeLastNameHandler}
      />
      <PasswordField t={t} onChangePassword={onChangePasswordHandler} />
      <Button
        className="registration-button"
        primary
        scale={true}
        size="large"
        label={isLoading ? t("LoadingProcessing") : t("JoinBtn")}
        isDisabled={isLoading}
        isLoading={isLoading}
        onClick={onSubmitHandler}
      />
    </StyledRegistrationForm>
  );
};

export default RegistrationForm;
