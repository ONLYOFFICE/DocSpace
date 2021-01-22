import React from "react";
import equal from "fast-deep-equal/react";
import {
  FieldContainer,
  RadioButtonGroup,
  PasswordInput,
} from "asc-web-components";

class PasswordField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    console.log("PasswordField render");

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
      t,
    } = this.props;

    const specialCharacters = "!@#$%^&*";

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
          clipEmailResource={`${t("Email")}: `}
          clipPasswordResource={`${t("Password")}: `}
          tooltipPasswordTitle={`${t("ErrorPasswordMessage")}:`}
          tooltipPasswordLength={t("ErrorPasswordLength", {
            from: passwordSettings ? passwordSettings.minLength : 8,
            to: "30",
          })}
          tooltipPasswordDigits={t("ErrorPasswordNoDigits")}
          tooltipPasswordCapital={t("ErrorPasswordNoUpperCase")}
          tooltipPasswordSpecial={`${t(
            "ErrorPasswordNoSpecialSymbols"
          )} (${specialCharacters})`}
          generatorSpecial={specialCharacters}
          passwordSettings={passwordSettings}
          isDisabled={inputIsDisabled}
        />
      </FieldContainer>
    );
  }
}

export default PasswordField;
