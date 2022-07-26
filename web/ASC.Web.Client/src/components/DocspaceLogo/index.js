import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { hugeMobile } from "@appserver/components/utils/device";
import { isMobileOnly } from "react-device-detect";

const StyledWrapper = styled.div`
  .logo-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 46px;
    padding-bottom: 64px;

    @media ${hugeMobile} {
      display: none;
    }
  }
`;

const DocspaceLogo = () => {
  if (isMobileOnly) return <></>;

  return (
    <StyledWrapper>
      <ReactSVG
        src="/static/images/docspace.big.react.svg"
        className="logo-wrapper"
      />
    </StyledWrapper>
  );
};

export default DocspaceLogo;
