import React from "react";

import { PasswordInput, FieldContainer } from "ASC.Web.Components";

const settings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false,
};

const PasswordField = ({
  t,
  passwordValid,
  errorText,
  password,
  isLoading,
  onChangePassword,
  onKeyPress,
}) => {
  const tooltipPassTitle = t("TooltipPasswordTitle");
  const tooltipPassLength = `${settings.minLength}${t(
    "TooltipPasswordLength"
  )}`;
  const tooltipPassDigits = settings.digits
    ? `${t("TooltipPasswordDigits")}`
    : null;
  const tooltipPassCapital = settings.upperCase
    ? `${t("TooltipPasswordCapital")}`
    : null;
  const tooltipPassSpecial = settings.specSymbols
    ? `${t("TooltipPasswordSpecial")}`
    : null;

  return (
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
        isTextTooltipVisible={true}
        tooltipPasswordTitle={tooltipPassTitle}
        tooltipPasswordLength={tooltipPassLength}
        tooltipPasswordDigits={tooltipPassDigits}
        tooltipPasswordCapital={tooltipPassCapital}
        tooltipPasswordSpecial={tooltipPassSpecial}
      />
    </FieldContainer>
  );
};

export default PasswordField;
