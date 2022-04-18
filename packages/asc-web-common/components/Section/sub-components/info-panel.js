import IconButton from "@appserver/components/icon-button";
import { Base } from "@appserver/components/themes";
import { isTablet, mobile, tablet } from "@appserver/components/utils/device";
import { inject } from "mobx-react";
import PropTypes from "prop-types";
import React, { useEffect } from "react";
import styled from "styled-components";

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

const StyledCloseButtonWrapper = styled.div`
  position: absolute;
  display: none;
  background-color: ${(props) => props.theme.infoPanel.closeButtonBg};
  padding: ${(props) => props.theme.infoPanel.closeButtonWrapperPadding};
  border-radius: 50%;

  .info-panel-button {
    width: auto;
    svg {
      width: ${(props) => props.theme.infoPanel.closeButtonSize};
      height: ${(props) => props.theme.infoPanel.closeButtonSize};
    }
    path {
      fill: ${(props) => props.theme.infoPanel.closeButtonIcon};
    }
  }

  @media ${tablet} {
    display: block;
    top: 0;
    left: 0;
    margin-top: 18px;
    margin-left: -34px;
  }
  @media ${mobile} {
    right: 0;
    left: auto;
    margin-top: -34px;
    margin-right: 10px;
  }
`;

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
        <StyledCloseButtonWrapper>
          <IconButton
            onClick={closeInfoPanel}
            iconName="/static/images/cross.react.svg"
            className="info-panel-button"
          />
        </StyledCloseButtonWrapper>
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
StyledCloseButtonWrapper.defaultProps = { theme: Base };
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
