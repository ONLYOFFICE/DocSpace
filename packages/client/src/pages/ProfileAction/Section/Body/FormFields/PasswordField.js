import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@docspace/components/field-container";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import PasswordInput from "@docspace/components/password-input";
import { PasswordLimitSpecialCharacters } from "@docspace/common/constants";

class PasswordField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const {
      isRequired,
      hasError,
      labelText,
      passwordSettings,

      radioName,
      radioValue,
      radioOptions,
      radioIsDisabled,
      radioOnChange,

      inputName,
      emailInputName,
      inputValue,
      inputIsDisabled,
      inputOnChange,
      inputTabIndex,

      copyLinkText,
      copiedResourceText,
      t,
    } = this.props;
    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
      >
        <RadioButtonGroup
          name={radioName}
          selected={radioValue}
          options={radioOptions}
          isDisabled={radioIsDisabled}
          onClick={radioOnChange}
          className="radio-group"
          spacing="33px"
        />
        <PasswordInput
          inputName={inputName}
          emailInputName={emailInputName}
          inputValue={inputValue}
          inputWidth="320px"
          inputTabIndex={inputTabIndex}
          onChange={inputOnChange}
          clipActionResource={copyLinkText}
          clipCopiedResource={copiedResourceText}
          clipEmailResource={`${t("Common:Email")}: `}
          clipPasswordResource={`${t("Common:Password")}: `}
          tooltipPasswordTitle={`${t("Common:PasswordLimitMessage")}:`}
          tooltipPasswordLength={`${t("Common:PasswordLimitLength", {
            fromNumber: passwordSettings ? passwordSettings.minLength : 8,
            toNumber: 30,
          })}`}
          tooltipPasswordDigits={`${t("Common:PasswordLimitDigits")}`}
          tooltipPasswordCapital={`${t("Common:PasswordLimitUpperCase")}`}
          tooltipPasswordSpecial={`${t(
            "Common:PasswordLimitSpecialSymbols"
          )} (${PasswordLimitSpecialCharacters})`}
          generatorSpecial={PasswordLimitSpecialCharacters}
          passwordSettings={passwordSettings}
          isDisabled={inputIsDisabled}
        />
      </FieldContainer>
    );
  }
}

export default PasswordField;
