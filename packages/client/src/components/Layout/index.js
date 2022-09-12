import React, { useEffect, useState } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import MobileLayout from "./MobileLayout";
import { size } from "@docspace/components/utils/device";
import {
  isIOS,
  isFirefox,
  isSafari,
  isMobile,
  isMobileOnly,
  isChrome,
  isTablet,
} from "react-device-detect";
import { inject, observer } from "mobx-react";

const StyledContainer = styled.div`
  width: 100%;
  height: ${(props) => props.contentHeight};
  /* height: ${(props) =>
    (props.isTabletView || isMobileOnly) && !isFirefox
      ? `${props.contentHeight}px`
      : "100vh"}; */

  #customScrollBar {
    z-index: 0;
    > .scroll-body {
      -webkit-user-select: none;
    }
  }
`;

const Layout = (props) => {
  const { children, isTabletView, setIsTabletView, isBannerVisible } = props;

  const [contentHeight, setContentHeight] = useState();
  const [isPortrait, setIsPortrait] = useState();

  const intervalTime = 100;
  const endTimeout = 300;
  let intervalHandler;
  let timeoutHandler;

  useEffect(() => {
    setIsPortrait(window.innerHeight > window.innerWidth);
  });
  useEffect(() => {
    const isTablet = window.innerWidth <= size.tablet;
    setIsTabletView(isTablet);

    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(onWidthChange);

    return () => {
      mediaQuery.removeListener(onWidthChange);
      if (intervalHandler) clearInterval(intervalHandler);
      if (timeoutHandler) clearTimeout(timeoutHandler);
    };
  }, []);

  useEffect(() => {
    if (isTabletView || isMobile) {
      if (isIOS && isSafari) window.addEventListener("resize", onResize);
      else window.addEventListener("orientationchange", onOrientationChange);
      changeRootHeight();
    }

    return () => {
      if (isTabletView || isMobile) {
        if (isIOS && isSafari) window.removeEventListener("resize", onResize);
        else
          window.removeEventListener("orientationchange", onOrientationChange);
      }
    };
  }, [isTabletView]);

  useEffect(() => {
    changeRootHeight();
  }, [isBannerVisible]);

  useEffect(() => {
    const htmlEl = document.getElementsByTagName("html")[0];
    const bodyEl = document.getElementsByTagName("body")[0];

    if (isMobileOnly || (isTablet && isChrome)) {
      htmlEl.style.height = bodyEl.style.height = "100%";
      htmlEl.style.overflow = "hidden";
    }

    if (isMobileOnly) {
      bodyEl.style.overflow = "auto";
    }

    if (isTablet) {
      bodyEl.style.overflow = "hidden";
    }
  }, []);

  const onWidthChange = (e) => {
    const { matches } = e;
    setIsTabletView(matches);
  };

  const onResize = () => {
    changeRootHeight();
  };
  const onOrientationChange = () => {
    changeRootHeight();
  };
  const changeRootHeight = () => {
    intervalHandler && clearInterval(intervalHandler);
    timeoutHandler && clearTimeout(timeoutHandler);

    let lastInnerHeight, noChangeCount;

    const updateHeight = () => {
      const correctorMobileChrome = 57; // ios
      //const correctorTabletSafari = 71; // ios

      clearInterval(intervalHandler);
      clearTimeout(timeoutHandler);

      intervalHandler = null;
      timeoutHandler = null;

      let height = "100vh";
      const windowHeight = window.innerHeight;

      if (isMobileOnly && isIOS && isChrome) {
        if (window.innerHeight < window.innerWidth && isPortrait) {
          height = window.screen.availWidth - correctorMobileChrome + "px";
        }
      }

      // if (isTablet && isIOS && isSafari) {
      //   if (
      //     window.innerHeight < window.innerWidth &&
      //     window.innerWidth > 1024
      //   ) {
      //     height = window.screen.availHeight - correctorTabletSafari;
      //   }
      // }

      const bannerHeight = isBannerVisible ? 80 : 0;
      let vh = (windowHeight - bannerHeight) * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);
      setContentHeight(height);
    };
    intervalHandler = setInterval(() => {
      //console.log("changeRootHeight setInterval"); TODO: need to refactoring
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

    timeoutHandler = setTimeout(() => {
      updateHeight();
    }, endTimeout);
  };

  return (
    <StyledContainer
      className="Layout"
      isTabletView={isTabletView}
      contentHeight={contentHeight}
    >
      {isMobileOnly ? <MobileLayout {...props} /> : children}
    </StyledContainer>
  );
};

Layout.propTypes = {
  isTabletView: PropTypes.bool,
  children: PropTypes.any,
  setIsTabletView: PropTypes.func,
};

export default inject(({ auth, bannerStore }) => {
  return {
    isTabletView: auth.settingsStore.isTabletView,
    setIsTabletView: auth.settingsStore.setIsTabletView,
    isBannerVisible: bannerStore.isBannerVisible,
  };
})(observer(Layout));
