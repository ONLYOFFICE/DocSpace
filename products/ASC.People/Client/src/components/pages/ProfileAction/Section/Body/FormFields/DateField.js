import React from 'react'
import { FieldContainer, DateInput } from 'asc-web-components'

const DateField = React.memo((props) => {
  const {
    isRequired,
    hasError,
    labelText,

    inputName,
    inputValue,
    inputIsDisabled,
    inputOnChange
  } = props;

  return (
    <FieldContainer
      isRequired={isRequired}
      hasError={hasError}
      labelText={labelText}
    >
      <DateInput
        name={inputName}
        selected={inputValue}
        disabled={inputIsDisabled}
        onChange={inputOnChange}
        hasError={hasError}
      />
    </FieldContainer>
  );
});

export default DateField