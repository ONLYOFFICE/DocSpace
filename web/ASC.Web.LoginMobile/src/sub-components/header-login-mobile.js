import React from "react";
import styled from "styled-components";

import Logo from "../../public/images/nav.logo.opened.react.svg";

const StyledHeader = styled("div")`
  width: 100%;
  height: 56px;
  background-color: #0f4071;

  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 0 20px 0 6px;
    display: block;
    padding: 16px 0px 0 38px;
  }
`;

const Header = () => {
  return (
    <StyledHeader>
      <Logo className="header-logo-icon" />
    </StyledHeader>
  );
};

export default Header;
