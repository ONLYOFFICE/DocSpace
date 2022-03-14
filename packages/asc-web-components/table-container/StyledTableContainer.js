import styled, { css } from "styled-components";
import Base from "../themes/base";
import { mobile, tablet } from "../utils/device";
import Scrollbar from "../scrollbar";
import { isMobile } from "react-device-detect";

const StyledTableContainer = styled.div`
  -moz-user-select: none;

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
    &:hover {
      border-color: #657077;
    }
  }

  .table-container_group-menu,
  .table-container_header {
    padding: 0 20px;

    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: linear-gradient(
      to right,
      #ffffff 17px,
      #eceef1 31px,
      #eceef1 calc(100% - 31px),
      #ffffff calc(100% - 17px)
    );
    border-top: 0;
  }
  .lengthen-header {
    border-bottom: 1px solid #eceef1;
    border-image-source: none;
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
  }
`;

const StyledTableGroupMenu = styled.div`
  position: relative;
  background: #fff;
  display: flex;
  flex-direction: row;
  align-items: center;
  width: 100%;
  z-index: 199;
  height: 52px;
  box-shadow: 0px 5px 20px rgba(4, 15, 27, 7%);
  border-radius: 0px 0px 6px 6px;
  margin: 0;
  width: 100%;

  @media ${tablet} {
    height: 60px;
  }

  @media ${mobile} {
    height: 52px;
  }

  .table-container_group-menu-checkbox {
    margin-left: 24px;
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}
  }

  .table-container_group-menu-separator {
    border-right: 1px solid #d0d5da;
    width: 2px;
    height: 20px;
    margin: 0 8px;
  }

  .table-container_group-menu_button {
    margin-right: 8px;
  }

  .table-container_group-menu-combobox {
    height: 24px;
    width: 16px;
    margin: 3px 0px 0px 3px;

    .combo-button {
      .combo-buttons_arrow-icon {
        margin: 1px 16px 0 0;
      }
    }
  }
`;

const StyledTableHeader = styled.div`
  position: fixed;
  background: #fff;
  display: grid;
  z-index: 1;
  height: 39px;
  border-bottom: 1px solid #eceef1;
  margin: 0 -20px;
  padding: 0 20px;

  .table-container_header-checkbox {
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}
  }

  .table-container_header-cell {
    overflow: hidden;
  }
`;

const StyledTableHeaderCell = styled.div`
  cursor: ${(props) =>
    props.showIcon && props.sortingVisible ? "pointer" : "default"};

  .header-container-text-icon {
    padding: 13px 0 0 4px;

    display: ${(props) =>
      props.isActive && props.showIcon ? "block" : "none"};
    ${(props) =>
      props.sorted &&
      css`
        transform: scale(1, -1);
        padding: 14px 0 14px 4px;
      `}
  }

  :hover {
    .header-container-text-icon {
      ${(props) =>
        props.showIcon &&
        css`
          display: block;
        `};
    }
  }

  .table-container_header-item {
    display: flex;
    user-select: none;
  }

  .header-container-text-wrapper {
    display: flex;
  }

  .header-container-text {
    height: 38px;
    display: flex;
    align-items: center;
    &:hover {
      color: #657077;
    }
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

  .table-container_row-loader {
    display: inline-flex;
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
    padding: 12px;
    margin-left: -12px;
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

const StyledScrollbar = styled(Scrollbar)`
  .scroll-body {
    display: flex;
  }
  .nav-thumb-horizontal {
    ${isMobile && "display: none !important"};
  }
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
  StyledScrollbar,
};
