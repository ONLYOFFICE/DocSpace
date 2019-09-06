import React from 'react'
import { FieldContainer, RadioButtonGroup } from 'asc-web-components'

const RadioField = React.memo((props) => {
  const {
    isRequired,
    hasError,
    labelText,

    radioName,
    radioValue,
    radioOptions,
    radioIsDisabled,
    radioOnChange
  } = props;

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
      />
    </FieldContainer>
  );
});

export default RadioField