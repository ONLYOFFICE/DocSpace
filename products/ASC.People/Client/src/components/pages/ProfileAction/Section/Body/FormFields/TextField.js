import React from "react";
import isEqual from "lodash/isEqual";
import { FieldContainer, TextInput } from "asc-web-components";

class TextField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
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
      tooltipContent
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
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
        />
      </FieldContainer>
    );
  }
}

export default TextField;
