import React from "react";
import styled, { css } from "styled-components";
import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import { isMobile } from "react-device-detect";
import { tablet } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

const StyledContainer = styled.div`
  margin-left: 20px;
  display: flex;
  align-items: center;

  height: 32px;

  .add-button {
    margin-right: 16px;
    min-width: 15px;

    @media ${tablet} {
      display: none;
    }

    ${isMobile &&
    css`
      display: none;
    `}
  }

  .option-button {
    margin-right: 16px;
    min-width: 15px;
  }

  .trash-button {
    margin-right: 16px;
    min-width: 15px;
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: ${(props) => (props.isInfoPanelVisible ? "none" : "flex")};

  align-items: center;
  align-self: center;
  justify-content: center;
  margin-left: auto;

  margin-bottom: 1px;

  @media ${tablet} {
    display: none;
    margin-left: ${(props) => (props.isRootFolder ? "auto" : "0")};
  }

  ${isMobile &&
  css`
    margin-left: ${(props) => (props.isRootFolder ? "auto" : "0")};
  `}

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;

    display: flex;

    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background-color: ${(props) =>
      props.isInfoPanelVisible
        ? props.theme.infoPanel.sectionHeaderToggleBgActive
        : props.theme.infoPanel.sectionHeaderToggleBg};

    path {
      fill: ${(props) =>
        props.isInfoPanelVisible
          ? props.theme.infoPanel.sectionHeaderToggleIconActive
          : props.theme.infoPanel.sectionHeaderToggleIcon};
    }
  }
`;
StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

const ToggleInfoPanelButton = ({
  isRootFolder,
  isInfoPanelVisible,
  toggleInfoPanel,
}) => {
  return (
    <StyledInfoPanelToggleWrapper
      isRootFolder={isRootFolder}
      isInfoPanelVisible={isInfoPanelVisible}
    >
      <div className="info-panel-toggle-bg">
        <IconButton
          className="info-panel-toggle"
          iconName="images/panel.react.svg"
          size="16"
          isFill={true}
          onClick={toggleInfoPanel}
        />
      </div>
    </StyledInfoPanelToggleWrapper>
  );
};

export default ToggleInfoPanelButton;
