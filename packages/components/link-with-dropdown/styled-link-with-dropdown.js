import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import Text from "../text";
import Base from "../themes/base";
import ExpanderDownIcon from "../../../public/images/expander-down.react.svg";
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
}) => <ExpanderDownIcon {...props} />;

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
      path {
        fill: ${(props) => props.theme.linkWithDropdown.expander.iconColor};
      }
    }
  }
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const SimpleText = ({ color, ...props }) => <Text as="span" {...props} />;
const StyledText = styled(SimpleText)`
  color: ${color};

  ${(props) =>
    props.isTextOverflow &&
    css`
      display: inline-block;
      max-width: ${(props) => props.theme.linkWithDropdown.text.maxWidth};
    `}
`;
StyledText.defaultProps = { theme: Base };

const StyledSpan = styled.span`
  position: relative;

  .drop-down-item {
    display: block;
  }

  .fixed-max-width {
    max-width: ${(props) => props.theme.linkWithDropdown.text.maxWidth};
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
