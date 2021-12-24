import React from 'react';
import styled, { css } from 'styled-components';
import { withTranslation } from 'react-i18next';
import DragAndDrop from '@appserver/components/drag-and-drop';
import Row from '@appserver/components/row';
import FilesRowContent from './FilesRowContent';
import { withRouter } from 'react-router-dom';

import withFileActions from '../../../../../HOCs/withFileActions';
import withContextOptions from '../../../../../HOCs/withContextOptions';
import SharedButton from '../../../../../components/SharedButton';
import ItemIcon from '../../../../../components/ItemIcon';
import { Base } from '@appserver/components/themes';

const checkedStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.checkedBackground};
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }
`;

const draggingStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.draggingBackground};
  &:hover {
    background: ${(props) => props.theme.filesSection.rowView.draggingHoverBackground};
  }
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }
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
  position: unset;
  cursor: ${(props) =>
    !props.isThirdPartyFolder &&
    (props.checked || props.isActive) &&
    'url(images/cursor.palm.svg), auto'};
  margin-top: -2px;

  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon {
    margin-right: 7px;
  }

  .share-button:hover,
  .share-button-icon:hover {
    cursor: pointer;
    color: ${(props) => props.theme.filesSection.rowView.shareButton.color};
    path {
      fill: ${(props) => props.theme.filesSection.rowView.shareButton.fill};
    }
  }
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media (max-width: 1312px) {
    .share-button {
      padding-top: 3px;
    }
  }

  .styled-element {
    height: 32px;
    /* width: ${(props) => (props.isEdit ? '52px' : '24px')}; */
    margin-right: 7px;
  }
`;

StyledSimpleFilesRow.defaultProps = { theme: Base };

const SimpleFilesRow = (props) => {
  const {
    t,
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
    displayShareButton,
    isPrivacy,
    contextOptionsProps,
    checkedProps,
    onFilesClick,
    onMouseClick,
    isEdit,
    showShare,
    isActive,
  } = props;

  const sharedButton =
    item.canShare && showShare ? (
      <SharedButton t={t} id={item.id} shared={item.shared} isFolder={item.isFolder} />
    ) : null;

  const element = <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />;

  return (
    <StyledWrapper>
      <DragAndDrop
        data-title={item.title}
        value={value}
        className={`files-item ${className}`}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}>
        <StyledSimpleFilesRow
          key={item.id}
          data={item}
          isEdit={isEdit}
          element={element}
          sectionWidth={sectionWidth}
          contentElement={sharedButton}
          onSelect={onContentFileSelect}
          rowContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          onClick={onMouseClick}
          onDoubleClick={onFilesClick}
          checked={checkedProps}
          {...contextOptionsProps}
          contextButtonSpacerWidth={displayShareButton}
          dragging={dragging && isDragging}
          isActive={isActive}
          isThirdPartyFolder={item.isThirdPartyFolder}>
          <FilesRowContent item={item} sectionWidth={sectionWidth} onFilesClick={onFilesClick} />
        </StyledSimpleFilesRow>
      </DragAndDrop>
    </StyledWrapper>
  );
};

export default withTranslation(['Home', 'Translations'])(
  withFileActions(withRouter(withContextOptions(SimpleFilesRow))),
);
