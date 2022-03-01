import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Resizable } from "re-resizable";
import { isMobile, isMobileOnly, isTablet } from "react-device-detect";
import {
  mobile,
  tablet,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
  isDesktop as isDesktopUtils,
} from "@appserver/components/utils/device";

import { Base } from "@appserver/components/themes";

const StyledCatalog = styled.div`
  position: relative;

  background: ${(props) => props.theme.catalog.background};

  ${isMobile &&
  css`
    margin-top: 48px;
  `}

  @media ${mobile} {
    position: fixed;
    margin-top: 16px;
    height: calc(100vh - 64px) !important;
    z-index: 400;
  }

  ${isMobileOnly &&
  css`
    position: fixed;
    margin-top: 64px;
    height: calc(100vh - 64px) !important;
  `}

  z-index: ${(props) =>
    props.showText && (isMobileOnly || isMobileUtils()) ? "201" : "100"};

  .resizable-block {
    display: flex;
    flex-direction: column;

    min-width: ${(props) => (props.showText ? "256px" : "52px")};
    width: ${(props) => (props.showText ? "256px" : "52px")};

    height: calc(100% - 44px) !important;

    background: ${(props) => props.theme.catalog.background};
    overflow-y: auto;
    overflow-x: hidden;
    scrollbar-width: none;
    padding-bottom: 0px;

    &::-webkit-scrollbar {
      width: 0;
      height: 0;
    }
    .resizable-border {
      div {
        cursor: ew-resize !important;
      }
    }
    @media ${tablet} {
      min-width: ${(props) => (props.showText ? "240px" : "52px")};
      max-width: ${(props) => (props.showText ? "240px" : "52px")};
      .resizable-border {
        display: none;
      }
    }

    @media ${mobile} {
      display: ${(props) => (props.catalogOpen ? "flex" : "none")};
      min-width: 100vw;
      width: 100vw;
      height: calc(100vh - 64px) !important;
      margin: 0;
      padding: 0;
      padding-bottom: 0px;
    }

    ${isTablet &&
    css`
      min-width: ${(props) => (props.showText ? "240px" : "52px")};
      max-width: ${(props) => (props.showText ? "240px" : "52px")};
      .resizable-border {
        display: none;
      }
    `}

    ${isMobileOnly &&
    css`
      display: ${(props) => (props.catalogOpen ? "flex" : "none")};
      min-width: 100vw !important;
      width: 100vw;
      height: calc(100vh - 64px) !important;
      margin: 0;
      padding: 0;
      padding-bottom: 0px;
    `}
  }
`;

StyledCatalog.defaultProps = { theme: Base };

const Catalog = (props) => {
  const { showText, setShowText, catalogOpen, children, ...rest } = props;
  const refTimer = React.useRef(null);
  const enable = {
    top: false,
    right: !isMobile,
    bottom: false,
    left: false,
  };

  const hideText = React.useCallback((event) => {
    event.preventDefault;
    setShowText(false);
  }, []);

  React.useEffect(() => {
    if (isMobileOnly) {
      window.addEventListener("popstate", hideText);
      return () => window.removeEventListener("popstate", hideText);
    }
  }, [hideText]);
  React.useEffect(() => {
    window.addEventListener("resize", sizeChangeHandler);
    return () => window.removeEventListener("resize", sizeChangeHandler);
  });

  React.useEffect(() => {
    sizeChangeHandler();
  }, []);
  const sizeChangeHandler = () => {
    clearTimeout(refTimer.current);

    refTimer.current = setTimeout(() => {
      console.log(window.innerWidth === 375);
      if (isMobileOnly || isMobileUtils() || window.innerWidth === 375)
        props.setShowText(true);
      if (
        ((isTabletUtils() && window.innerWidth !== 375) || isMobile) &&
        !isMobileOnly
      )
        props.setShowText(false);
      if (isDesktopUtils() && !isMobile) props.setShowText(true);
    }, 100);
  };

  return (
    <StyledCatalog showText={showText} catalogOpen={catalogOpen} {...rest}>
      <Resizable
        defaultSize={{
          width: 256,
        }}
        enable={enable}
        className="resizable-block"
        handleWrapperClass="resizable-border not-selectable"
      >
        {children}
      </Resizable>
    </StyledCatalog>
  );
};

Catalog.propTypes = {
  showText: PropTypes.bool,
  setShowText: PropTypes.func,
  children: PropTypes.any,
};

export default Catalog;
