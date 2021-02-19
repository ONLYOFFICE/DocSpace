import React, { useState } from "react";

import { FieldContainer, EmailInput } from "ASC.Web.Components";

const EmailField = ({
  value,
  t,
  hasError,
  errorText,
  isLoading,
  onChangeEmail,
}) => {
  const [errors, setErrors] = useState(null);
  const [isEmpty, setIsEmpty] = useState(true);

  const onChangeEmailHandler = (result) => {
    const { errors, value, isValid } = result;
    let errorsString = "";

    setIsEmpty(!value);

    if (errors.length > 0) {
      const translatedErrors = errors.map((item) => {
        return t(`${item}`);
      });
      errorsString = translatedErrors.join(", ");
      errorsString = errorsString[0].toUpperCase() + errorsString.slice(1);
    }

    setErrors(errorsString);
    onChangeEmail && onChangeEmail(value, isValid);
  };

  return (
    <FieldContainer
      isVertical={true}
      labelVisible={false}
      hasError={hasError}
      errorMessage={isEmpty ? t("RequiredFieldMessage") : errors} //TODO: Add wrong login server error
    >
      <EmailInput
        id="login"
        name="login"
        hasError={hasError}
        value={value}
        placeholder={t("RegistrationEmailWatermark")}
        size="large"
        scale={true}
        tabIndex={1}
        isDisabled={isLoading}
        autoComplete="username"
        onValidateInput={onChangeEmailHandler}
      />
    </FieldContainer>
  );
};

export default EmailField;
