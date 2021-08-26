import styled, { css } from "styled-components";
import Base from "../themes/base";

const HeaderStyles = css`
  height: 39px;
  position: fixed;
  background: #fff;
  z-index: 1;
  border-bottom: 1px solid #eceef1;
`;

const StyledTableContainer = styled.div`
  width: 100%;
  max-width: 100%;
  margin-top: -18px;

  display: grid;

  .table-column {
    user-select: none;
    position: relative;
    min-width: 10%;
  }

  .resize-handle {
    display: block;
    cursor: ew-resize;
    height: 10px;
    margin: 14px 8px 0 auto;
    z-index: 1;
    border-right: 2px solid #d0d5da;
  }

  .content-container {
    overflow: hidden;
  }

  .children-wrap {
    display: flex;
    flex-direction: column;
  }

  .table-cell {
    height: 47px;
    border-bottom: 1px solid #eceef1;
  }

  .table-container_group-menu {
    .table-container_group-menu-checkbox {
      width: 22px;
    }

    .table-container_group-menu-combobox {
      height: 24px;
      width: 16px;
      margin-bottom: 16px;

      .combo-button {
        height: 24px;
        margin-top: 8px;
        width: 16px;

        .combo-buttons_arrow-icon {
          margin: 8px 16px 0 0;

          /* svg {
            path {
              fill: #333;
            }
          } */
        }
      }
    }

    .table-container_group-menu-separator {
      border-right: 1px solid #eceef1;
      width: 2px;
      height: 10px;
      margin: 0 8px;
    }
  }
`;

const StyledTableGroupMenu = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;
  width: ${(props) => props.width};

  ${HeaderStyles}

  .table-container_group-menu-checkbox {
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}
  }

  .table-container_group-menu_button {
    margin-right: 8px;
  }
`;

const StyledTableHeader = styled.div`
  display: grid;

  ${HeaderStyles}

  .table-container_header-checkbox {
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}
  }

  .table-container_header-cell {
    overflow: hidden;
  }
`;

const StyledTableHeaderCell = styled.div`
  .table-container_header-item {
    display: flex;
    user-select: none;
  }

  .header-container-text-wrapper {
    display: flex;
    cursor: pointer;

    .header-container-text-icon {
      padding: 16px 0 0 4px;

      display: ${(props) => (props.isActive ? "block" : "none")};
      ${(props) =>
        props.sorted &&
        css`
          transform: scale(1, -1);
          padding: 14px 0 0 4px;
        `}
    }

    :hover {
      .header-container-text-icon {
        display: block;
      }
    }
  }

  .header-container-text {
    height: 38px;
    display: flex;
    align-items: center;
  }
`;

const StyledTableBody = styled.div`
  display: contents;
`;

const StyledTableRow = styled.div`
  display: contents;

  .table-container_header-checkbox {
    svg {
      margin: 0;
    }
  }

  .droppable-hover {
    background: ${(props) =>
      props.dragging
        ? `${props.theme.dragAndDrop.acceptBackground} !important`
        : "none"};
  }
`;

const StyledTableCell = styled.div`
  /* padding-right: 8px; */
  height: 40px;
  max-height: 40px;
  border-bottom: 1px solid #eceef1;
  overflow: hidden;

  display: flex;
  align-items: center;

  .react-svg-icon svg {
    margin-top: 2px;
  }

  .table-container_element {
    display: ${(props) => (props.checked ? "none" : "flex")};
  }
  .table-container_row-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
  }

  ${(props) =>
    props.hasAccess &&
    css`
      :hover {
        .table-container_element {
          display: none;
        }
        .table-container_row-checkbox {
          display: flex;
        }
      }
    `}
`;

const StyledTableSettings = styled.div`
  margin: 14px 0 0px 8px;
  display: inline-block;
  position: relative;
  cursor: pointer;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .table-container_settings-checkbox {
    padding: 8px 16px;
  }
`;

const StyledEmptyTableContainer = styled.div`
  grid-column-start: 1;
  grid-column-end: -1;
  height: 40px;
`;

StyledTableRow.defaultProps = { theme: Base };

export {
  StyledTableContainer,
  StyledTableRow,
  StyledTableBody,
  StyledTableHeader,
  StyledTableHeaderCell,
  StyledTableCell,
  StyledTableSettings,
  StyledTableGroupMenu,
  StyledEmptyTableContainer,
};
