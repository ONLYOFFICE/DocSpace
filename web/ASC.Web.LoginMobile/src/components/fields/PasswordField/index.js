import React, { useState } from "react";
import styled from "styled-components";

import { PasswordInput, FieldContainer } from "ASC.Web.Components";

const StyledFieldContainer = styled(FieldContainer)`
  .password-field-wrapper {
    width: 100%;
  }
`;
const PasswordField = ({
  t,
  hasError,
  value,
  isLoading,
  settings,
  isRegisterForm,
  onChangePassword,
}) => {
  const [validation, setValidation] = useState({
    isValid: false,
    errors: "",
  });

  const [isEmptyPass, setIsEmptyPass] = useState(true);

  const tooltipPassTitle = t("TooltipPasswordTitle");
  const tooltipPassLength = settings.minLength
    ? `${settings.minLength}${t("TooltipPasswordLength")}`
    : null;
  const tooltipPassDigits = settings.digits
    ? `${t("TooltipPasswordDigits")}`
    : null;
  const tooltipPassCapital = settings.upperCase
    ? `${t("TooltipPasswordCapital")}`
    : null;
  const tooltipPassSpecial = settings.specSymbols
    ? `${t("TooltipPasswordSpecial")}`
    : null;

  const onChangePassHandler = (e) => {
    const pass = e.target.value;
    const cleanPass = pass.trim();
    let isValid = true;

    if (!cleanPass) {
      isValid = false;
      setIsEmptyPass(true);
    }

    if (isRegisterForm && cleanPass) {
      isValid = validation.isValid;
      setIsEmptyPass(false);
    }

    onChangePassword(cleanPass, isValid);
  };

  const isValidPassHandler = (isValid, validation) => {
    const errorsArr = [];
    let errors = "";
    for (let key in validation) {
      if (!validation[key]) {
        errorsArr.push("Incorrect" + key[0].toUpperCase() + key.slice(1));
      }
    }

    const translatedErrors = errorsArr.map((item) => {
      return t(`${item}`, { lengthSetting: settings.minLength });
    });

    if (translatedErrors.length > 0) {
      errors = t("Add") + translatedErrors.join(", ");
    }

    setValidation({ isValid, errors });
  };

  const hasErrorUpdated = isRegisterForm
    ? (hasError && !validation.isValid) || !!validation.errors
    : hasError;

  const needTooltip = isRegisterForm
    ? (hasError && !validation.isValid) || !!validation.errors
    : false;

  console.log(validation.errors);

  return (
    <StyledFieldContainer
      isVertical={true}
      labelVisible={false}
      hasError={hasErrorUpdated}
      errorMessage={
        validation.errors && !isEmptyPass
          ? validation.errors
          : t("RequiredFieldMessage")
      } //TODO: Add wrong password server error
    >
      <PasswordInput
        simpleView={!isRegisterForm}
        passwordSettings={settings}
        id="password"
        inputName="password"
        placeholder={t("Password")}
        type="password"
        hasError={hasErrorUpdated}
        inputValue={value}
        size="large"
        scale={true}
        tabIndex={1}
        isDisabled={isLoading}
        autoComplete="current-password"
        onChange={onChangePassHandler}
        isTextTooltipVisible={!needTooltip && isRegisterForm}
        isDisableTooltip={true}
        hideNewPasswordButton={true}
        tooltipPasswordTitle={tooltipPassTitle}
        tooltipPasswordLength={tooltipPassLength}
        tooltipPasswordDigits={tooltipPassDigits}
        tooltipPasswordCapital={tooltipPassCapital}
        tooltipPasswordSpecial={tooltipPassSpecial}
        onValidateInput={isValidPassHandler}
      />
    </StyledFieldContainer>
  );
};

PasswordField.defaultProps = {
  settings: {
    minLength: 6,
    upperCase: true,
    digits: true,
    specSymbols: true,
  },
};

export default PasswordField;
