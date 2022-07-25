import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@docspace/components/field-container";
import EmailInput from "@docspace/components/email-input";

class EmailField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const {
      isRequired,
      hasError,
      labelText,
      emailSettings,

      inputName,
      inputValue,
      inputOnChange,
      inputTabIndex,
      placeholder,
      scale,
      inputIsDisabled,
      onValidateInput,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
      >
        <EmailInput
          className="field-input"
          name={inputName}
          value={inputValue}
          onChange={inputOnChange}
          emailSettings={emailSettings}
          tabIndex={inputTabIndex}
          placeholder={placeholder}
          scale={scale}
          autoComplete="email"
          isDisabled={inputIsDisabled}
          onValidateInput={onValidateInput}
          hasError={hasError}
        />
      </FieldContainer>
    );
  }
}

export default EmailField;
