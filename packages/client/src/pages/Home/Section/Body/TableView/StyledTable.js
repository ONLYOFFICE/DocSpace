import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import TableRow from "@docspace/components/table-container/TableRow";
import DragAndDrop from "@docspace/components/drag-and-drop";
import CursorPalmSvgUrl from "PUBLIC_DIR/images/cursor.palm.react.svg?url";

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
      `url(${CursorPalmSvgUrl}), auto !important`};

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

  ${(props) =>
    props.isHighlight &&
    css`
      .table-container_cell:not(.table-container_element-wrapper) {
        animation: Highlight 2s 1;

        @keyframes Highlight {
          0% {
            background: ${(props) => props.theme.filesSection.animationColor};
          }

          100% {
            background: none;
          }
        }
      }

      .table-container_file-name-cell {
        margin-left: -24px;
        padding-left: 24px;
      }
      .table-container_row-context-menu-wrapper {
        margin-right: -20px;
        padding-right: 18px;
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
        font-size: 9px;
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

export {
  StyledBadgesContainer,
  StyledQuickButtonsContainer,
  StyledTableRow,
  StyledDragAndDrop,
};
