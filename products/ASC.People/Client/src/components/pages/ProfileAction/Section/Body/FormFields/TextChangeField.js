import React from 'react'
import styled from 'styled-components';
import { FieldContainer, TextInput, Button } from 'asc-web-components'

const InputContainer = styled.div`
  width: 320px;
  display: flex;
  align-items: center;
`;

const TextChangeField = React.memo((props) => {
  const {
    isRequired,
    hasError,
    labelText,

    inputName,
    inputValue,

    buttonText,
    buttonIsDisabled,
    buttonOnClick
  } = props;

  return (
    <FieldContainer
      isRequired={isRequired}
      hasError={hasError}
      labelText={labelText}
    >
      <InputContainer>
        <TextInput
          name={inputName}
          value={inputValue}
          isDisabled={true}
          hasError={hasError}
        />
        <Button
          label={buttonText}
          onClick={buttonOnClick}
          isDisabled={buttonIsDisabled}
          size="medium"
          style={{ marginLeft: "8px" }}
        />
      </InputContainer>
    </FieldContainer>
  );
});

export default TextChangeField;