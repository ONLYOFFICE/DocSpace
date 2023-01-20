import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { hugeMobile } from "@docspace/components/utils/device";
import { Dark } from "@docspace/components/themes";

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

const SimpleNav = ({ theme, logoUrls }) => {
  const logo = Object.values(logoUrls)[0];
  const logoUrl = !theme.isBase ? logo.path.dark : logo.path.light;

  return (
    <StyledNav id="login-header" theme={theme}>
      <img src={logoUrl} />
    </StyledNav>
  );
};

export default inject(({ loginStore }) => {
  return { theme: loginStore.theme };
})(observer(SimpleNav));
