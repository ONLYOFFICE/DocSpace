import React from "react";
import styled, { css } from "styled-components";
import { isIOS, isFirefox, isMobile } from "react-device-detect";

const StyledMain = styled.main`
  height: ${(props) =>
    isIOS && !isFirefox
      ? "calc(var(--vh, 1vh) * 100)"
      : props.isDesktop
      ? "100vh"
      : "calc(100vh - 48px)"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;

  ${isMobile &&
  css`
    height: auto;
    min-height: 100%;
    width: 100%;
  `}
`;

const Main = React.memo((props) => {
  if (isIOS && !isFirefox) {
    const vh = (window.innerHeight - 57) * 0.01;
    document.documentElement.style.setProperty("--vh", `${vh}px`);
  }
  //console.log("Main render");
  return <StyledMain className="main" {...props} />;
});

/*Main.defaultProps = {
  fullscreen: false
};*/

Main.displayName = "Main";

export default Main;
