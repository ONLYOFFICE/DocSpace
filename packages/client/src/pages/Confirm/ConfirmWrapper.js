import React from "react";
import styled, { css } from "styled-components";
import { isIOS, isFirefox, isMobileOnly } from "react-device-detect";

const StyledWrapper = styled.div`
  height: ${isIOS && !isFirefox ? "calc(var(--vh, 1vh) * 100)" : "100vh"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;

  ${isMobileOnly &&
  css`
    height: auto;
    min-height: 100%;
    width: 100%;
  `}
`;

const ConfirmWrapper = (props) => {
  const { children, className } = props;
  return <StyledWrapper className={className}>{children}</StyledWrapper>;
};

export default ConfirmWrapper;
