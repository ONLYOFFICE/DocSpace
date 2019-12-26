import React from "react";
import isEqual from "lodash/isEqual";
import {
  FieldContainer,
  EmailInput
} from "asc-web-components";

class EmailField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    console.log("EmailField render");

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
          autoComplete='email'
          isDisabled={inputIsDisabled}
          onValidateInput={onValidateInput}
          hasError={hasError}
        />
      </FieldContainer>
    );
  }
}

export default EmailField;
