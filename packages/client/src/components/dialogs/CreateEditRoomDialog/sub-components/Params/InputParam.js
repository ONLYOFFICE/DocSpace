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

const InputParam = React.forwardRef(
  (
    {
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
      onKeyUp,
      onKeyDown,
    },
    ref
  ) => {
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
            forwardedRef={ref}
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
            onKeyUp={onKeyUp}
            onKeyDown={onKeyDown}
          />
        </FieldContainer>
      </StyledInputParam>
    );
  }
);

InputParam.defaultProps = {
  isValidTitle: true,
};

export default InputParam;
