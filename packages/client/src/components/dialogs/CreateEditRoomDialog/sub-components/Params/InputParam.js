import React from "react";
import styled from "styled-components";
import { StyledParam } from "./StyledParam";

import Label from "@docspace/components/label";
import TextInput from "@docspace/components/text-input";

const StyledInputParam = styled(StyledParam)`
  flex-direction: column;
  gap: 4px;

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
}) => {
  return (
    <StyledInputParam>
      <Label
        className="input-label"
        display="display"
        htmlFor={id}
        text={title}
      />
      <TextInput
        id={id}
        value={value}
        onChange={onChange}
        onFocus={onFocus}
        onBlur={onBlur}
        scale
        placeholder={placeholder}
        tabIndex={2}
      />
    </StyledInputParam>
  );
};

export default InputParam;
