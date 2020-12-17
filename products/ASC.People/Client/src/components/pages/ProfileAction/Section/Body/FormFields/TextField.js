import React from "react";
import equal from "fast-deep-equal/react";
import { FieldContainer, TextInput } from "asc-web-components";

class TextField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    console.log("TextField render");

    const {
      isRequired,
      hasError,
      labelText,

      inputName,
      inputValue,
      inputIsDisabled,
      inputOnChange,
      inputAutoFocussed,
      inputTabIndex,
      tooltipContent,
      helpButtonHeaderContent,
      maxLength,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
      >
        <TextInput
          name={inputName}
          value={inputValue}
          isDisabled={inputIsDisabled}
          onChange={inputOnChange}
          hasError={hasError}
          className="field-input"
          isAutoFocussed={inputAutoFocussed}
          tabIndex={inputTabIndex}
          maxLength={maxLength}
        />
      </FieldContainer>
    );
  }
}

export default TextField;
