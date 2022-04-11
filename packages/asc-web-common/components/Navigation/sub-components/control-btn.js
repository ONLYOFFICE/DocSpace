import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import { isMobile } from "react-device-detect";
import { tablet } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

const StyledContainer = styled.div`
  margin-left: 20px;
  display: flex;
  align-items: center;

  .add-button {
    margin-right: 12px;
    min-width: 17px;

    ${(props) =>
      !props.isDropBox &&
      css`
        @media ${tablet} {
          display: none;
        }
      `}

    ${isMobile &&
    css`
      ${(props) => !props.isDropBox && "display: none"};
    `}
  }

  .option-button {
    margin-right: 15px;
    min-width: 17px;
  }

  .trash-button {
    min-width: 17px;
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;
  align-items: center;
  align-self: center;
  justify-content: center;
  margin-left: auto;

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

const ControlButtons = ({
  personal,
  isDropBox,
  isRootFolder,
  canCreate,
  getContextOptionsFolder,
  getContextOptionsPlus,
  isRecycleBinFolder,
  isEmptyFilesList,
  clearTrash,
  isInfoPanelVisible,
  toggleInfoPanel,
}) => {
  return (
    <StyledContainer isDropBox={isDropBox}>
      {!isRootFolder && canCreate ? (
        <>
          <ContextMenuButton
            className="add-button"
            directionX="right"
            iconName="images/plus.svg"
            size={17}
            isFill
            getData={getContextOptionsPlus}
            isDisabled={false}
          />
          {!personal && (
            <ContextMenuButton
              className="option-button"
              directionX="right"
              iconName="images/vertical-dots.react.svg"
              size={17}
              isFill
              getData={getContextOptionsFolder}
              isDisabled={false}
            />
          )}
        </>
      ) : canCreate ? (
        <ContextMenuButton
          className="add-button"
          directionX="right"
          iconName="images/plus.svg"
          size={17}
          isFill
          getData={getContextOptionsPlus}
          isDisabled={false}
        />
      ) : isRecycleBinFolder && !isEmptyFilesList ? (
        <IconButton
          iconName="images/clear.active.react.svg"
          size={17}
          isFill={true}
          onClick={clearTrash}
          className="trash-button"
        />
      ) : (
        <></>
      )}
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
    </StyledContainer>
  );
};

ControlButtons.propTypes = {
  personal: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  canCreate: PropTypes.bool,
  getContextOptionsFolder: PropTypes.func,
  getContextOptionsPlus: PropTypes.func,
};

export default React.memo(ControlButtons);
