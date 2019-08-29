import React from 'react'
import { FieldContainer, TextInput } from 'asc-web-components'

const TextField = React.memo((props) => {
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
      <TextInput
        name={inputName}
        value={inputValue}
        isDisabled={inputIsDisabled}
        onChange={inputOnChange}
        hasError={hasError}
        className="field-input"
      />
    </FieldContainer>
  );
});

export default TextField;