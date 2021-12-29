import React, { useState } from 'react';
import { withRouter } from 'react-router';
import withContent from '../../../../../HOCs/withContent';
import withBadges from '../../../../../HOCs/withBadges';
import withFileActions from '../../../../../HOCs/withFileActions';
import withContextOptions from '../../../../../HOCs/withContextOptions';
import ItemIcon from '../../../../../components/ItemIcon';
import SharedButton from '../../../../../components/SharedButton';
import { withTranslation } from 'react-i18next';
import TableRow from '@appserver/components/table-container/TableRow';
import TableCell from '@appserver/components/table-container/TableCell';
import DragAndDrop from '@appserver/components/drag-and-drop';
import FileNameCell from './sub-components/FileNameCell';
import SizeCell from './sub-components/SizeCell';
import AuthorCell from './sub-components/AuthorCell';
import DateCell from './sub-components/DateCell';
import TypeCell from './sub-components/TypeCell';
import globalColors from '@appserver/components/utils/globalColors';
import styled, { css } from 'styled-components';
import Base from '@appserver/components/themes/base';
import { isSafari } from 'react-device-detect';

const rowCheckboxCheckedStyle = css`
  border-image-source: ${(props) => props.theme.filesSection.tableView.row.checkboxChecked};
`;
const contextMenuWrapperCheckedStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.contextMenuWrapperChecked};
`;

const rowCheckboxDraggingStyle = css`
  border-image-source: ${(props) => props.theme.filesSection.tableView.row.checkboxDragging};
`;

const contextMenuWrapperDraggingStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.contextMenuWrapperDragging};
`;

const rowCheckboxDraggingHoverStyle = css`
  border-image-source: ${(props) => props.theme.filesSection.tableView.row.checkboxDraggingHover};
`;
const contextMenuWrapperDraggingHoverStyle = css`
  border-image-source: ${(props) =>
    props.theme.filesSection.tableView.row.contextMenuWrapperDraggingHover};
`;

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    ${isSafari && `border-image-slice: 0 !important`};
    background: ${(props) =>
      (props.checked || props.isActive) &&
      `${props.theme.filesSection.tableView.row.backgroundActive} !important`};
    cursor: ${(props) =>
      !props.isThirdPartyFolder &&
      (props.checked || props.isActive) &&
      'url(images/cursor.palm.svg), auto'};
  }

  .table-container_element {
    /* margin-left: ${(props) => (props.isFolder ? '-3px' : '-4px')}; */
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
    border-image-source: ${(props) => props.theme.filesSection.tableView.row.borderImageCheckbox};

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
    border-image-source: ${(props) =>
      props.theme.filesSection.tableView.row.borderImageContextMenu};

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

const StyledShare = styled.div`
  cursor: pointer;
  margin: 0 auto;

  .share-button {
    padding: 4px;
    border: 1px solid transparent;
    border-radius: 3px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    :hover {
      border: ${(props) => props.theme.filesSection.tableView.row.borderHover};
      svg {
        cursor: pointer;
      }
    }

    .share-button-icon {
      margin-right: 4px;
      padding-top: 3px;
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
    isActive,
    onHideContextMenu,
    onFilesClick,
    theme,
  } = props;
  const { acceptBackground, background } = theme.dragAndDrop;

  const sharedButton =
    item.canShare && showShare ? (
      <SharedButton
        t={t}
        id={item.id}
        shared={item.shared}
        isFolder={item.isFolder}
        isSmallIcon={true}
      />
    ) : null;

  const element = <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />;

  const selectionProp = {
    className: `files-item ${className} ${value}`,
    value,
  };

  const [isDragActive, setIsDragActive] = useState(false);

  const dragStyles = {
    style: {
      background: dragging && isDragging ? (isDragActive ? acceptBackground : background) : 'none',
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
      onDragLeave={onDragLeave}>
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
        checked={checkedProps}>
        <TableCell {...dragStyles} {...selectionProp}>
          <FileNameCell
            theme={theme}
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
            <AuthorCell sideColor={theme.filesSection.tableView.row.sideColor} {...props} />
          </TableCell>
        )}
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell create sideColor={theme.filesSection.tableView.row.sideColor} {...props} />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <DateCell sideColor={theme.filesSection.tableView.row.sideColor} {...props} />
        </TableCell>
        <TableCell {...dragStyles} {...selectionProp}>
          <SizeCell sideColor={theme.filesSection.tableView.row.sideColor} {...props} />
        </TableCell>

        <TableCell {...dragStyles} {...selectionProp}>
          <TypeCell sideColor={theme.filesSection.tableView.row.sideColor} {...props} />
        </TableCell>

        <TableCell {...dragStyles} {...selectionProp}>
          <StyledShare>{sharedButton}</StyledShare>
        </TableCell>
      </StyledTableRow>
    </StyledDragAndDrop>
  );
};

export default withTranslation('Home')(
  withFileActions(withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))),
);
