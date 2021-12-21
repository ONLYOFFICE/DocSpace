import React, { useState } from "react";
import PasswordInput from "@appserver/components/password-input";
import Button from "@appserver/components/button";
import styled, { css } from "styled-components";

const StyledBody = styled.div`
  display: flex;
  width: 468px;

  #conversion-button {
    margin-left: 8px;
  }
  #conversion-password {
    width: 382px;
  }
`;
const PasswordComponent = ({}) => {
  const [password, setPassword] = useState();
  const onClick = () => {
    console.log("on click");
  };
  return (
    <StyledBody>
      <PasswordInput
        simpleView
        id="conversion-password"
        type="password"
        inputValue={password}
      />
      <Button
        id="conversion-button"
        size="medium"
        scale
        primary
        label={"Button Text"}
        onClick={onClick}
      />
    </StyledBody>
  );
};

export default PasswordComponent;
