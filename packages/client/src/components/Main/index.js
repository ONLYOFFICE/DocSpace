import React from "react";
import styled, { css } from "styled-components";
import { isIOS, isFirefox, isMobileOnly } from "react-device-detect";

import { mobile } from "@docspace/components/utils/device";

const StyledMain = styled.main`
  height: ${isIOS && !isFirefox ? "calc(var(--vh, 1vh) * 100)" : "100vh"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: column;
  box-sizing: border-box;

  .main-container {
    width: 100%;
    height: 100%;

    display: flex;
    flex-direction: row;
    box-sizing: border-box;
  }

  ${!isMobileOnly &&
  css`
    @media ${mobile} {
      height: ${isIOS && !isFirefox
        ? "calc(var(--vh, 1vh) * 100)"
        : "calc(100vh - 64px)"};
    }
  `}

  ${isMobileOnly &&
  css`
    height: auto;
    max-height: 100%;
    width: 100%;
  `}
`;

const Main = React.memo((props) => {
  //console.log("Main render");

  return <StyledMain className="main" {...props} />;
});

Main.displayName = "Main";

export default Main;
