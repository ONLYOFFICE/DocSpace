import React, { useEffect, useState } from "react";
import styled from "styled-components";
import MobileLayout from "./MobileLayout";
import { utils } from "asc-web-components";
import { isIOS, isFirefox, isChrome, isSafari } from "react-device-detect";

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

  console.log(size.tablet);

  useEffect(() => {
    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(setWindowWidth);

    return () => mediaQuery.removeListener(setWindowWidth);
  }, []);

  useEffect(() => {
    if (isTablet) {
      if (isIOS && isSafari) window.addEventListener("resize", resizeHandler);
      else window.addEventListener("orientationchange", resizeHandler);
      resizeHandler();
    }

    return () => {
      if (isTablet) {
        if (isIOS && isSafari)
          window.removeEventListener("resize", resizeHandler);
        else window.removeEventListener("orientationchange", resizeHandler);
      }
    };
  }, []);

  const resizeHandler = () => {
    const intervalTime = 100;
    const endTimeout = 300;

    let interval, timeout, lastInnerHeight, noChangeCount;

    const updateHeight = () => {
      clearInterval(interval);
      clearTimeout(timeout);

      interval = null;
      timeout = null;

      let vh = (window.innerHeight - 57) * 0.01;

      if (isChrome) {
        if (window.innerHeight < window.innerWidth) {
          vh = (window.innerHeight + 57) * 0.01;
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
        <MobileLayout {...props} windowWidth={windowWidth} />
      ) : (
        children
      )}
    </StyledContainer>
  );
};
export default Layout;
