import React, { useEffect } from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@docspace/components/drag-and-drop";
import Row from "@docspace/components/row";
import FilesRowContent from "./FilesRowContent";
import { isTablet, isMobile } from "react-device-detect";

import withFileActions from "../../../../../HOCs/withFileActions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";
import marginStyles from "./CommonStyles";
import { Base } from "@docspace/components/themes";
import { tablet } from "@docspace/components/utils/device";
import CursorPalmReactSvgUrl from "PUBLIC_DIR/images/cursor.palm.react.svg?url";
import { classNames } from "@docspace/components/utils/classNames";
const checkedStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.checkedBackground};
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
  height: 56px;

  ${(props) =>
    !isMobile &&
    !props.isDragging &&
    css`
      :hover {
        cursor: pointer;
        ${checkedStyle}

        ${(props) =>
          !props.showHotkeyBorder &&
          css`
            margin-top: -2px;
            padding-top: 1px;
            padding-bottom: 1px;
            border-top: ${(props) =>
              `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
            border-bottom: ${(props) =>
              `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
          `}
      }
    `};
  position: unset;
  cursor: ${(props) =>
    !props.isThirdPartyFolder &&
    (props.checked || props.isActive) &&
    `url(${CursorPalmReactSvgUrl}), auto`};
  ${(props) =>
    props.inProgress &&
    css`
      pointer-events: none;
      /* cursor: wait; */
    `}

  margin-top: 0px;

  ${(props) =>
    props.showHotkeyBorder &&
    css`
      border-top: 1px solid #2da7db !important;
      margin-top: -1px;
      margin-left: -24px;
      margin-right: -24px;
      padding-left: 24px;
      padding-right: 24px;
    `}

  ${(props) =>
    props.isHighlight &&
    css`
      ${marginStyles}

      margin-top: -2px;
      padding-top: 1px;
      padding-bottom: 1px;
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      border-bottom: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};

      animation: Highlight 2s 1;

      @keyframes Highlight {
        0% {
          background: ${(props) => props.theme.filesSection.animationColor};
        }

        100% {
          background: none;
        }
      }
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

  .row_content {
    ${(props) =>
      props.sectionWidth > 500 && `max-width: fit-content;`}//min-width: auto
  }

  .badges {
    display: flex;
    align-items: center;
    margin-bottom: 26px;
  }

  .badge {
    margin-right: 8px;
  }

  .badge:last-child {
    margin-right: 0px;
  }

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

  .row_context-menu-wrapper {
    width: min-content;
    justify-content: space-between;
    flex: 0 1 auto;
  }

  .row_content {
    max-width: none;
    min-width: 0;
  }

  .badges {
    margin-top: ${(props) =>
      props.isSmallContainer ? "1px" : props.isRooms ? "4px" : "2px"};
    margin-bottom: 0px;

    ${(props) =>
      props.isSmallContainer &&
      css`
        .tablet-pinned {
          margin-top: 2px;
        }
      `}
  }

  .temp-badges {
    margin-top: 0px;
  }

  .badge {
    margin-right: ${(props) => (props.isSmallContainer ? "8px" : "24px")};
  }

  .lock-file {
    svg {
      height: 16px;
    }
  }

  .expandButton {
    margin-left: ${(props) => (!props.folderCategory ? "6px" : "0")};
    padding-top: 0px;
  }
  .expandButton > div:first-child {
    ${(props) =>
      props.folderCategory &&
      css`
        padding-left: 0 !important;
      `}
  }
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
    onDoubleClick,
    onMouseClick,
    isEdit,
    isActive,
    inProgress,
    isAdmin,
    getContextModel,
    showHotkeyBorder,
    id,
    isRooms,

    folderCategory,
    isHighlight,
  } = props;

  const [isDragOver, setIsDragOver] = React.useState(false);

  const withAccess = item.security?.Lock;
  const isSmallContainer = sectionWidth <= 500;

  const element = (
    <ItemIcon
      id={item.id}
      icon={item.icon}
      fileExst={item.fileExst}
      isRoom={item.isRoom}
      defaultRoomIcon={item.defaultRoomIcon}
    />
  );

  const onDragOver = (dragOver) => {
    if (dragOver !== isDragOver) {
      setIsDragOver(dragOver);
    }
  };

  const onDragLeave = () => {
    setIsDragOver(false);
  };

  const dragStyles =
    dragging && isDragging
      ? {
          marginLeft: "-16px",
          marginRight: "-16px",
          paddingLeft: "16px",
          paddingRight: "16px",
        }
      : {};

  const idWithFileExst = item.fileExst
    ? `${item.id}_${item.fileExst}`
    : item.id ?? "";

  return (
    <StyledWrapper
      id={id}
      onDragOver={onDragOver}
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
        className={classNames("files-item", className, idWithFileExst)}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
        onDragOver={onDragOver}
        onDragLeave={onDragLeave}
        style={dragStyles}
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
          onContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          onClick={onMouseClick}
          onDoubleClick={onDoubleClick}
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
          isDragOver={isDragOver}
          isSmallContainer={isSmallContainer}
          isRooms={isRooms}
          folderCategory={folderCategory}
          isHighlight={isHighlight}
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
  withFileActions(withQuickButtons(SimpleFilesRow))
);
