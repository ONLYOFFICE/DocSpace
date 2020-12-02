import React, { useEffect, useState } from "react";
import styled from "styled-components";
import MobileLayout from "./MobileLayout";
import { utils } from "asc-web-components";
import { isIOS, isFirefox, isChrome, isMobile } from "react-device-detect";

const { size } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  height: ${isIOS && !isFirefox
    ? "calc(var(--vh, 1vh) * 100 + 57px)"
    : "100vh"};
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
    if (isMobile) {
      window.addEventListener("resize", resizeHandler);

      resizeHandler();
    }

    return () => {
      if (isMobile) {
        window.removeEventListener("resize", resizeHandler);
      }
    };
  }, []);

  const resizeHandler = () => {
    const intervalTime = 100;
    const endTimeout = 500;

    let interval, timeout, lastInnerHeight, noChangeCount;

    const updateHeight = () => {
      clearInterval(interval);
      clearTimeout(timeout);

      interval = null;
      timeout = null;

      const vh = (window.innerHeight - 57) * 0.01;

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
        <MobileLayout {...props} />
      ) : (
        children
      )}
    </StyledContainer>
  );
};
export default Layout;
