import { Base } from "@docspace/components/themes";
import {
  isTablet,
  isMobile as isMobileUtils,
  tablet,
  isDesktop,
} from "@docspace/components/utils/device";
import { inject } from "mobx-react";
import PropTypes from "prop-types";
import React, { useEffect } from "react";
import styled, { css } from "styled-components";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";

import { isMobile, isMobileOnly } from "react-device-detect";
import { Portal } from "@docspace/components";

const StyledInfoPanelWrapper = styled.div.attrs(({ id }) => ({
  id: id,
}))`
  user-select: none;
  height: auto;
  width: auto;
  background: ${(props) => props.theme.infoPanel.blurColor};
  backdrop-filter: blur(3px);
  z-index: 300;
  @media ${tablet} {
    z-index: 309;
    position: fixed;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
  }

  ${isMobile &&
  css`
    @media ${tablet} {
      z-index: 309;
      position: fixed;
      top: 0;
      bottom: 0;
      left: 0;
      right: 0;
    }
  `}
`;

const StyledInfoPanel = styled.div`
  height: 100%;
  width: 400px;
  background-color: ${(props) => props.theme.infoPanel.backgroundColor};
  border-left: ${(props) => `1px solid ${props.theme.infoPanel.borderColor}`};
  display: flex;
  flex-direction: column;

  .scroll-body {
    padding-bottom: 20px;
  }

  @media ${tablet} {
    position: absolute;
    border: none;
    right: 0;
    width: 480px;
    max-width: calc(100vw - 69px);
  }

  ${isMobile &&
  css`
    @media ${tablet} {
      position: absolute;
      border: none;
      right: 0;
      width: 480px;
      max-width: calc(100vw - 69px);
    }
  `}

  @media (max-width: 428px) {
    bottom: 0;
    height: calc(100% - 64px);
    width: 100vw;
    max-width: 100vw;
  }
`;

const StyledControlContainer = styled.div`
  display: none;

  width: 17px;
  height: 17px;
  position: absolute;

  cursor: pointer;

  align-items: center;
  justify-content: center;
  z-index: 450;

  @media ${tablet} {
    display: flex;

    top: 18px;
    left: -27px;
  }

  ${isMobile &&
  css`
    @media ${tablet} {
      display: flex;
      top: 18px;
      left: -27px;
    }
  `}

  @media (max-width: 428px) {
    display: flex;

    top: -27px;
    right: 10px;
    left: unset;
  }
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 17px;
  height: 17px;
  z-index: 455;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

const InfoPanel = ({
  children,
  isVisible,
  isMobileHidden,
  setIsVisible,
  canDisplay,
  viewAs,
}) => {
  const closeInfoPanel = () => setIsVisible(false);

  useEffect(() => {
    const onMouseDown = (e) => {
      if (e.target.id === "InfoPanelWrapper") closeInfoPanel();
    };

    if (viewAs === "row" || isTablet() || isMobile || isMobileUtils())
      document.addEventListener("mousedown", onMouseDown);

    window.onpopstate = () => {
      if (!isDesktop() && isVisible) closeInfoPanel();
    };

    return () => {
      document.removeEventListener("mousedown", onMouseDown);
    };
  }, []);

  const infoPanelComponent = (
    <StyledInfoPanelWrapper
      isRowView={viewAs === "row"}
      className="info-panel"
      id="InfoPanelWrapper"
    >
      <StyledInfoPanel isRowView={viewAs === "row"}>
        <StyledControlContainer
          isRowView={viewAs === "row"}
          onClick={closeInfoPanel}
        >
          <StyledCrossIcon />
        </StyledControlContainer>

        {children}
      </StyledInfoPanel>
    </StyledInfoPanelWrapper>
  );

  const renderPortalInfoPanel = () => {
    console.log(isMobileHidden);
    const rootElement = document.getElementById("root");

    return (
      <Portal
        element={infoPanelComponent}
        appendTo={rootElement}
        visible={isVisible && !isMobileHidden}
      />
    );
  };

  return !isVisible ||
    !canDisplay ||
    ((isTablet() || isMobile || isMobileUtils()) && isMobileHidden)
    ? null
    : isMobileOnly
    ? renderPortalInfoPanel()
    : infoPanelComponent;
};

InfoPanel.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any,
  ]),
  isVisible: PropTypes.bool,
};

StyledInfoPanelWrapper.defaultProps = { theme: Base };
StyledInfoPanel.defaultProps = { theme: Base };
InfoPanel.defaultProps = { theme: Base };

export default inject(({ auth }) => {
  const { isVisible, isMobileHidden, setIsVisible, getCanDisplay } =
    auth.infoPanelStore;

  const canDisplay = getCanDisplay();

  return {
    isVisible,
    isMobileHidden,
    setIsVisible,
    canDisplay,
  };
})(InfoPanel);
