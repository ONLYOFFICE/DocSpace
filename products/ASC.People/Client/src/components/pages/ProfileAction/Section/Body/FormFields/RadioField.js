import React from "react";
import isEqual from "lodash/isEqual";
import { FieldContainer, RadioButtonGroup } from "asc-web-components";

class RadioField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    console.log("RadioField render");

    const {
      isRequired,
      hasError,
      labelText,

      radioName,
      radioValue,
      radioOptions,
      radioIsDisabled,
      radioOnChange,

      tooltipContent,
      helpButtonHeaderContent,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
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
      </FieldContainer>
    );
  }
}

export default RadioField;
