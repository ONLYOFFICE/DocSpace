import React from "react";

import { FieldContainer, TextInput } from "ASC.Web.Components";

const EmailField = ({
  t,
  identifierValid,
  errorText,
  identifier,
  isLoading,
  onChangeLogin,
  onKeyPress,
}) => {
  return (
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
  );
};

export default EmailField;
