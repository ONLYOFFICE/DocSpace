import React from "react";
import styled from "styled-components";
import { StyledParam } from "./StyledParam";

import Label from "@docspace/components/label";
import TextInput from "@docspace/components/text-input";
import FieldContainer from "@docspace/components/field-container";

const StyledInputParam = styled(StyledParam)`
  flex-direction: column;
  gap: 4px;
  max-height: 54px;

  .input-label {
    cursor: pointer;
    user-select: none;
  }
`;

const InputParam = ({
  id,
  title,
  placeholder,
  value,
  onChange,
  onFocus,
  onBlur,
  isDisabled,
  isValidTitle,
  errorMessage,
  isAutoFocussed,
}) => {
  return (
    <StyledInputParam>
      <Label
        title={title}
        className="input-label"
        display="display"
        htmlFor={id}
        text={title}
      />

      <FieldContainer
        isVertical={true}
        labelVisible={false}
        hasError={!isValidTitle}
        errorMessage={errorMessage}
      >
        <TextInput
          id={id}
          value={value}
          onChange={onChange}
          onFocus={onFocus}
          onBlur={onBlur}
          scale
          placeholder={placeholder}
          tabIndex={2}
          isDisabled={isDisabled}
          hasError={!isValidTitle}
          isAutoFocussed={isAutoFocussed}
        />
      </FieldContainer>
    </StyledInputParam>
  );
};

InputParam.defaultProps = {
  isValidTitle: true,
};

export default InputParam;
