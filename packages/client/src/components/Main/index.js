import React from "react";
import styled, { css } from "styled-components";
import { isIOS, isFirefox, isMobileOnly } from "react-device-detect";

const StyledMain = styled.main`
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

const Main = React.memo((props) => {
  // const orientationChanged = React.useCallback(() => {
  //   if (isIOS && !isFirefox) {
  //     let vh = (window.innerHeight - 48) * 0.01;
  //     document.documentElement.style.setProperty("--vh", `${vh}px`);
  //   }
  // }, []);

  // React.useEffect(() => {
  //   orientationChanged();
  //   if (isIOS && !isFirefox) {
  //     window.addEventListener("resize", orientationChanged);
  //   }
  //   return () => window.removeEventListener("resize", orientationChanged);
  // }, []);

  //console.log("Main render");

  return <StyledMain className="main" {...props} />;
});

/*Main.defaultProps = {
  fullscreen: false
};*/

Main.displayName = "Main";

export default Main;
