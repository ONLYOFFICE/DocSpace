import React, { useEffect, useState } from "react";
import styled from "styled-components";
import MobileLayout from "./MobileLayout";
import { utils } from "asc-web-components";
import { isIOS, isFirefox, isChrome, isMobile } from "react-device-detect";

const { size, tablet } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  height: ${isIOS && !isFirefox
    ? !isChrome
      ? "calc(var(--vh, 1vh) * 100 + 57px)"
      : "calc(var(--vh, 1vh) * 100 + 57px)"
    : "100vh "};
  @media ${tablet} {
    margin: auto;
  }
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
    if (!isMobile) return;
    const intervalTime = 100;
    const endTimeout = 300;

    let interval, timeout, lastInnerHeight, noChangeCount;

    const updateHeight = () => {
      let vh;
      clearInterval(interval);
      clearTimeout(timeout);

      interval = null;
      timeout = null;

      vh = (window.innerHeight - 57) * 0.01;
      if (isIOS && isChrome) {
        if (window.innerHeight > window.innerWidth) {
          document.documentElement.style.setProperty("--lm", "-40px");
        }
      }
      document.documentElement.style.setProperty("--vh", `${vh}px`);
    };
    interval = setInterval(() => {
      if (window.innerHeight === lastInnerHeight) {
        noChangeCount++;

        if (noChangeCount === intervalTime) {
          updateHeight();
        }
      } else {
        lastInnerHeight = window.innerHeight;
        noChangeCount = 0;
      }
    });

    timeout = setTimeout(() => {
      updateHeight();
    }, endTimeout);
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
