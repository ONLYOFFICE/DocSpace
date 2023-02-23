import React from "react";
import styled from "styled-components";

import TextInput from "@docspace/components/text-input";

const Label = styled.label`
  display: block;
  margin-top: 20px;
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 13px;
  line-height: 20px;

  color: #333333;

  input {
    margin-top: 4px;
    width: 100%;
  }
`;

export const LabledInput = ({ label, placeholder }) => {
  return (
    <Label>
      <span>{label}</span>
      <TextInput
        id=""
        mask={null}
        name=""
        onBlur={() => {}}
        onChange={function noRefCheck() {}}
        onFocus={function noRefCheck() {}}
        placeholder={placeholder}
        tabIndex={1}
      />
    </Label>
  );
};
