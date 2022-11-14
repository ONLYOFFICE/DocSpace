import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { hugeMobile } from "@docspace/components/utils/device";
import { ReactSVG } from "react-svg";

const StyledNav = styled.div`
  display: none;
  height: 48px;
  align-items: center;
  justify-content: center;
  background-color: ${(props) => props.theme.login.navBackground};

  svg {
    path:last-child {
      fill: ${(props) => props.theme.client.home.logoColor};
    }
  }
  @media ${hugeMobile} {
    display: flex;
  }
`;

const SimpleNav = ({ theme }) => {
  return (
    <StyledNav id="login-header" theme={theme}>
      <ReactSVG src="/static/images/logo.docspace.react.svg" />
    </StyledNav>
  );
};

export default inject(({ loginStore }) => {
  return { theme: loginStore.theme };
})(observer(SimpleNav));
