import styled, { css } from "styled-components";
import Base from "../themes/base";

import Text from "../text/";

import { tablet } from "../utils/device";
import { isMobile } from "react-device-detect";

const styledHeaderText = css`
  font-size: ${(props) => props.theme.menuItem.text.header.fontSize};
  line-height: ${(props) => props.theme.menuItem.text.header.lineHeight};
`;

const styledMobileText = css`
  font-size: ${(props) => props.theme.menuItem.text.mobile.fontSize};
  line-height: ${(props) => props.theme.menuItem.text.mobile.lineHeight};
`;

const StyledText = styled(Text)`
  font-weight: ${(props) => props.theme.menuItem.text.fontWeight};
  font-size: ${(props) => props.theme.menuItem.text.fontSize};
  line-height: ${(props) => props.theme.menuItem.text.lineHeight};
  margin: ${(props) => props.theme.menuItem.text.margin};
  color: ${(props) => props.theme.menuItem.text.color};
  text-align: left;
  text-transform: none;
  text-decoration: none;
  user-select: none;
  ${isMobile
    ? (props) => (props.isHeader ? styledHeaderText : styledMobileText)
    : null}

  @media ${tablet} {
    ${(props) => (props.isHeader ? styledHeaderText : styledMobileText)}
  }
`;
StyledText.defaultProps = { theme: Base };

const StyledMenuItem = styled.div`
  display: ${(props) =>
    props.isHeader && !isMobile
      ? "none"
      : props.textOverflow
      ? "block"
      : "flex"};

  align-items: center;
  width: 100%;
  height: ${(props) =>
    isMobile
      ? props.isHeader
        ? props.theme.menuItem.header.height
        : props.theme.menuItem.mobile.height
      : props.theme.menuItem.height};
  max-height: ${(props) =>
    isMobile
      ? props.isHeader
        ? props.theme.menuItem.header.height
        : props.theme.menuItem.mobile.height
      : props.theme.menuItem.height};
  border: none;
  border-bottom: ${(props) =>
    isMobile && props.isHeader
      ? props.theme.menuItem.header.borderBottom
      : props.theme.menuItem.borderBottom};
  cursor: ${(props) => (isMobile && props.isHeader ? "default" : "pointer")};
  margin: 0;
  margin-bottom: ${(props) =>
    isMobile && props.isHeader
      ? props.theme.menuItem.header.marginBottom
      : props.theme.menuItem.marginBottom};
  padding: ${(props) =>
    isMobile
      ? props.theme.menuItem.mobile.padding
      : props.theme.menuItem.padding};
  box-sizing: border-box;
  background: none;
  outline: 0 !important;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media ${tablet} {
    display: ${(props) => (props.textOverflow ? "block" : "flex")};
    height: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.header.height
        : props.theme.menuItem.mobile.height};
    max-height: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.header.height
        : props.theme.menuItem.mobile.height};
    padding: ${(props) => props.theme.menuItem.mobile.padding};
    border: none;
    border-bottom: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.header.borderBottom
        : props.theme.menuItem.borderBottom};
    margin-bottom: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.header.marginBottom
        : props.theme.menuItem.marginBottom};
    cursor: ${(props) => (props.isHeader ? "default" : "pointer")};
  }

  .drop-down-item_icon {
    path {
      fill: ${(props) => props.theme.menuItem.svgFill};
    }
  }

  &:hover {
    background-color: ${(props) =>
      props.noHover
        ? props.theme.menuItem.background
        : props.theme.menuItem.hover};
  }
  ${(props) =>
    props.isSeparator &&
    css`
      border-bottom: ${props.theme.menuItem.separator.borderBottom};
      cursor: default !important;
      margin: ${props.theme.menuItem.separator.margin};
      height: ${props.theme.menuItem.separator.height};
      width: ${props.theme.menuItem.separator.width};
      &:hover {
        cursor: default !important;
      }
    `}
`;
StyledMenuItem.defaultProps = { theme: Base };

const IconWrapper = styled.div`
  display: flex;
  align-items: center;
  width: ${(props) =>
    props.isHeader && isMobile
      ? props.theme.menuItem.iconWrapper.header.width
      : props.theme.menuItem.iconWrapper.width};
  height: ${(props) =>
    props.isHeader && isMobile
      ? props.theme.menuItem.iconWrapper.header.height
      : props.theme.menuItem.iconWrapper.height};

  @media ${tablet} {
    width: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.iconWrapper.header.width
        : props.theme.menuItem.iconWrapper.width};
    height: ${(props) =>
      props.isHeader
        ? props.theme.menuItem.iconWrapper.header.height
        : props.theme.menuItem.iconWrapper.height};
  }

  svg {
    &:not(:root) {
      width: 100%;
      height: 100%;
    }
  }
`;
IconWrapper.defaultProps = { theme: Base };

export { StyledMenuItem, StyledText, IconWrapper };
