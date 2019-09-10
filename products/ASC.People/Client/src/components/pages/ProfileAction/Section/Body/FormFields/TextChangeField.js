import React from 'react'
import styled from 'styled-components';
import { FieldContainer, TextInput, Button } from 'asc-web-components'

const InputContainer = styled.div`
  width: 100%;
  max-width: 320px;
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
    inputTabIndex,

    buttonText,
    buttonIsDisabled,
    buttonOnClick,
    buttonTabIndex
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
          tabIndex={inputTabIndex}
        />
        <Button
          label={buttonText}
          onClick={buttonOnClick}
          isDisabled={buttonIsDisabled}
          size="medium"
          style={{ marginLeft: "8px" }}
          tabIndex={buttonTabIndex}
        />
      </InputContainer>
    </FieldContainer>
  );
});

export default TextChangeField;