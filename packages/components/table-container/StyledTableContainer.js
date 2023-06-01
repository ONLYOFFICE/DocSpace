import styled, { css } from "styled-components";
import Base from "../themes/base";
import { mobile, tablet, hugeMobile } from "../utils/device";
import IconButton from "../icon-button";
import Scrollbar from "../scrollbar";
import { isMobile, isMobileOnly } from "react-device-detect";
import { ColorTheme } from "@docspace/components/ColorTheme";

const reactWindowContainerStyles = css`
  height: 100%;
  display: block;
`;

const reactWindowBodyStyles = css`
  display: block;
  height: 100%;
`;

const StyledTableContainer = styled.div`
  user-select: none;

  width: 100%;
  max-width: 100%;
  margin-top: -25px;

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
    margin: 14px 0px 0 auto;
    z-index: 1;
    border-right: ${(props) => props.theme.tableContainer.borderRight};
    &:hover {
      border-color: ${(props) => props.theme.tableContainer.hoverBorderColor};
    }
  }

  .table-container_group-menu,
  .table-container_header {
    z-index: 200;
    padding: 0 20px;

    border-bottom: 1px solid;
    border-image-slice: 1;
    border-image-source: ${(props) =>
      props.theme.tableContainer.header.borderImageSource};
    border-top: 0;
    border-left: 0;
  }

  .lengthen-header {
    border-image-slice: 1;
    border-image-source: ${(props) =>
      props.theme.tableContainer.header.lengthenBorderImageSource};
  }

  .hotkeys-lengthen-header {
    border-bottom: ${(props) =>
      props.theme.tableContainer.header.hotkeyBorderBottom};
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
    border-bottom: ${(props) => props.theme.tableContainer.tableCellBorder};
  }

  .table-container_group-menu {
    .table-container_group-menu-checkbox {
      width: 22px;
    }
  }

  ${({ useReactWindow }) => useReactWindow && reactWindowContainerStyles}
`;

StyledTableContainer.defaultProps = { theme: Base };

const StyledTableGroupMenu = styled.div`
  position: relative;

  background: ${(props) => props.theme.tableContainer.groupMenu.background};
  border-bottom: ${(props) =>
    props.theme.tableContainer.groupMenu.borderBottom};
  box-shadow: ${(props) => props.theme.tableContainer.groupMenu.boxShadow};
  border-radius: 0px 0px 6px 6px;

  display: flex;
  flex-direction: row;
  align-items: center;

  width: 100%;
  height: 100%;

  z-index: 199;

  margin: 0;

  .table-container_group-menu-checkbox {
    margin-left: 28px;
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}

    @media ${tablet} {
      margin-left: 24px;
    }

    ${isMobile &&
    css`
      margin-left: 24px;
    `}
  }

  .table-container_group-menu-separator {
    border-right: ${(props) =>
      props.theme.tableContainer.groupMenu.borderRight};
    width: 1px;
    height: 21px;
    margin: 0 16px 0 20px;

    @media ${tablet} {
      height: 36px;
    }

    ${isMobile &&
    css`
      height: 36px;
    `}

    @media ${hugeMobile} {
      height: 20px;
    }

    ${isMobileOnly &&
    css`
      height: 20px;
    `}
  }

  .table-container_group-menu_button {
    margin-right: 8px;
  }

  .table-container_group-menu-combobox {
    height: 24px;
    width: 16px;
    margin: 7px 2px 0px 9px;
    background: transparent;

    .combo-button {
      .combo-buttons_arrow-icon {
        margin: 1px 16px 0 0;
      }
    }
  }
`;

StyledTableGroupMenu.defaultProps = { theme: Base };

const StyledInfoPanelToggleColorThemeWrapper = styled(ColorTheme)`
  display: flex;

  align-items: center;
  align-self: center;
  justify-content: center;
  margin: ${isMobile ? "0 16px 0 auto" : "0 20px 0 auto"};
  height: 100%;
  width: auto;
  padding-left: 20px;
  padding-right: 0;

  @media ${tablet} {
    display: none;
    margin: 0 16px 0 auto;
  }

  margin-top: 1px;

  .info-panel-toggle-bg {
    margin-bottom: 1px;
  }
`;
StyledInfoPanelToggleColorThemeWrapper.defaultProps = { theme: Base };

const StyledTableHeader = styled.div`
  position: fixed;
  background: ${(props) => props.theme.tableContainer.header.background};
  display: grid;
  z-index: 1;
  height: 39px;
  border-bottom: ${(props) => props.theme.tableContainer.header.borderBottom};
  margin: 0 -20px;
  padding: 0 20px;

  .table-container_header-checkbox {
    ${(props) => props.checkboxMargin && `margin-left: ${props.checkboxMargin}`}
  }

  .table-container_header-cell {
    overflow: hidden;
  }
`;

StyledTableHeader.defaultProps = { theme: Base };

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

    svg {
      width: 12px;
      height: 12px;
      path {
        fill: ${(props) =>
          props.isActive
            ? props.theme.tableContainer.header.activeIconColor
            : props.theme.tableContainer.header.iconColor} !important;
      }
    }

    &:hover {
      path {
        fill: ${(props) =>
          props.theme.tableContainer.header.hoverIconColor} !important;
      }
    }
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
    display: grid;
    grid-template-columns: 1fr 22px;

    margin-right: 8px;

    user-select: none;
  }

  .header-container-text-wrapper {
    display: flex;
  }

  .header-container-text {
    height: 38px;
    display: flex;
    align-items: center;
    color: ${(props) =>
      props.isActive
        ? props.theme.tableContainer.header.activeTextColor
        : props.theme.tableContainer.header.textColor};

    &:hover {
      color: ${(props) =>
        props.theme.tableContainer.header.hoverTextColor} !important;
    }
  }
`;

StyledTableHeaderCell.defaultProps = { theme: Base };

const StyledTableBody = styled.div`
  display: contents;

  ${({ useReactWindow }) => useReactWindow && reactWindowBodyStyles}
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
  height: 48px;
  max-height: 48px;
  border-bottom: ${(props) => props.theme.tableContainer.tableCell.border};
  overflow: hidden;

  display: flex;
  align-items: center;

  padding-right: 30px;

  .react-svg-icon svg {
    margin-top: 2px;
  }

  .table-container_element {
    display: ${(props) => (props.checked ? "none" : "flex")};
  }
  .table-container_row-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
    padding: 16px;
    margin-left: -4px;
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

StyledTableCell.defaultProps = { theme: Base };

const StyledTableSettings = styled.div`
  margin: 14px 0 0px 2px;
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
  .nav-thumb-vertical {
    display: none !important;
  }
  .nav-thumb-horizontal {
    ${isMobile && "display: none !important"};
  }
`;

StyledTableRow.defaultProps = { theme: Base };

const StyledSettingsIcon = styled(IconButton)`
  ${(props) =>
    props.isDisabled &&
    css`
      svg {
        path {
          fill: ${props.theme.tableContainer.header
            .settingsIconDisableColor} !important;
        }
      }
    `}
`;

StyledSettingsIcon.defaultProps = { theme: Base };

export {
  StyledTableContainer,
  StyledTableRow,
  StyledTableBody,
  StyledTableHeader,
  StyledTableHeaderCell,
  StyledTableCell,
  StyledTableSettings,
  StyledTableGroupMenu,
  StyledInfoPanelToggleColorThemeWrapper,
  StyledEmptyTableContainer,
  StyledScrollbar,
  StyledSettingsIcon,
};
