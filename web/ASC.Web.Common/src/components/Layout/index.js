import React, { useEffect, useState } from "react";
import styled, { css } from "styled-components";
import MobileLayout from "./MobileLayout";
import { utils } from "asc-web-components";
import {
  isIOS,
  isFirefox,
  isSafari,
  isMobile,
  isMobileOnly,
  isChrome,
} from "react-device-detect";

import { connect } from "react-redux";
import store from "../../store";

const { setIsTabletView } = store.auth.actions;
const { getIsTabletView } = store.auth.selectors;

const { size } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  height: ${(props) =>
    props.isTabletView && !isFirefox
      ? isMobile
        ? `${props.contentHeight}px`
        : "100vh"
      : "100vh"};

  #desktopScroll {
    > .scroll-body {
      position: ${(props) =>
        props.isTabletView ? "static" : "absolute"} !important;

      padding-right: ${(props) =>
        props.isTabletView ? "1px" : "16px"} !important;

      height: ${isMobile ? "100%" : "calc(100% + 20px)"};
    }
    .nav-thumb-horizontal {
      ${(props) =>
        props.isTabletView &&
        css`
          position: fixed !important;
          bottom: 4px;
        `}
    }
  }
`;

const Layout = (props) => {
  const { children, isTabletView, setIsTabletView } = props;

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
    mediaQuery.addEventListener("change", onWidthChange);

    return () => {
      mediaQuery.removeEventListener("change", onWidthChange);
      if (intervalHandler) clearInterval(intervalHandler);
      if (timeoutHandler) clearTimeout(timeoutHandler);
    };
  }, []);

  useEffect(() => {
    if (isTabletView) {
      if (isIOS && isSafari) window.addEventListener("resize", onResize);
      else window.addEventListener("orientationchange", onOrientationChange);
      changeRootHeight();
    }

    return () => {
      if (isTabletView) {
        if (isIOS && isSafari) window.removeEventListener("resize", onResize);
        else
          window.removeEventListener("orientationchange", onOrientationChange);
      }
    };
  }, [isTabletView]);

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
      clearInterval(intervalHandler);
      clearTimeout(timeoutHandler);

      intervalHandler = null;
      timeoutHandler = null;

      let height = window.innerHeight;

      if (isMobileOnly && isIOS && isChrome) {
        if (window.innerHeight < window.innerWidth && isPortrait) {
          height = window.screen.availWidth - 57;
        }
      }
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
      <MobileLayout {...props} />
    </StyledContainer>
  );
};

const mapStateToProps = (state) => {
  return {
    isTabletView: getIsTabletView(state),
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    setIsTabletView: (isTabletView) => dispatch(setIsTabletView(isTabletView)),
  };
};
export default connect(mapStateToProps, mapDispatchToProps)(Layout);
