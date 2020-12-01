import React, { useEffect, useState } from "react";
import styled from "styled-components";
import MobileLayout from "./MobileLayout";
import { utils } from "asc-web-components";
import {
  isIOS,
  isMobileSafari,
  isFirefox,
  isChrome,
} from "react-device-detect";

const { size } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  height: ${isIOS && !isFirefox
    ? !isChrome
      ? "calc(var(--vh, 1vh) * 100 + 57px)"
      : "var(--vh, 100vh)"
    : "100vh "};
`;

const Layout = (props) => {
  const { children } = props;
  const isTablet = window.innerWidth <= size.tablet;

  const [windowWidth, setWindowWidth] = useState({
    matches: isTablet,
  });

  useEffect(() => {
    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(setWindowWidth);

    return () => mediaQuery.removeListener(setWindowWidth);
  }, []);

  useEffect(() => {
    window.addEventListener("resize", resizeHandler);

    resizeHandler();

    return () => {
      window.removeEventListener("resize", resizeHandler);
    };
  }, []);

  const resizeHandler = () => {
    let vh;
    if (isIOS) {
      if (isMobileSafari) {
        vh = (window.innerHeight - 57) * 0.01;
      }
      if (isChrome) {
        vh = window.innerHeight;
      }
    } else {
      vh = (window.innerHeight - 57) * 0.01;
    }

    document.documentElement.style.setProperty("--vh", `${vh}px`);
  };

  return (
    <StyledContainer className="Layout">
      {windowWidth && windowWidth.matches ? (
        <MobileLayout {...props} className="mobile-layout" />
      ) : (
        children
      )}
    </StyledContainer>
  );
};
export default Layout;
