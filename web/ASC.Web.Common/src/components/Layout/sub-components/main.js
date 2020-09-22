import React from "react";
import styled from "styled-components";

const StyledMain = styled.main`
  height: 100vh;
  height: calc(var(--vh, 1vh) * 100);
  padding: ${props => (props.fullscreen ? "0" : "56px 0 0 0")};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;
`;

const Main = React.memo(props => {
  const vh = window.innerHeight * 0.01;
  document.documentElement.style.setProperty('--vh', `${vh}px`);

  //console.log("Main render");
  return <StyledMain {...props} />;
});

Main.displayName = "Main";

export default Main;
