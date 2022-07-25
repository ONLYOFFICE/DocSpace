import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";

class TextField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const {
      isRequired,
      hasError,
      labelText,
      errorMessage,

      inputName,
      inputValue,
      inputIsDisabled,
      inputOnChange,
      inputAutoFocussed,
      inputTabIndex,
      tooltipContent,
      helpButtonHeaderContent,
      maxLength,
      maxLabelWidth,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        errorMessage={errorMessage}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
        maxLabelWidth={maxLabelWidth}
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
