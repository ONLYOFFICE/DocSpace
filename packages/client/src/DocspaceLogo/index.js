import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { hugeMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";

const StyledWrapper = styled.div`
  .logo-wrapper {
    width: 100%;
    height: 46px;

    svg {
      path:last-child {
        fill: ${(props) => props.theme.client.home.logoColor};
      }
    }
    @media ${hugeMobile} {
      display: none;
    }
  }
`;

const DocspaceLogo = (props) => {
  const { className } = props;
  if (isMobileOnly) return <></>;

  return (
    <StyledWrapper>
      <ReactSVG
        src="/static/images/docspace.big.react.svg"
        className={`logo-wrapper ${className}`}
      />
    </StyledWrapper>
  );
};

export default DocspaceLogo;
