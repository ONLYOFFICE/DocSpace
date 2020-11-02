import React from "react";
import styled from "styled-components";
import { isIOS, isFirefox } from "react-device-detect";

const StyledMain = styled.main`
  height: ${isIOS && !isFirefox
    ? "calc(var(--vh, 1vh) * 100)"
    : "calc(100vh - 56px)"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;
`;

const Main = React.memo((props) => {
  const vh = (window.innerHeight - 57) * 0.01;
  document.documentElement.style.setProperty("--vh", `${vh}px`);
  //console.log("Main render");
  return <StyledMain {...props} />;
});

/*Main.defaultProps = {
  fullscreen: false
};*/

Main.displayName = "Main";

export default Main;
