import React, { useState } from "react";

import { FieldContainer, EmailInput } from "ASC.Web.Components";

const EmailField = ({
  t,
  userNameValid,
  errorText,
  isLoading,
  onChangeLogin,
}) => {
  const [errors, setErrors] = useState(null);

  const onChangeEmail = (result) => {
    const { errors, isValid, value } = result;

    setErrors(null);

    if (!isValid && errors.length > 0 && value) {
      setErrors(t("IncorrectEmail"));
    }

    console.log(errors);

    onChangeLogin && onChangeLogin(result);
  };

  return (
    <FieldContainer
      isVertical={true}
      labelVisible={false}
      hasError={userNameValid}
      errorMessage={errors ? errors : errorText} //TODO: Add wrong login server error
    >
      <EmailInput
        id="login"
        name="login"
        hasError={userNameValid}
        //value={userName}
        placeholder={t("RegistrationEmailWatermark")}
        size="large"
        scale={true}
        isAutoFocussed={true}
        tabIndex={1}
        isDisabled={isLoading}
        autoComplete="username"
        onValidateInput={onChangeEmail}
      />
    </FieldContainer>
  );
};

export default EmailField;
