import React from "react";
import styled from "styled-components";

import Label from "@docspace/components/label";

const StyledControlGroup = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
`;

const ControlGroup = ({ labelText, children }) => {
  return (
    <StyledControlGroup>
      <Label className="label" text={labelText} />
      {children}
    </StyledControlGroup>
  );
};

export default ControlGroup;
