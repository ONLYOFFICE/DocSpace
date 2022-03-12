import React, { useState } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import withFileActions from "../../../../../HOCs/withFileActions";
import ItemIcon from "../../../../../components/ItemIcon";
import { withTranslation } from "react-i18next";
import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";
import DragAndDrop from "@appserver/components/drag-and-drop";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";
import AuthorCell from "./sub-components/AuthorCell";
import DateCell from "./sub-components/DateCell";
import TypeCell from "./sub-components/TypeCell";
import globalColors from "@appserver/components/utils/globalColors";
import styled, { css } from "styled-components";
import Base from "@appserver/components/themes/base";
import { isSafari } from "react-device-detect";

const hotkeyBorderStyle = css`
  border-image-source: linear-gradient(to left, #2da7db 24px, #2da7db 24px);
`;

const rowCheckboxDraggingStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.checkboxDragging};
`;

const contextMenuWrapperDraggingStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.contextMenuWrapperDragging};
`;

const rowCheckboxDraggingHoverStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.checkboxDraggingHover};
`;
const contextMenuWrapperDraggingHoverStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.contextMenuWrapperDraggingHover};
`;

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    /* ${isSafari && `border-image-slice: 0 !important`}; */
    background: ${(props) =>
      (props.checked || props.isActive) &&
      `${props.theme.filesSection.tableView.row.backgroundActive} !important`};
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

    ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"}
  }

  .table-container_element-wrapper,
  .table-container_row-loader {
    min-width: 36px;
  }

  .table-container_row-loader {
    svg {
      margin-left: 4px;
    }
  }

  .table-container_element {
    /* margin-left: ${(props) => (props.isFolder ? "-3px" : "-4px")}; */
  }

  .table-container_row-checkbox {
    padding-left: 16px;
    width: 16px;
  }

  &:hover {
    .table-container_file-name-cell {
      ${(props) => props.dragging && rowCheckboxDraggingHoverStyle}
    }
    .table-container_row-context-menu-wrapper {
      ${(props) => props.dragging && contextMenuWrapperDraggingHoverStyle}
    }
  }

  .table-container_file-name-cell {
    min-width: 30px;
    margin-left: -24px;
    padding-left: 24px;
    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: ${(props) =>
      props.theme.filesSection.tableView.row.borderImageCheckbox};

    ${(props) =>
      !props.isActive &&
      !props.checked &&
      css`
        border-image-slice: 1;
        border-bottom: 1px solid;
        border-image-source: ${(props) =>
          props.theme.filesSection.tableView.row.borderImageRight};
      `};

    border-top: 0;
    border-right: 0;
    border-left: 0;

    ${(props) => props.showHotkeyBorder && hotkeyBorderStyle};
    ${(props) => props.dragging && rowCheckboxDraggingStyle};
  }

  .table-container_row-context-menu-wrapper {
    margin-right: -20x;
    width: 28px;
    padding-right: 18px;
    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: ${(props) =>
      props.theme.filesSection.tableView.row.borderImageContextMenu};

    ${(props) =>
      !props.isActive &&
      !props.checked &&
      css`
        border-bottom: 1px solid;
        border-image-slice: 1;
        border-image-source: ${(props) =>
          props.theme.filesSection.tableView.row.borderImageLeft};
      `};

    border-top: 0;
    border-left: 0;

    ${(props) => props.dragging && contextMenuWrapperDraggingStyle};
    ${(props) => props.showHotkeyBorder && hotkeyBorderStyle};
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
        margin-top: -1px;
        border-top: 1px solid #2da7db;
      }
    `}
`;

const StyledDragAndDrop = styled(DragAndDrop)`
  display: contents;
`;

const StyledBadgesContainer = styled.div`
  margin-left: 8px;

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
    margin: 1px -2px -2px -2px;
  }

  .badge-version {
    width: max-content;
    margin: -2px 6px -2px -2px;
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
  } = props;
  const { acceptBackground, background } = theme.dragAndDrop;

  const element = (
    <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />
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

  return (
    <StyledDragAndDrop
      data-title={item.title}
      value={value}
      className={`files-item ${className} ${item.id}_${item.fileExst} ${
        showHotkeyBorder
          ? "table-hotkey-border"
          : checkedProps || isActive
          ? "table-row-selected"
          : ""
      }`}
      onDrop={onDrop}
      onMouseDown={onMouseDown}
      dragging={dragging && isDragging}
      onDragOver={onDragOver}
      onDragLeave={onDragLeave}
    >
      <StyledTableRow
        className="table-row"
        {...dragStyles}
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
      >
        <TableCell
          {...dragStyles}
          className={`${selectionProp?.className} table-container_file-name-cell`}
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
          <StyledBadgesContainer>{badgesComponent}</StyledBadgesContainer>
        </TableCell>
        {!personal && (
          <TableCell {...dragStyles} {...selectionProp}>
            <AuthorCell
              sideColor={theme.filesSection.tableView.row.sideColor}
              {...props}
            />
          </TableCell>
        )}
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell
            create
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <SizeCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>

        <TableCell {...dragStyles} {...selectionProp}>
          <TypeCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>

        <TableCell {...dragStyles} {...selectionProp}>
          <StyledQuickButtonsContainer>
            {quickButtonsComponent}
          </StyledQuickButtonsContainer>
        </TableCell>
      </StyledTableRow>
    </StyledDragAndDrop>
  );
};

export default withTranslation(["Home", "Common", "VersionBadge"])(
  withFileActions(
    withRouter(withContent(withQuickButtons(withBadges(FilesTableRow))))
  )
);
