import React, { useState } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import withFileActions from "../../../../../HOCs/withFileActions";
import ItemIcon from "../../../../../components/ItemIcon";
import { withTranslation } from "react-i18next";
import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import DragAndDrop from "@docspace/components/drag-and-drop";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";
import AuthorCell from "./sub-components/AuthorCell";
import DateCell from "./sub-components/DateCell";
import TypeCell from "./sub-components/TypeCell";
import TagsCell from "./sub-components/TagsCell";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import { classNames } from "@docspace/components/utils/classNames";

const hotkeyBorderStyle = css`
  border-bottom: 1px solid;
  border-image-slice: 1;
  border-image-source: linear-gradient(to left, #2da7db 24px, #2da7db 24px);
`;

const rowCheckboxDraggingStyle = css`
  margin-left: -20px;
  padding-left: 20px;

  border-bottom: 1px solid;
  border-image-slice: 1;
  border-image-source: ${(props) => `linear-gradient(to right, 
          ${props.theme.filesSection.tableView.row.borderColorTransition} 17px, ${props.theme.filesSection.tableView.row.borderColor} 31px)`};
`;

const contextMenuWrapperDraggingStyle = css`
  margin-right: -20px;
  padding-right: 20px;

  border-bottom: 1px solid;
  border-image-slice: 1;
  border-image-source: ${(props) => `linear-gradient(to left,
          ${props.theme.filesSection.tableView.row.borderColorTransition} 17px, ${props.theme.filesSection.tableView.row.borderColor} 31px)`};
`;

const StyledTableRow = styled(TableRow)`
  ${(props) =>
    props.isRoom &&
    css`
      .table-container_cell {
        height: 48px;
        max-height: 48px;
      }

      .table-container_row-checkbox {
        padding-left: 20px !important;
      }
    `}
  ${(props) =>
    !props.isDragging &&
    css`
      :hover {
        .table-container_cell {
          cursor: pointer;
          background: ${(props) =>
            `${props.theme.filesSection.tableView.row.backgroundActive} !important`};

          margin-top: ${(props) => (props.showHotkeyBorder ? "-2px" : "-1px")};
          ${(props) =>
            !props.showHotkeyBorder &&
            css`
              border-top: ${(props) =>
                `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
            `}
        }
        .table-container_file-name-cell {
          margin-left: -24px;
          padding-left: 24px;
        }
        .table-container_row-context-menu-wrapper {
          margin-right: -20px;
          padding-right: 18px;
        }
      }
    `}
  .table-container_cell {
    background: ${(props) =>
      (props.checked || props.isActive) &&
      `${props.theme.filesSection.tableView.row.backgroundActive} !important`};
    cursor: ${(props) =>
      !props.isThirdPartyFolder &&
      (props.checked || props.isActive) &&
      "url(/static/images/cursor.palm.react.svg), auto !important"};

    ${(props) =>
      props.inProgress &&
      css`
        pointer-events: none;
        /* cursor: wait; */
      `}

    ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"}
  }

  .table-container_element-wrapper,
  .table-container_quick-buttons-wrapper {
    padding-right: 0px;
  }

  .table-container_element-wrapper,
  .table-container_row-loader {
    min-width: ${(props) => (props.isRoom ? "40px" : "36px")};
  }

  .table-container_element-container {
    width: 32px;
    height: 32px;

    display: flex;
    justify-content: center;
    align-items: center;
  }

  .table-container_row-loader {
    svg {
      margin-left: 4px;
    }
  }

  .table-container_row-checkbox {
    padding-left: 20px;
    width: 16px;
  }

  .table-container_file-name-cell {
    ${(props) =>
      props.showHotkeyBorder &&
      css`
        margin-left: -24px;
        padding-left: 24px;
        ${hotkeyBorderStyle}
      `};
    ${(props) => props.dragging && rowCheckboxDraggingStyle};
  }

  .table-container_row-context-menu-wrapper {
    padding-right: 0px;

    ${(props) => props.dragging && contextMenuWrapperDraggingStyle};
    ${(props) =>
      props.showHotkeyBorder &&
      css`
        margin-right: -20px;
        padding-right: 18px;
        ${hotkeyBorderStyle}
      `};
  }

  .edit {
    svg:not(:root) {
      width: 12px;
      height: 12px;
    }
  }

  ${(props) =>
    props.showHotkeyBorder &&
    css`
      .table-container_cell {
        margin-top: -2px;

        border-top: 1px solid #2da7db !important;
        border-right: 0;
        border-left: 0;
      }
      .table-container_file-name-cell > .table-container_cell {
        margin-top: 2px;
        border-top: 0px !important;
      }

      .item-file-name,
      .row_update-text,
      .expandButton,
      .badges,
      .tag,
      .author-cell,
      .table-container_cell > p {
        margin-top: 2px;
      }
    `}
`;

const StyledDragAndDrop = styled(DragAndDrop)`
  display: contents;
`;

const StyledBadgesContainer = styled.div`
  margin-left: 8px;

  display: flex;
  align-items: center;

  ${(props) =>
    props.showHotkeyBorder &&
    css`
      margin-top: 1px;
    `}

  .badges {
    display: flex;
    align-items: center;
    margin-right: 12px;
  }

  .badges:last-child {
    margin-left: 0px;
  }

  .badge {
    cursor: pointer;
    margin-right: 8px;
  }

  .new-items {
    min-width: 12px;
    width: max-content;
    margin: 0 -2px -2px -2px;
  }

  .badge-version {
    width: max-content;
    margin: 0 5px -2px -2px;

    > div {
      padding: 0 3.3px 0 4px;
      p {
        letter-spacing: 0.5px;
        font-size: 8px;
        font-weight: 800;
      }
    }
  }

  .badge-new-version {
    width: max-content;
  }
`;

const StyledQuickButtonsContainer = styled.div`
  width: 100%;

  .badges {
    display: flex;
    justify-content: flex-end;
    align-items: center;
  }

  .badge {
    margin-right: 14px;
  }

  .badge:last-child {
    margin-right: 10px;
  }

  .lock-file {
    svg {
      height: 12px;
    }
  }

  .favorite {
    margin-top: 1px;
  }

  .share-button-icon:hover {
    cursor: pointer;
    path {
      fill: ${(props) =>
        props.theme.filesSection.tableView.row.shareHoverColor};
    }
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
`;

StyledQuickButtonsContainer.defaultProps = { theme: Base };

const FilesTableRow = (props) => {
  const {
    t,
    fileContextClick,
    item,
    onContentFileSelect,
    checkedProps,
    className,
    value,
    onMouseClick,
    badgesComponent,
    dragging,
    isDragging,
    onDrop,
    onMouseDown,
    personal,
    isActive,
    onHideContextMenu,
    onFilesClick,
    inProgress,
    index,
    setFirsElemChecked,
    setHeaderBorder,
    theme,
    quickButtonsComponent,
    getContextModel,
    showHotkeyBorder,
    tableColumns,
    id,
    hideColumns,
    isRooms,
  } = props;
  const { acceptBackground, background } = theme.dragAndDrop;

  const element = (
    <ItemIcon
      id={item.id}
      icon={item.icon}
      fileExst={item.fileExst}
      isRoom={item.isRoom}
    />
  );

  const selectionProp = {
    className: `files-item ${className} ${value}`,
    value,
  };

  const [isDragActive, setIsDragActive] = useState(false);

  const dragStyles = {
    style: {
      background:
        dragging && isDragging
          ? isDragActive
            ? acceptBackground
            : background
          : "none",
    },
  };

  const onDragOver = (dragActive) => {
    if (dragActive !== isDragActive) {
      setIsDragActive(dragActive);
    }
  };

  const onDragLeave = () => {
    setIsDragActive(false);
  };

  React.useEffect(() => {
    if (index === 0) {
      if (checkedProps || isActive) {
        setFirsElemChecked(true);
      } else {
        setFirsElemChecked(false);
      }
      if (showHotkeyBorder) {
        setHeaderBorder(true);
      } else {
        setHeaderBorder(false);
      }
    }
  }, [checkedProps, isActive, showHotkeyBorder]);

  let availableColumns = [];
  let authorAvailableDrag = true;
  let createdAvailableDrag = true;
  let modifiedAvailableDrag = true;
  let sizeAvailableDrag = true;
  let typeAvailableDrag = true;
  let ownerAvailableDrag = true;
  let tagsAvailableDrag = true;
  let activityAvailableDrag = true;
  let buttonsAvailableDrag = true;

  if (dragging && isDragging) {
    availableColumns = localStorage.getItem(tableColumns).split(",");

    authorAvailableDrag = availableColumns.includes("Author") && !hideColumns;
    createdAvailableDrag = availableColumns.includes("Created") && !hideColumns;
    modifiedAvailableDrag =
      availableColumns.includes("Modified") && !hideColumns;
    sizeAvailableDrag = availableColumns.includes("Size") && !hideColumns;
    typeAvailableDrag = availableColumns.includes("Type") && !hideColumns;
    buttonsAvailableDrag = availableColumns.includes("QuickButtons");
    ownerAvailableDrag = availableColumns.includes("Owner") && !hideColumns;
    tagsAvailableDrag = availableColumns.includes("Tags") && !hideColumns;
    activityAvailableDrag =
      availableColumns.includes("Activity") && !hideColumns;
  }

  const idWithFileExst = item.fileExst
    ? `${item.id}_${item.fileExst}`
    : item.id ?? "";

  return (
    <StyledDragAndDrop
      id={id}
      data-title={item.title}
      value={value}
      className={classNames("files-item", className, idWithFileExst, {
        ["table-hotkey-border"]: showHotkeyBorder,
        ["table-row-selected"]: !showHotkeyBorder && (checkedProps || isActive),
      })}
      onDrop={onDrop}
      onMouseDown={onMouseDown}
      dragging={dragging && isDragging}
      onDragOver={onDragOver}
      onDragLeave={onDragLeave}
    >
      <StyledTableRow
        className="table-row"
        {...dragStyles}
        isDragging={dragging}
        dragging={dragging && isDragging}
        selectionProp={selectionProp}
        key={item.id}
        fileContextClick={fileContextClick}
        onClick={onMouseClick}
        isActive={isActive}
        inProgress={inProgress}
        isFolder={item.isFolder}
        onHideContextMenu={onHideContextMenu}
        isThirdPartyFolder={item.isThirdPartyFolder}
        onDoubleClick={onFilesClick}
        checked={checkedProps}
        contextOptions={item.contextOptions}
        getContextModel={getContextModel}
        showHotkeyBorder={showHotkeyBorder}
        title={
          item.isFolder
            ? t("Translations:TitleShowFolderActions")
            : t("Translations:TitleShowActions")
        }
        isRoom={item.isRoom}
      >
        <TableCell
          {...dragStyles}
          className={classNames(
            selectionProp?.className,
            "table-container_file-name-cell"
          )}
          value={value}
        >
          <FileNameCell
            theme={theme}
            onContentSelect={onContentFileSelect}
            checked={checkedProps}
            element={element}
            inProgress={inProgress}
            {...props}
          />
          <StyledBadgesContainer showHotkeyBorder={showHotkeyBorder}>
            {badgesComponent}
          </StyledBadgesContainer>
        </TableCell>

        {(item.isRoom || isRooms) && (
          <TableCell
            style={
              !typeAvailableDrag
                ? { background: "none !important" }
                : dragStyles.style
            }
            {...selectionProp}
          >
            <TypeCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}

        {!item.isRoom && isRooms && (
          <TableCell
            style={
              !typeAvailableDrag
                ? { background: "none !important" }
                : dragStyles.style
            }
            {...selectionProp}
          ></TableCell>
        )}

        {item.isRoom && (
          <TableCell
            style={
              !tagsAvailableDrag
                ? { background: "none !important" }
                : dragStyles.style
            }
            {...selectionProp}
          >
            <TagsCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}

        {!personal && (
          <TableCell
            style={
              !authorAvailableDrag && !ownerAvailableDrag
                ? { background: "none" }
                : dragStyles.style
            }
            {...selectionProp}
          >
            <AuthorCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}

        {!item.isRoom && !isRooms && (
          <TableCell
            style={
              !createdAvailableDrag
                ? { background: "none !important" }
                : dragStyles.style
            }
            {...selectionProp}
          >
            <DateCell
              create
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}

        <TableCell
          style={
            !modifiedAvailableDrag && !activityAvailableDrag
              ? { background: "none" }
              : dragStyles.style
          }
          {...selectionProp}
        >
          <DateCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>

        {!item.isRoom && !isRooms && (
          <TableCell
            style={
              !sizeAvailableDrag ? { background: "none" } : dragStyles.style
            }
            {...selectionProp}
          >
            <SizeCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}

        {!item.isRoom && !isRooms && (
          <TableCell
            style={
              !typeAvailableDrag
                ? { background: "none !important" }
                : dragStyles.style
            }
            {...selectionProp}
          >
            <TypeCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}
        {!item.isRoom && !isRooms && (
          <TableCell
            style={
              !buttonsAvailableDrag ? { background: "none" } : dragStyles.style
            }
            {...selectionProp}
            className={classNames(
              selectionProp?.className,
              "table-container_quick-buttons-wrapper"
            )}
          >
            <StyledQuickButtonsContainer>
              {quickButtonsComponent}
            </StyledQuickButtonsContainer>
          </TableCell>
        )}
      </StyledTableRow>
    </StyledDragAndDrop>
  );
};

export default withTranslation(["Files", "Common", "InfoPanel"])(
  withRouter(
    withFileActions(withContent(withQuickButtons(withBadges(FilesTableRow))))
  )
);
