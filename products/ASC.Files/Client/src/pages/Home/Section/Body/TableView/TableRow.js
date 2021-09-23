import React, { useState } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import ItemIcon from "../../../../../components/ItemIcon";
import SharedButton from "../../../../../components/SharedButton";
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
import styled from "styled-components";
import Base from "@appserver/components/themes/base";

const sideColor = globalColors.gray;
const { acceptBackground, background } = Base.dragAndDrop;

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    background: ${(props) => props.checked && "#f8f9f9 !important"};
    cursor: ${(props) => props.checked && "url(images/cursor.palm.svg), auto"};
  }
`;

const StyledDragAndDrop = styled(DragAndDrop)`
  display: contents;
`;

const StyledShare = styled.div`
  cursor: pointer;

  .share-button {
    padding: 4px;
    border: 1px solid transparent;
    border-radius: 3px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    :hover {
      border: 1px solid #a3a9ae;
      svg {
        cursor: pointer;
      }
    }

    .share-button-icon {
      margin-right: 7px;
    }
  }
`;

const StyledBadgesContainer = styled.div`
  display: flex;
  align-items: center;
  height: 19px;
  margin-left: 8px;

  .badges {
    display: flex;
    align-items: center;
    height: 19px;
    margin-right: 12px;
  }

  .badge {
    cursor: pointer;
    height: 14px;
    width: 14px;
    margin-right: 6px;
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
    showShare,
    personal,
  } = props;

  const sharedButton =
    item.canShare && showShare ? (
      <SharedButton
        t={t}
        id={item.id}
        shared={item.shared}
        isFolder={item.isFolder}
      />
    ) : null;

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
        item={item}
        element={element}
        fileContextClick={fileContextClick}
        onContentSelect={onContentFileSelect}
        onClick={onMouseClick}
        {...contextOptionsProps}
        checked={checkedProps}
      >
        <TableCell {...dragStyles} {...selectionProp}>
          <FileNameCell {...props} />
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
          <StyledShare>{sharedButton}</StyledShare>
        </TableCell>
      </StyledTableRow>
    </StyledDragAndDrop>
  );
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))
  )
);
