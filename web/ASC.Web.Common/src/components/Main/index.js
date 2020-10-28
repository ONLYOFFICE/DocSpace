import React from "react";
import styled from "styled-components";
import { isIOS } from "react-device-detect";
import { utils} from "asc-web-components";
const { tablet,smallTablet } = utils.device;
const StyledMain = styled.main`
  height: ${isIOS ? "calc(var(--vh, 1vh) * 100)" : "calc(100vh - 56px)"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;

  @media ${tablet} {
    height: auto;
    min-height: 100%;
  }

  @media ${smallTablet} {
    min-height: auto;
  }
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
