import React, { useState } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
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
const sideColor = globalColors.gray;
const { acceptBackground, background } = Base.dragAndDrop;

const rowCheckboxCheckedStyle = css`
  border-image-source: linear-gradient(to right, #f3f4f4 24px, #eceef1 24px);
`;
const contextMenuWrapperCheckedStyle = css`
  border-image-source: linear-gradient(to left, #f3f4f4 24px, #eceef1 24px);
`;

const rowCheckboxDraggingStyle = css`
  border-image-source: linear-gradient(to right, #f8f7bf 24px, #eceef1 24px);
`;

const contextMenuWrapperDraggingStyle = css`
  border-image-source: linear-gradient(to left, #f8f7bf 24px, #eceef1 24px);
`;

const rowCheckboxDraggingHoverStyle = css`
  border-image-source: linear-gradient(
    to right,
    rgb(239, 239, 178) 24px,
    #eceef1 24px
  );
`;
const contextMenuWrapperDraggingHoverStyle = css`
  border-image-source: linear-gradient(
    to left,
    rgb(239, 239, 178) 24px,
    #eceef1 24px
  );
`;

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    ${isSafari && `border-image-slice: 0 !important`};
    background: ${(props) =>
      (props.checked || props.isActive) && "#F3F4F4 !important"};
    cursor: ${(props) =>
      !props.isThirdPartyFolder &&
      (props.checked || props.isActive) &&
      "url(images/cursor.palm.svg), auto"};
  }

  .table-container_element {
    /* margin-left: ${(props) => (props.isFolder ? "-3px" : "-4px")}; */
  }

  .table-container_row-checkbox {
    padding-left: 4px;
    width: 26px;
  }

  &:hover {
    .table-container_row-checkbox-wrapper {
      ${(props) => props.dragging && rowCheckboxDraggingHoverStyle}
    }
    .table-container_row-context-menu-wrapper {
      ${(props) => props.dragging && contextMenuWrapperDraggingHoverStyle}
    }
  }

  .table-container_row-checkbox-wrapper {
    min-width: 30px;
    margin-left: -24px;
    padding-left: 24px;
    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: linear-gradient(to right, #ffffff 24px, #eceef1 24px);

    border-top: 0;
    border-right: 0;

    ${(props) => props.checked && rowCheckboxCheckedStyle};
    ${(props) => props.dragging && rowCheckboxDraggingStyle};
  }

  .table-container_row-context-menu-wrapper {
    margin-right: -20x;
    width: 28px;
    padding-right: 18px;
    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: linear-gradient(to left, #ffffff 24px, #eceef1 24px);

    border-top: 0;
    border-left: 0;

    ${(props) => props.checked && contextMenuWrapperCheckedStyle};
    ${(props) => props.dragging && contextMenuWrapperDraggingStyle};
  }

  .edit {
    svg:not(:root) {
      width: 12px;
      height: 12px;
    }
  }
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
`;

const FilesTableRow = (props) => {
  const {
    t,
    contextOptionsProps,
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
    quickButtonsComponent,
  } = props;

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

  return (
    <StyledDragAndDrop
      data-title={item.title}
      value={value}
      className={`files-item ${className}`}
      onDrop={onDrop}
      onMouseDown={onMouseDown}
      dragging={dragging && isDragging}
      onDragOver={onDragOver}
      onDragLeave={onDragLeave}
    >
      <StyledTableRow
        {...dragStyles}
        dragging={dragging && isDragging}
        selectionProp={selectionProp}
        key={item.id}
        fileContextClick={fileContextClick}
        onClick={onMouseClick}
        {...contextOptionsProps}
        isActive={isActive}
        isFolder={item.isFolder}
        onHideContextMenu={onHideContextMenu}
        isThirdPartyFolder={item.isThirdPartyFolder}
        onDoubleClick={onFilesClick}
        checked={checkedProps}
        title={
          item.isFolder
            ? t("Translations:TitleShowFolderActions")
            : t("Translations:TitleShowActions")
        }
      >
        <TableCell {...dragStyles} {...selectionProp}>
          <FileNameCell
            onContentSelect={onContentFileSelect}
            checked={checkedProps}
            element={element}
            {...selectionProp}
            {...props}
          />
          <StyledBadgesContainer>{badgesComponent}</StyledBadgesContainer>
        </TableCell>
        {!personal && (
          <TableCell {...dragStyles} {...selectionProp}>
            <AuthorCell sideColor={sideColor} {...props} />
          </TableCell>
        )}
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell create sideColor={sideColor} {...props} />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell sideColor={sideColor} {...props} />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <SizeCell sideColor={sideColor} {...props} />
        </TableCell>

        <TableCell {...dragStyles} {...selectionProp}>
          <TypeCell sideColor={sideColor} {...props} />
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
    withRouter(
      withContextOptions(
        withContent(withQuickButtons(withBadges(FilesTableRow)))
      )
    )
  )
);
