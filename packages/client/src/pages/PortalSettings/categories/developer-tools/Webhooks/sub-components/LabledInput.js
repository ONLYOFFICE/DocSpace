import React from "react";
import styled from "styled-components";

import TextInput from "@docspace/components/text-input";

import Label from "@docspace/components/label";

const StyledLabel = styled(Label)`
  display: block;
  margin-top: 20px;
  line-height: 20px;

  input {
    margin-top: 4px;
    width: 100%;
  }
`;

export const LabledInput = ({
  label,
  placeholder,
  value,
  onChange,
  name,
  mask,
  hasError,
  className,
  required = false,
  id,
}) => {
  return (
    <StyledLabel text={label} className={className}>
      <TextInput
        id={id}
        name={name}
        placeholder={placeholder}
        tabIndex={1}
        value={value}
        onChange={onChange}
        required={required}
        hasError={hasError}
        {...(mask ? { mask: mask } : {})}
      />
    </StyledLabel>
  );
};
