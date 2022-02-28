import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";
import Row from "@appserver/components/row";
import FilesRowContent from "./FilesRowContent";
import { withRouter } from "react-router-dom";
import { isTablet } from "react-device-detect";

import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";
import marginStyles from "./CommonStyles";

const checkedStyle = css`
  background: #f3f4f4;
  ${marginStyles}
`;

const draggingStyle = css`
  background: #f8f7bf;
  &:hover {
    background: #efefb2;
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
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
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
    ${(props) => props.sectionWidth > 500 && `max-width: fit-content;`}
    min-width: auto;
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
    margin-left: 12px;
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
    contextOptionsProps,
    checkedProps,
    onFilesClick,
    onMouseClick,
    isEdit,
    isActive,
    inProgress,
    isAdmin,
  } = props;

  const withAccess = isAdmin || item.access === 0;
  const isSmallContainer = sectionWidth <= 500;

  const element = (
    <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />
  );

  return (
    <StyledWrapper
      className={`row-wrapper ${
        checkedProps || isActive ? "row-selected" : ""
      }`}
    >
      <DragAndDrop
        data-title={item.title}
        value={value}
        className={`files-item ${className}`}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
      >
        <StyledSimpleFilesRow
          key={item.id}
          data={item}
          isEdit={isEdit}
          element={element}
          sectionWidth={sectionWidth}
          contentElement={isSmallContainer ? null : quickButtonsComponent}
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
          inProgress={inProgress}
          isThirdPartyFolder={item.isThirdPartyFolder}
          className="files-row"
          withAccess={withAccess}
        >
          <FilesRowContent
            item={item}
            sectionWidth={sectionWidth}
            onFilesClick={onFilesClick}
            quickButtons={isSmallContainer ? quickButtonsComponent : null}
          />
        </StyledSimpleFilesRow>
      </DragAndDrop>
    </StyledWrapper>
  );
};

export default withTranslation(["Home", "Translations"])(
  withFileActions(
    withRouter(withContextOptions(withQuickButtons(SimpleFilesRow)))
  )
);
