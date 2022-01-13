import styled from "styled-components";
import Base from "../themes/base";

const StyledContextMenu = styled.div`
  .p-contextmenu {
    position: absolute;
    background: ${(props) => props.theme.dropDown.background};
    border-radius: ${(props) => props.theme.dropDown.borderRadius};
    -moz-border-radius: ${(props) => props.theme.dropDown.borderRadius};
    -webkit-border-radius: ${(props) => props.theme.dropDown.borderRadius};
    box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    -moz-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    -webkit-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    padding: 4px 0px;
  }

  .p-contextmenu ul {
    margin: 0;
    padding: 0;
    list-style: none;
  }

  .p-contextmenu .p-submenu-list {
    position: absolute;
    background: ${(props) => props.theme.dropDown.background};
    border-radius: ${(props) => props.theme.dropDown.borderRadius};
    -moz-border-radius: ${(props) => props.theme.dropDown.borderRadius};
    -webkit-border-radius: ${(props) => props.theme.dropDown.borderRadius};
    box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    -moz-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    -webkit-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
    padding: 4px 0px;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    margin-left: 4px;
    margin-top: -4px;
  }

  .p-contextmenu .p-menuitem-link {
    cursor: pointer;
    display: flex;
    align-items: center;
    text-decoration: none;
    overflow: hidden;
    position: relative;
    border: ${(props) => props.theme.dropDownItem.border};
    margin: ${(props) => props.theme.dropDownItem.margin};
    padding: ${(props) => props.theme.dropDownItem.padding};
    font-family: ${(props) => props.theme.fontFamily};
    font-style: normal;
    background: none;
    user-select: none;
    outline: 0 !important;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    font-weight: ${(props) => props.theme.dropDownItem.fontWeight};
    font-size: ${(props) => props.theme.dropDownItem.fontSize};
    color: ${(props) => props.theme.dropDownItem.color};
    text-transform: none;

    &:hover {
      background-color: ${(props) =>
        props.noHover
          ? props.theme.dropDownItem.backgroundColor
          : props.theme.dropDownItem.hoverBackgroundColor};
    }

    &.p-disabled {
      color: ${(props) => props.theme.dropDownItem.disableColor};

      &:hover {
        cursor: default;
        background-color: ${(props) =>
          props.theme.dropDownItem.hoverDisabledBackgroundColor};
      }
    }
  }

  .p-contextmenu .p-menuitem-text {
    line-height: ${(props) => props.theme.dropDownItem.lineHeight};
  }

  .p-contextmenu .p-menu-separator {
    cursor: default;
    padding: 0px 16px;
    margin: 4px 16px 4px;
    border-bottom: 1px solid #eceef1;
    width: calc(90%-32px);

    &:hover {
      cursor: default;
    }
  }

  .p-contextmenu .p-menuitem {
    position: relative;
    margin: ${(props) => props.theme.dropDownItem.margin};
  }

  .p-menuitem-icon {
    max-height: ${(props) => props.theme.dropDownItem.lineHeight};

    width: 16px;
    height: 16px;

    & svg {
      height: 16px;
      width: 16px;
    }
    path {
      fill: ${(props) => props.theme.dropDownItem.icon.color};
    }

    &.p-disabled {
      path {
        fill: ${(props) => props.theme.dropDownItem.icon.disableColor};
      }
    }

    margin-right: 8px;
  }

  .p-submenu-icon {
    margin-left: auto;
  }

  .p-contextmenu-enter {
    opacity: 0;
  }

  .p-contextmenu-enter-active {
    opacity: 1;
    transition: opacity 250ms;
  }
`;

StyledContextMenu.defaultProps = {
  theme: Base,
};

export default StyledContextMenu;
