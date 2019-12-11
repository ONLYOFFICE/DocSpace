import React, { memo } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Text from "../text";

const colorCss = css`
    color: ${props => props.color};
`;

const hoveredCss = css`
  ${colorCss};
  text-decoration: ${props => (props.type === 'page' ? 'underline' : 'underline dashed')};
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const PureText = ({type, color, ...props}) => <Text {...props}/>;
const StyledText = styled(PureText)`
  text-decoration: none;
  user-select: none;
  cursor: pointer;
  opacity: ${props => props.isSemitransparent && "0.5"};

  line-height: calc(100% + 6px);
  ${colorCss};

  &:hover {
    ${hoveredCss};
  }

  ${props => props.isHovered && hoveredCss}

  ${props => props.isTextOverflow && css`
      display: inline-block;
      max-width: 100%;
    `}
`;

// eslint-disable-next-line react/display-name
const Link = memo(({ isTextOverflow, children, ...rest }) => {
  // console.log("Link render", rest);

  return (
    <StyledText
      tag="a"
      isTextOverflow={isTextOverflow}
      truncate={isTextOverflow}
      {...rest}
    >
      {children}
    </StyledText>
  );
});

Link.propTypes = {
  color: PropTypes.string,
  fontSize: PropTypes.string,
  href: PropTypes.string,
  isBold: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  isTextOverflow: PropTypes.bool,
  onClick: PropTypes.func,
  rel: PropTypes.string,
  tabIndex: PropTypes.number,
  target: PropTypes.oneOf(["_blank", "_self", "_parent", "_top"]),
  title: PropTypes.string,
  type: PropTypes.oneOf(["action", "page"]),
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

Link.defaultProps = {
  color: "#333333",
  fontSize: '13px',
  href: undefined,
  isBold: false,
  isHovered: false,
  isSemitransparent: false,
  isTextOverflow: true,
  rel: "noopener noreferrer",
  tabIndex: -1,
  type: "page",
  className: "",
};

export default Link;
