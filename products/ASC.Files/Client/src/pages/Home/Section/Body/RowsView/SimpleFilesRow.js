import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";
import Row from "@appserver/components/row";
import FilesRowContent from "./FilesRowContent";
import { withRouter } from "react-router-dom";

import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";

const checkedStyle = css`
  background: #f3f4f4;
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
  background: #f8f7bf;
  &:hover {
    background: #efefb2;
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
    "url(images/cursor.palm.svg), auto"};
  margin-top: -2px;

  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon:hover {
    cursor: pointer;
    path {
      fill: #3b72a7;
    }
  }
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .styled-element {
    height: 32px;
    margin-right: 7px;
  }

  .can-convert:hover {
    path {
      fill: #3b72a7;
    }
  }

  .badge {
    width: 12px;
    height: 12px;
    margin-right: 8px;
  }

  .new-items {
    width: 12px;
    min-width: max-content;
    height: 12px;
    border: 0;
    padding: 0;
    margin-top: 3px;
  }

  .badge:last-child {
    margin-right: 0px;
  }

  .badge-version {
    min-width: 21px;
    height: max-content;
    margin-right: 8px;
    border: 0;
    padding: 0;

    p {
      text-align: center;
      height: 12px;
    }
  }

  .lock-file {
    cursor: ${(props) => (props.withAccess ? "pointer" : "default")};
  }

  .badges {
    display: flex;
    align-items: center;
    height: 12px;
    margin-top: 3px;
  }

  .favorite {
    cursor: pointer;
  }

  .expandButton {
    margin-left: 12px;
  }

  ${(props) =>
    props.sectionWidth <= 1024 &&
    props.sectionWidth > 500 &&
    `
    .badges {
      height: 16px;
      margin-top: 2.5px;
     }

    .badge {
      width: 16px;
      height: 16px;
      margin-right: 24px;
    }

    .lock-file {
      svg {
        height: 16px;
      }
    }

    .tablet {
      margin-top: 16px;
    }

    .tablet:last-child {
      margin-right: 60px;
    }

    .new-items {
      width: 16px;
      height: 16px;
      margin-top: 16px;
      margin-right: 0.2vw;  
 
      p {
        text-align:center;
        padding-top: 2px;
      }
    }

    .badge-version {
      min-width: 25px;

      p {
        padding-top: 2px;
        height: 14px;
      }
    }
  `}
  ${(props) =>
    props.sectionWidth > 755 &&
    `
    .new-items {
      margin-right: -0.6vw;
    }
  `}
`;

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
    quickButtonsComponent,
    displayShareButton,
    isPrivacy,
    contextOptionsProps,
    checkedProps,
    onFilesClick,
    onMouseClick,
    isEdit,
    isActive,
    isAdmin,
  } = props;

  console.log("sectionWidth", sectionWidth);
  const withAccess = isAdmin || item.access === 0;

  const element = (
    <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />
  );

  return (
    <StyledWrapper>
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
          contentElement={quickButtonsComponent}
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
          isThirdPartyFolder={item.isThirdPartyFolder}
          withAccess={withAccess}
        >
          <FilesRowContent
            item={item}
            sectionWidth={sectionWidth}
            onFilesClick={onFilesClick}
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
