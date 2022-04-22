import { Base } from "@appserver/components/themes";
import { isTablet, mobile, tablet } from "@appserver/components/utils/device";
import { inject } from "mobx-react";
import PropTypes from "prop-types";
import React, { useEffect } from "react";
import styled from "styled-components";
import CrossIcon from "@appserver/components/public/static/images/cross.react.svg";

import { isMobile, isMobileOnly } from "react-device-detect";

const StyledInfoPanelWrapper = styled.div.attrs(({ id }) => ({
  id: id,
}))`
  height: auto;
  width: auto;
  background: rgba(6, 22, 38, 0.2);
  backdrop-filter: blur(18px);

  @media ${tablet} {
    z-index: 309;
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
  }
`;

const StyledInfoPanel = styled.div`
  height: 100%;
  width: 400px;
  background-color: ${(props) => props.theme.infoPanel.backgroundColor};
  border-left: ${(props) => `1px solid ${props.theme.infoPanel.borderColor}`};
  display: flex;
  flex-direction: column;

  @media ${tablet} {
    position: absolute;
    border: none;
    right: 0;
    width: 480px;
    max-width: calc(100vw - 69px);
  }

  @media ${mobile} {
    bottom: 0;
    height: 80%;
    width: 100vw;
    max-width: 100vw;
  }
`;

const StyledControlContainer = styled.div`
  display: none;

  width: 24px;
  height: 24px;
  position: absolute;

  border-radius: 100px;
  cursor: pointer;

  align-items: center;
  justify-content: center;
  z-index: 450;
  background: ${(props) => props.theme.catalog.control.background};

  @media ${tablet} {
    display: flex;

    top: 18px;
    left: -34px;
  }

  ${isMobile &&
  css`
    display: flex;

    top: 18px;
    left: -34px;
  `}

  @media ${mobile} {
    position: fixed;
    display: flex;

    top: 30px;
    right: 10px;
  }

  ${isMobileOnly &&
  css`
    position: fixed !important;
    display: flex;

    top: 30px !important;
    right: 10px !important;
  `}
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 12px;
  height: 12px;
  z-index: 455;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

const InfoPanel = ({ children, isVisible, setIsVisible }) => {
  if (!isVisible) return null;

  const closeInfoPanel = () => setIsVisible(false);

  useEffect(() => {
    const onMouseDown = (e) => {
      if (e.target.id === "InfoPanelWrapper") closeInfoPanel();
    };

    if (isTablet()) document.addEventListener("mousedown", onMouseDown);
    return () => document.removeEventListener("mousedown", onMouseDown);
  }, []);

  return (
    <StyledInfoPanelWrapper className="info-panel" id="InfoPanelWrapper">
      <StyledInfoPanel>
        <StyledControlContainer onClick={closeInfoPanel}>
          <StyledCrossIcon />
        </StyledControlContainer>

        {children}
      </StyledInfoPanel>
    </StyledInfoPanelWrapper>
  );
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

export default inject(({ infoPanelStore }) => {
  let isVisible = false;
  let setIsVisible = () => {};

  if (infoPanelStore) {
    isVisible = infoPanelStore.isVisible;
    setIsVisible = infoPanelStore.setIsVisible;
  }

  return {
    isVisible,
    setIsVisible,
  };
})(InfoPanel);
