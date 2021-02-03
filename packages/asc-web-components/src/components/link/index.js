import React, { memo } from "react";
import PropTypes from "prop-types";
import StyledText from "./styled-link";

// eslint-disable-next-line react/display-name
const Link = memo(({ isTextOverflow, children, noHover, ...rest }) => {
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
  fontSize: "13px",
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
