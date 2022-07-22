import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledDropdown = styled.div`
  @media (orientation: landscape) {
    ${(props) =>
      props.isPersonal &&
      props.isExternalLink &&
      window.innerHeight < 500 &&
      css`
        top: 10% !important;
        left: 45% !important;
      `}
  }

  font-family: ${(props) => props.theme.fontFamily};
  font-style: normal;
  font-weight: ${(props) => props.theme.dropDown.fontWeight};
  font-size: ${(props) => props.theme.dropDown.fontSize};
  ${(props) =>
    props.maxHeight &&
    `
    max-height: ${props.maxHeight}px;
    overflow-y: auto;
  `}
  height: fit-content;
  position: absolute;
  ${(props) => props.manualWidth && `width: ${props.manualWidth};`}
  ${(props) =>
    props.directionY === "top" &&
    css`
      bottom: ${(props) => (props.manualY ? props.manualY : "100%")};
    `}
  ${(props) =>
    props.directionY === "bottom" &&
    css`
      top: ${(props) => (props.manualY ? props.manualY : "100%")};
    `}
  ${(props) =>
    props.directionX === "right" &&
    css`
      right: ${(props) => (props.manualX ? props.manualX : "0px")};
    `}
  ${(props) =>
    props.directionX === "left" &&
    css`
      left: ${(props) => (props.manualX ? props.manualX : "0px")};
    `}
  z-index: ${(props) =>
    props.zIndex ? props.zIndex : props.theme.dropDown.zIndex};
  display: ${(props) =>
    props.open ? (props.columnCount ? "block" : "table") : "none"};
  background: ${(props) => props.theme.dropDown.background};
  border: ${(props) => props.theme.dropDown.border};
  border-radius: ${(props) => props.theme.dropDown.borderRadius};
  -moz-border-radius: ${(props) => props.theme.dropDown.borderRadius};
  -webkit-border-radius: ${(props) => props.theme.dropDown.borderRadius};
  box-shadow: ${(props) => props.theme.dropDown.boxShadow};
  -moz-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
  -webkit-box-shadow: ${(props) => props.theme.dropDown.boxShadow};
  padding: ${(props) =>
    !props.maxHeight &&
    props.children &&
    props.children.length > 1 &&
    `4px 0px`};
  ${(props) =>
    props.columnCount &&
    `
    -webkit-column-count: ${props.columnCount};
    -moz-column-count: ${props.columnCount};
          column-count: ${props.columnCount};
  `}

  .scroll-drop-down-item {
    .scroll-body {
      padding-right: 0 !important;
    }
  }
`;

StyledDropdown.defaultProps = { theme: Base };
export default StyledDropdown;
