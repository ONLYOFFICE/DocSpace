import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import { isMobile } from "react-device-detect";
import { tablet } from "@appserver/components/utils/device";

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
    margin-left: auto;
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
  margin-left: ${({ isRootFolder }) => (isRootFolder ? "auto" : "none")};

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background-color: ${(props) =>
      props.isInfoPanelVisible ? "#F8F9F9" : "transparent"};
  }
`;

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
      {canCreate && (
        <ContextMenuButton
          className="add-button"
          directionX="left"
          iconName="images/plus.svg"
          size={17}
          isFill
          getData={getContextOptionsPlus}
          isDisabled={false}
        />
      )}

      {isRecycleBinFolder && !isEmptyFilesList && (
        <IconButton
          iconName="images/clear.active.react.svg"
          size={17}
          isFill={true}
          onClick={clearTrash}
          className="trash-button"
        />
      )}

      {!isRootFolder && !personal && (
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

      <StyledInfoPanelToggleWrapper
        isRootFolder={isRootFolder}
        isInfoPanelVisible={isInfoPanelVisible}
      >
        <div className="info-panel-toggle-bg">
          <IconButton
            className="info-panel-toggle"
            iconName="images/panel.svg"
            size="17"
            color={isInfoPanelVisible ? "#3B72A7" : "#A3A9AE"}
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
