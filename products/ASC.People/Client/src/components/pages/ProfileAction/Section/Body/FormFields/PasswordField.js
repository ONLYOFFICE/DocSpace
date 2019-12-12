import React from "react";
import isEqual from "lodash/isEqual";
import {
  FieldContainer,
  RadioButtonGroup,
  PasswordInput
} from "asc-web-components";

class PasswordField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
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

      copyLinkText
    } = this.props;

    const tooltipPasswordLength =
      "from " + passwordSettings.minLength + " to 30 characters";

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
          spacing='33px'
        />
        <PasswordInput
          inputName={inputName}
          emailInputName={emailInputName}
          inputValue={inputValue}
          inputWidth="320px"
          inputTabIndex={inputTabIndex}
          onChange={inputOnChange}
          clipActionResource={copyLinkText}
          clipEmailResource="E-mail: "
          clipPasswordResource="Password: "
          tooltipPasswordTitle="Password must contain:"
          tooltipPasswordLength={tooltipPasswordLength}
          tooltipPasswordDigits="digits"
          tooltipPasswordCapital="capital letters"
          tooltipPasswordSpecial="special characters (!@#$%^&*)"
          generatorSpecial="!@#$%^&*"
          passwordSettings={passwordSettings}
          isDisabled={inputIsDisabled}
        />
      </FieldContainer>
    );
  }
}

export default PasswordField;
