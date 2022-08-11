import React from "react";
import styled from "styled-components";
import { tablet, hugeMobile } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 32px;
  background: #ffffff;
  box-shadow: 0px 5px 20px rgba(4, 15, 27, 0.07);
  border-radius: 12px;
  max-width: 320px;

  @media ${tablet} {
    max-width: 416px;
    min-width: 416px;
  }

  @media ${hugeMobile} {
    padding: 0;
    border-radius: 0;
    box-shadow: none;
    max-width: 343px;
    min-width: 343px;
    background: #ffffff;
  }
`;

const FormWrapper = (props) => {
  const { children } = props;
  return <StyledWrapper>{children}</StyledWrapper>;
};

export default FormWrapper;
