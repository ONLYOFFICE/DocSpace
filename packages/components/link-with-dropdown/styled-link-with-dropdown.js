import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import Text from "../text";
import Base from "../themes/base";
import ExpanderDownReactSvg from "PUBLIC_DIR/images/expander-down.react.svg";
import { transform } from "lodash";
// eslint-disable-next-line no-unused-vars
const SimpleLinkWithDropdown = ({
  isBold,
  fontSize,
  fontWeight,
  isTextOverflow,
  isHovered,
  isSemitransparent,
  color,
  title,
  dropdownType,
  data,
  isDisabled,
  ...props
}) => <a {...props}></a>;

SimpleLinkWithDropdown.propTypes = {
  isBold: PropTypes.bool,
  fontSize: PropTypes.number,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isTextOverflow: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  color: PropTypes.string,
  title: PropTypes.string,
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"])
    .isRequired,
  data: PropTypes.array,
};

const color = (props) =>
  props.isDisabled ? props.theme.linkWithDropdown.disableColor : props.color;

// eslint-disable-next-line react/prop-types, no-unused-vars
const ExpanderDownIconWrapper = ({
  isSemitransparent,
  dropdownType,
  isOpen,
  isDisabled,
  ...props
}) => <ExpanderDownReactSvg {...props} />;

const Caret = styled(ExpanderDownIconWrapper)`
  position: absolute;

  width: ${(props) => props.theme.linkWithDropdown.caret.width};
  min-width: ${(props) => props.theme.linkWithDropdown.caret.minWidth};
  height: ${(props) => props.theme.linkWithDropdown.caret.height};
  min-height: ${(props) => props.theme.linkWithDropdown.caret.minHeight};
  margin-left: ${(props) => props.theme.linkWithDropdown.caret.marginLeft};
  margin-top: ${(props) => props.theme.linkWithDropdown.caret.marginTop};

  right: ${(props) => props.theme.linkWithDropdown.caret.right};
  top: ${(props) => props.theme.linkWithDropdown.caret.top};
  bottom: ${(props) => props.theme.linkWithDropdown.caret.bottom};

  ${(props) =>
    props.theme.interfaceDirection === "rtl" &&
    css`
      margin-right: ${(props) => props.theme.linkWithDropdown.caret.marginLeft};
      margin-left: 0;
      left: ${(props) => props.theme.linkWithDropdown.caret.right};
      right: 0;
    `}

  margin: ${(props) => props.theme.linkWithDropdown.caret.margin};

  path {
    fill: ${color};
  }

  ${(props) =>
    props.dropdownType === "appearDashedAfterHover" &&
    `opacity: ${props.theme.linkWithDropdown.caret.opacity};`}

  ${(props) =>
    props.isOpen &&
    `
      bottom: ${props.theme.linkWithDropdown.caret.isOpenBottom};
      transform: ${props.theme.linkWithDropdown.caret.transform};
    `}
`;
Caret.defaultProps = { theme: Base };

const StyledLinkWithDropdown = styled(SimpleLinkWithDropdown)`
  ${(props) => !props.isDisabled && "cursor: pointer;"}
  text-decoration: none;
  user-select: none;
  position: relative;
  display: flex;
  align-items: center;

  padding-right: ${(props) => props.theme.linkWithDropdown.paddingRight};

  ${(props) =>
    props.theme.interfaceDirection === "rtl" &&
    css`
      padding-left: ${(props) => props.theme.linkWithDropdown.paddingRight};
      padding-right: 0;
    `}

  color: ${color};

  ${(props) => props.isSemitransparent && `opacity: 0.5`};
  ${(props) =>
    props.dropdownType === "alwaysDashed" &&
    `text-decoration:  ${props.theme.linkWithDropdown.textDecoration};`};

  &:not([href]):not([tabindex]) {
    ${(props) =>
      props.dropdownType === "alwaysDashed" &&
      `text-decoration:  ${props.theme.linkWithDropdown.textDecoration};`};
    color: ${color};

    &:hover {
      text-decoration: ${(props) =>
        props.theme.linkWithDropdown.textDecoration};
      color: ${color};
    }
  }

  :hover {
    color: ${color};

    svg {
      ${(props) =>
        props.dropdownType === "appearDashedAfterHover" &&
        `position: absolute; opacity: ${props.theme.linkWithDropdown.svg.opacity};`};
      ${(props) =>
        props.isSemitransparent &&
        `opacity: ${props.theme.linkWithDropdown.svg.semiTransparentOpacity};`};
    }
  }
`;
StyledLinkWithDropdown.defaultProps = { theme: Base };

const StyledTextWithExpander = styled.div`
  display: flex;
  gap: 4px;

  .expander {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 6.35px;
    svg {
      transform: ${(props) => (props.isOpen ? "rotate(180deg)" : "rotate(0)")};
      width: 6.35px;
      height: auto;
    }
  }
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const SimpleText = ({ color, ...props }) => <Text as="span" {...props} />;
const StyledText = styled(SimpleText)`
  color: inherit;
  ${(props) =>
    props.isTextOverflow &&
    css`
      display: inline-block;
      max-width: ${(props) => props.theme.linkWithDropdown.text.maxWidth};
    `};
`;
StyledText.defaultProps = { theme: Base };

const focusColor = css`
  color: ${(props) => props.theme.linkWithDropdown.color.focus};
  background: ${(props) => props.theme.linkWithDropdown.background.focus};
  .expander {
    path {
      fill: ${(props) => props.theme.linkWithDropdown.color.focus};
    }
  }
`;

const StyledSpan = styled.span`
  display: inline-block;
  padding: 4px 8px;
  border-radius: 3px;
  position: relative;

  .drop-down-item {
    display: block;
  }

  .fixed-max-width {
    max-width: ${(props) => props.theme.linkWithDropdown.text.maxWidth};
  }

  color: ${(props) => props.theme.linkWithDropdown.color.default};
  background: ${(props) => props.theme.linkWithDropdown.background.default};
  .expander {
    path {
      fill: ${(props) => props.theme.linkWithDropdown.color.default};
    }
  }

  ${(props) =>
    !props.$isOpen &&
    css`
      :hover {
        color: ${(props) => props.theme.linkWithDropdown.color.hover};

        background: ${(props) => props.theme.linkWithDropdown.background.hover};
        .expander {
          path {
            fill: ${(props) => props.theme.linkWithDropdown.color.hover};
          }
        }
      }
    `}

  ${(props) =>
    props.$isOpen
      ? focusColor
      : css`
          :focus-within,
          :focus {
            ${focusColor}
          }
        `}

  :active {
    color: ${(props) => props.theme.linkWithDropdown.color.active};
    background: ${(props) => props.theme.linkWithDropdown.background.active};
    .expander {
      path {
        fill: ${(props) => props.theme.linkWithDropdown.color.active};
      }
    }
  }
`;
StyledSpan.defaultProps = { theme: Base };

export {
  StyledSpan,
  StyledTextWithExpander,
  StyledText,
  StyledLinkWithDropdown,
  Caret,
};
