import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import ContextMenuButton from "@docspace/components/context-menu-button";
import IconButton from "@docspace/components/icon-button";
import { isMobile } from "react-device-detect";
import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";
import ToggleInfoPanelButton from "./toggle-infopanel-btn";
import PlusButton from "./plus-btn";
import ContextButton from "./context-btn";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";

const StyledContainer = styled.div`
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: 20px;
        `
      : css`
          margin-left: 20px;
        `}
  display: flex;
  align-items: center;

  height: 32px;

  .add-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 16px;
          `
        : css`
            margin-right: 16px;
          `}
    min-width: 15px;

    @media ${tablet} {
      display: ${(props) => (props.isFrame ? "flex" : "none")};
    }

    ${isMobile &&
    css`
      display: ${(props) => (props.isFrame ? "flex" : "none")};
    `}
  }

  .add-drop-down {
    margin-top: 8px;
  }

  .option-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 16px;
          `
        : css`
            margin-right: 16px;
          `}
    min-width: 15px;

    @media ${tablet} {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-left: 9px;
            `
          : css`
              margin-right: 9px;
            `}
    }
  }

  .trash-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 16px;
          `
        : css`
            margin-right: 16px;
          `}
    min-width: 15px;
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;
  align-items: center;
  align-self: center;
  justify-content: center;
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: auto;
        `
      : css`
          margin-left: auto;
        `}

  @media ${tablet} {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: ${(props) => (props.isRootFolder ? "auto" : "0")};
          `
        : css`
            margin-left: ${(props) => (props.isRootFolder ? "auto" : "0")};
          `}
  }

  ${isMobile &&
  css`
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: ${(props) => (props.isRootFolder ? "auto" : "0")};
          `
        : css`
            margin-left: ${(props) => (props.isRootFolder ? "auto" : "0")};
          `}
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

const ControlButtons = ({
  personal,
  isDropBoxComponent,
  isRootFolder,
  canCreate,
  getContextOptionsFolder,
  getContextOptionsPlus,
  isRecycleBinFolder,
  isEmptyFilesList,
  clearTrash,
  isInfoPanelVisible,
  toggleInfoPanel,
  toggleDropBox,
  isDesktop,
  titles,
  withMenu,
  onPlusClick,
  isFrame,
  isPublicRoom,
  isTrashFolder,
}) => {
  const toggleInfoPanelAction = () => {
    toggleInfoPanel && toggleInfoPanel();
    toggleDropBox && toggleDropBox();
  };

  return (
    <StyledContainer isDropBoxComponent={isDropBoxComponent} isFrame={isFrame}>
      {!isRootFolder || (isRecycleBinFolder && !isEmptyFilesList) ? (
        <>
          {!isMobile && canCreate && (
            <PlusButton
              className="add-button"
              getData={getContextOptionsPlus}
              withMenu={withMenu}
              onPlusClick={onPlusClick}
              isFrame={isFrame}
              title={titles?.actions}
            />
          )}

          {/* <ContextMenuButton
            id="header_optional-button"
            zIndex={402}
            className="option-button"
            directionX="right"
            iconName={VerticalDotsReactSvgUrl}
            size={15}
            isFill
            getData={getContextOptionsFolder}
            isDisabled={false}
            title={titles?.contextMenu}
          /> */}

          <ContextButton
            id="header_optional-button"
            className="option-button"
            getData={getContextOptionsFolder}
            withMenu={withMenu}
            //onPlusClick={onPlusClick}
            title={titles?.actions}
            isTrashFolder={isTrashFolder}
          />

          {!isDesktop && (
            <ToggleInfoPanelButton
              isRootFolder={isRootFolder}
              isInfoPanelVisible={isInfoPanelVisible}
              toggleInfoPanel={toggleInfoPanelAction}
            />
          )}
        </>
      ) : canCreate ? (
        <>
          {!isMobile && (
            <PlusButton
              id="header_add-button"
              className="add-button"
              getData={getContextOptionsPlus}
              withMenu={withMenu}
              onPlusClick={onPlusClick}
              isFrame={isFrame}
              title={titles?.actions}
            />
          )}
          {!isDesktop && (
            <ToggleInfoPanelButton
              isRootFolder={isRootFolder}
              isInfoPanelVisible={isInfoPanelVisible}
              toggleInfoPanel={toggleInfoPanelAction}
            />
          )}
        </>
      ) : (
        <>
          {!isDesktop && (
            <ToggleInfoPanelButton
              isRootFolder={isRootFolder}
              isInfoPanelVisible={isInfoPanelVisible}
              toggleInfoPanel={toggleInfoPanelAction}
            />
          )}

          {isPublicRoom && (
            <ContextMenuButton
              id="header_optional-button"
              zIndex={402}
              className="option-button"
              directionX="right"
              iconName={VerticalDotsReactSvgUrl}
              size={15}
              isFill
              getData={getContextOptionsFolder}
              isDisabled={false}
              title={titles?.contextMenu}
            />
          )}
        </>
      )}
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
