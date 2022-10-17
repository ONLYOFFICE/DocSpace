import React from "react";
import styled from "styled-components";
import { hugeMobile } from "@docspace/components/utils/device";

const StyledNav = styled.div`
  display: none;
  height: 48px;
  align-items: center;
  justify-content: center;
  background-color: #f8f9f9;

  @media ${hugeMobile} {
    display: flex;
  }
`;

const SimpleNav = () => {
  return (
    <StyledNav>
      <img src="/static/images/logo.docspace.react.svg" />
    </StyledNav>
  );
};

export default SimpleNav;
