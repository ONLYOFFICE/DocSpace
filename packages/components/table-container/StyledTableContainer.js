import styled, { css } from "styled-components";
import Base from "../themes/base";
import { mobile, tablet, hugeMobile } from "../utils/device";
import IconButton from "../icon-button";
import Scrollbar from "../scrollbar";
import { isMobile, isMobileOnly } from "react-device-detect";
import { ColorTheme } from "@docspace/components/ColorTheme";
import {
  getCorrectBorderRadius,
  getCorrectFourValuesStyle,
} from "../utils/rtlUtils";

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
    margin: ${({ theme }) =>
      getCorrectFourValuesStyle("14px 0px 0 auto", theme.interfaceDirection)};
    z-index: 1;

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `border-left: ${theme.tableContainer.borderRight};`
        : `border-right: ${theme.tableContainer.borderRight};`}

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

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `border-right: 0;`
        : `border-left: 0;`}
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

StyledTableContainer.defaultProps = {
  theme: Base,
};

const StyledTableGroupMenu = styled.div`
  position: relative;

  background: ${(props) => props.theme.tableContainer.groupMenu.background};
  border-bottom: ${(props) =>
    props.theme.tableContainer.groupMenu.borderBottom};
  box-shadow: ${(props) => props.theme.tableContainer.groupMenu.boxShadow};
  border-radius: ${({ theme }) =>
    getCorrectBorderRadius("0px 0px 6px 6px", theme.interfaceDirection)};

  display: flex;
  flex-direction: row;
  align-items: center;

  width: 100%;
  height: 100%;

  z-index: 199;

  margin: 0;

  .table-container_group-menu-checkbox {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? `
        margin-right: 28px;
        ${props.checkboxMargin && `margin-right: ${props.checkboxMargin};`}`
        : `
        margin-left: 28px;
        ${props.checkboxMargin && `margin-left: ${props.checkboxMargin};`}`}

    @media ${tablet} {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-right: 24px;`
          : `margin-left: 24px;`}
    }

    ${isMobile &&
    css`
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-right: 24px;`
          : `margin-left: 24px;`}
    `}
  }

  .table-container_group-menu-separator {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? `border-left: ${props.theme.tableContainer.groupMenu.borderRight};`
        : `border-right: ${props.theme.tableContainer.groupMenu.borderRight};`}
    width: 1px;
    height: 21px;
    margin: ${({ theme }) =>
      getCorrectFourValuesStyle("0 16px 0 20px", theme.interfaceDirection)};

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
    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-left: 8px;`
        : `margin-right: 8px;`}
  }

  .table-container_group-menu-combobox {
    height: 24px;
    width: 16px;
    margin: ${({ theme }) =>
      getCorrectFourValuesStyle("7px 2px 0px 9px", theme.interfaceDirection)};
    background: transparent;

    .combo-button {
      .combo-buttons_arrow-icon {
        margin: ${({ theme }) =>
          getCorrectFourValuesStyle("1px 16px 0 0", theme.interfaceDirection)};
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
  margin: ${({ theme }) =>
    isMobile
      ? getCorrectFourValuesStyle("0 16px 0 auto", theme.interfaceDirection)
      : getCorrectFourValuesStyle("0 20px 0 auto", theme.interfaceDirection)};
  height: 100%;
  width: auto;

  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `
      padding-right: 20px;
      padding-left: 0;`
      : `
      padding-left: 20px;
      padding-right: 0;`}

  @media ${tablet} {
    display: none;
    margin: ${({ theme }) =>
      getCorrectFourValuesStyle("0 16px 0 auto", theme.interfaceDirection)};
  }

  margin-top: 1px;

  .info-panel-toggle-bg {
    margin-bottom: 1px;
  }

  .info-panel-toggle svg {
    ${({ theme }) =>
      theme.interfaceDirection === "rtl" && `transform: scaleX(-1);`}
  }
`;
StyledInfoPanelToggleColorThemeWrapper.defaultProps = {
  theme: Base,
};

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
    ${(props) =>
      props.checkboxMargin &&
      (props.interfaceDirection === "rtl"
        ? `margin-right: ${props.checkboxMargin}`
        : `margin-left: ${props.checkboxMargin}`)}
  }

  .table-container_header-cell {
    overflow: hidden;
  }
`;

StyledTableHeader.defaultProps = {
  theme: Base,
};

const StyledTableHeaderCell = styled.div`
  cursor: ${(props) =>
    props.showIcon && props.sortingVisible ? "pointer" : "default"};

  .header-container-text-icon {
    padding: ${({ theme }) =>
      getCorrectFourValuesStyle("13px 0 0 4px", theme.interfaceDirection)};

    display: ${(props) =>
      props.isActive && props.showIcon ? "block" : "none"};
    ${(props) =>
      props.sorted &&
      css`
        transform: scale(1, -1);
        padding: ${getCorrectFourValuesStyle(
          "14px 0 14px 4px",
          props.theme.interfaceDirection
        )};
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

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-left: 8px;`
        : `margin-right: 8px;`}

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

StyledTableHeaderCell.defaultProps = {
  theme: Base,
};

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

  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `padding-left: 30px;`
      : `padding-right: 30px;`}

  .react-svg-icon svg {
    margin-top: 2px;
  }

  .table-container_element {
    display: ${(props) => (props.checked ? "none" : "flex")};
  }
  .table-container_row-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
    padding: 16px;

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-right: -4px;`
        : `margin-left: -4px;`}
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

StyledTableCell.defaultProps = {
  theme: Base,
};

const StyledTableSettings = styled.div`
  margin: ${({ theme }) =>
    getCorrectFourValuesStyle("14px 0 0px 2px", theme.interfaceDirection)};
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
  .scroller {
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

StyledSettingsIcon.defaultProps = {
  theme: Base,
};

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
