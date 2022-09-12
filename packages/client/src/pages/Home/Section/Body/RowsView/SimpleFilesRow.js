import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@docspace/components/drag-and-drop";
import Row from "@docspace/components/row";
import FilesRowContent from "./FilesRowContent";
import { withRouter } from "react-router-dom";
import { isTablet, isMobile } from "react-device-detect";

import withFileActions from "../../../../../HOCs/withFileActions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";
import marginStyles from "./CommonStyles";
import { Base } from "@docspace/components/themes";
import { tablet } from "@docspace/components/utils/device";

const checkedStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.checkedBackground};
  ${marginStyles}
`;

const draggingStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.draggingBackground};
  &:hover {
    background: ${(props) =>
      props.theme.filesSection.rowView.draggingHoverBackground};
  }
  ${marginStyles}
`;

const StyledWrapper = styled.div`
  .files-item {
    border-left: none;
    border-right: none;
    margin-left: 0;
  }
`;

const StyledSimpleFilesRow = styled(Row)`
  ${(props) => (props.checked || props.isActive) && checkedStyle};
  ${(props) => props.dragging && draggingStyle}

  ${(props) =>
    !isMobile &&
    !props.isDragging &&
    css`
      :hover {
        cursor: pointer;
        ${checkedStyle}

        margin-top: -3px;
        border-top: ${(props) =>
          `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      }
    `};

  position: unset;
  cursor: ${(props) =>
    !props.isThirdPartyFolder &&
    (props.checked || props.isActive) &&
    "url(/static/images/cursor.palm.react.svg), auto"};
  ${(props) =>
    props.inProgress &&
    css`
      pointer-events: none;
      /* cursor: wait; */
    `}

  margin-top: -2px;

  ${(props) =>
    props.showHotkeyBorder &&
    css`
      border-top: 1px solid #2da7db !important;
      margin-top: -3px;
      margin-left: -24px;
      margin-right: -24px;
      padding-left: 24px;
      padding-right: 24px;
    `}

  ::after {
    ${(props) =>
      props.showHotkeyBorder &&
      css`
        background: #2da7db;
        padding-left: 24px;
        padding-right: 24px;
        margin-left: -24px;
        margin-right: -24px;

        @media ${tablet} {
          margin-left: -16px;
          margin-right: -16px;
          padding-left: 16px;
          padding-right: 16px;
        }
      `}
  }

  ${(props) =>
    (!props.contextOptions || props.isEdit) &&
    `
    & > div:last-child {
        width: 0px;
        overflow: hidden;
      }
  `}

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .styled-element {
    height: 32px;
    margin-right: 7px;
  }

  .row_context-menu-wrapper {
    width: fit-content;
    justify-content: space-between;
    flex: 1 1 auto;
  }

  .row_content {
    ${(props) =>
      props.sectionWidth > 500 && `max-width: fit-content;`}//min-width: auto
  }

  .badges {
    display: flex;
    align-items: center;
    margin-top: 2px;
    margin-bottom: 26px;
  }

  .badge {
    margin-right: 8px;
  }

  ${(props) =>
    props.sectionWidth > 500 &&
    `
      .badge:last-child {
        margin-right: 0px;
      }
  `}

  .lock-file {
    cursor: ${(props) => (props.withAccess ? "pointer" : "default")};
    svg {
      height: 12px;
    }
  }

  .favorite {
    cursor: pointer;
    margin-top: 1px;
  }

  .expandButton {
    margin-left: 6px;
    padding-top: 7px;
  }

  ${(props) =>
    ((props.sectionWidth <= 1024 && props.sectionWidth > 500) || isTablet) &&
    `
    .row_context-menu-wrapper{
      width: min-content;
      justify-content: space-between;
      flex: 0 1 auto;
    } 

    .row_content {
      max-width: none;
      min-width: 0;
    } 

    .badges {
      margin-bottom: 0px;
    }

    .badge {
      margin-right: 24px;
    }

    .lock-file{
      svg {
        height: 16px;
      }
    }

    .expandButton {
      padding-top: 0px;
    }
  `}
`;

StyledSimpleFilesRow.defaultProps = { theme: Base };

const SimpleFilesRow = (props) => {
  const {
    item,
    sectionWidth,
    dragging,
    onContentFileSelect,
    fileContextClick,
    onDrop,
    onMouseDown,
    className,
    isDragging,
    value,
    quickButtonsComponent,
    displayShareButton,
    isPrivacy,
    checkedProps,
    onFilesClick,
    onMouseClick,
    isEdit,
    isActive,
    inProgress,
    isAdmin,
    getContextModel,
    showHotkeyBorder,
    id,
    isRooms,
  } = props;

  const withAccess = isAdmin || item.access === 0;
  const isSmallContainer = sectionWidth <= 500;

  const element = (
    <ItemIcon
      id={item.id}
      icon={item.isRoom && item.logo.big ? item.logo.big : item.icon}
      fileExst={item.fileExst}
      isRoom={item.isRoom}
    />
  );

  return (
    <StyledWrapper
      id={id}
      className={`row-wrapper ${
        showHotkeyBorder
          ? "row-hotkey-border"
          : checkedProps || isActive
          ? "row-selected"
          : ""
      }`}
    >
      <DragAndDrop
        data-title={item.title}
        value={value}
        className={`files-item ${className} ${item.id}_${item.fileExst}`}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
      >
        <StyledSimpleFilesRow
          key={item.id}
          data={item}
          isEdit={isEdit}
          element={element}
          mode={"modern"}
          sectionWidth={sectionWidth}
          contentElement={
            isSmallContainer || isRooms ? null : quickButtonsComponent
          }
          onSelect={onContentFileSelect}
          rowContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          onClick={onMouseClick}
          onDoubleClick={onFilesClick}
          checked={checkedProps}
          contextOptions={item.contextOptions}
          contextButtonSpacerWidth={displayShareButton}
          dragging={dragging && isDragging}
          isDragging={dragging}
          isActive={isActive}
          inProgress={inProgress}
          isThirdPartyFolder={item.isThirdPartyFolder}
          className="files-row"
          withAccess={withAccess}
          getContextModel={getContextModel}
          showHotkeyBorder={showHotkeyBorder}
          isRoom={item.isRoom}
        >
          <FilesRowContent
            item={item}
            sectionWidth={sectionWidth}
            onFilesClick={onFilesClick}
            quickButtons={
              isSmallContainer || isRooms ? quickButtonsComponent : null
            }
            isRooms={isRooms}
          />
        </StyledSimpleFilesRow>
      </DragAndDrop>
    </StyledWrapper>
  );
};

export default withTranslation(["Files", "Translations", "InfoPanel"])(
  withRouter(withFileActions(withQuickButtons(SimpleFilesRow)))
);
