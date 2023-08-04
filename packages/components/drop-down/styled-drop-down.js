import styled, { css } from "styled-components";
import Base from "../themes/base";
import { isMobileOnly } from "react-device-detect";

const StyledDropdown = styled.div`
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
  ${(props) =>
    (props.isMobileView || isMobileOnly) &&
    css`
      box-shadow: ${(props) => props.theme.dropDown.boxShadowMobile};
      -moz-box-shadow: ${(props) => props.theme.dropDown.boxShadowMobile};
      -webkit-box-shadow: ${(props) => props.theme.dropDown.boxShadowMobile};
    `}

  padding: ${(props) => !props.maxHeight && props.itemCount > 1 && `4px 0px`};
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
  &.download-dialog-dropDown {
    margin-top: 4px;
  }

  @media (orientation: portrait) {
    ${(props) =>
      props.isMobileView &&
      css`
        top: auto !important;
        bottom: 0;
        left: 0;
        width: 100%;
      `}
  }
`;

StyledDropdown.defaultProps = { theme: Base };
export default StyledDropdown;
