import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Text } from "../text";

const SimpleLink = ({
  rel,
  isBold,
  fontSize,
  isTextOverflow,
  isHovered,
  isSemitransparent,
  type,
  color,
  title,
  ...props
}) => <a {...props} />;


const colorCss = css`
    color: ${props => props.color};
`;

const hoveredCss = css`
  ${colorCss};
  text-decoration: ${props => (props.type === "page" ? "underline" : "underline dashed")};
`;

const StyledLink = styled(SimpleLink)`
  text-decoration: none;
  user-select: none;
  cursor: pointer;
  opacity: ${props => props.isSemitransparent && "0.5"};

  ${colorCss};

  &:hover {
    cursor: pointer;
    ${hoveredCss};
  }

  ${props => props.isHovered && hoveredCss}
`;

const Link = props => {
  const {
    isBold,
    title,
    fontSize,
    color,
    href,
    onClick,
    isTextOverflow
  } = props;

  console.log("Link render", props);

  return (
    <StyledLink {...props}>
      <Text.Body
        as="span"
        color={color}
        fontSize={fontSize}
        isBold={isBold}
        title={title}
        truncate={isTextOverflow}
      >
        {props.children}
      </Text.Body>
    </StyledLink>
  );
};

Link.propTypes = {
  color: PropTypes.string,
  fontSize: PropTypes.number,
  href: PropTypes.string,
  isBold: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  isTextOverflow: PropTypes.bool,
  onClick: PropTypes.func,
  target: PropTypes.oneOf(["_blank", "_self", "_parent", "_top"]),
  text: PropTypes.string,
  title: PropTypes.string,
  type: PropTypes.oneOf(["action", "page"]),
  tabindex: PropTypes.number,
  rel: PropTypes.string,
};

Link.defaultProps = {
  color: "#333333",
  fontSize: 13,
  href: undefined,
  isBold: false,
  isHovered: false,
  isSemitransparent: false,
  isTextOverflow: true,
  type: "page",
  tabindex: -1,
  rel: "noopener noreferrer"
};

export default Link;
