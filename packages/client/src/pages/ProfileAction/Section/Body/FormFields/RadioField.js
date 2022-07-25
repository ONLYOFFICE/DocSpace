import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@docspace/components/field-container";
import RadioButtonGroup from "@docspace/components/radio-button-group";

class RadioField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
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
      maxLabelWidth,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
        maxLabelWidth={maxLabelWidth}
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
