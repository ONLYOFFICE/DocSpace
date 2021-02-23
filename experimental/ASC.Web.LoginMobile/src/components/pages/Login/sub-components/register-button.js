import React from "react";
import styled from "styled-components";

import { Box, Link } from "ASC.Web.Components";

const StyledRegisterButton = styled(Box)`
  position: absolute;
  bottom: 0;
  left: 0;
  height: 66px;
  width: 100%;
  background-color: #f8f9f9;

  .link-registration {
    display: block;
    width: min-content;
    color: #316daa;
    margin: 24px auto 0 auto;
  }
`;

const RegisterButton = ({ title, onClick }) => {
  return (
    <StyledRegisterButton>
      <Link className="link-registration" fontSize="13px" onClick={onClick}>
        {title}
      </Link>
    </StyledRegisterButton>
  );
};

export default RegisterButton;
