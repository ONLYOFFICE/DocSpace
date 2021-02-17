import React from "react";

import { FieldContainer, TextInput } from "ASC.Web.Components";

const EmailField = ({
  t,
  userNameValid,
  errorText,
  userName,
  isLoading,
  onChangeLogin,
  onKeyPress,
}) => {
  return (
    <FieldContainer
      isVertical={true}
      labelVisible={false}
      hasError={!userNameValid}
      errorMessage={errorText ? errorText : t("RequiredFieldMessage")} //TODO: Add wrong login server error
    >
      <TextInput
        id="login"
        name="login"
        hasError={!userNameValid}
        value={userName}
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
  );
};

export default EmailField;
