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
    ${props => !props.noHover && hoveredCss};
  }

  ${props => !props.noHover && props.isHovered && hoveredCss}

  ${props => props.isTextOverflow && css`
      display: inline-block;
      max-width: 100%;
    `}
`;

// eslint-disable-next-line react/display-name
const Link = memo(({ isTextOverflow, children, noHover,  ...rest }) => {
  // console.log("Link render", rest);

  return (
    <StyledText
      tag="a"
      isTextOverflow={isTextOverflow}
      noHover={noHover}
      truncate={isTextOverflow}
      {...rest}
    >
      {children}
    </StyledText>
  );
});

Link.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  href: PropTypes.string,
  id: PropTypes.string,
  isBold: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  isTextOverflow: PropTypes.bool,
  noHover: PropTypes.bool,
  onClick: PropTypes.func,
  rel: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  tabIndex: PropTypes.number,
  target: PropTypes.oneOf(["_blank", "_self", "_parent", "_top"]),
  title: PropTypes.string,
  type: PropTypes.oneOf(["action", "page"]),
};

Link.defaultProps = {
  className: "",
  color: "#333333",
  fontSize: '13px',
  href: undefined,
  isBold: false,
  isHovered: false,
  isSemitransparent: false,
  isTextOverflow: false,
  noHover: false,
  rel: "noopener noreferrer",
  tabIndex: -1,
  type: "page",
};

export default Link;
