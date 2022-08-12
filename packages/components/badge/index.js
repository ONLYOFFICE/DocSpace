import React from "react";
import PropTypes from "prop-types";

import { StyledBadge, StyledInner, StyledText } from "./styled-badge";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const Badge = (props) => {
  //console.log("Badge render");

  const onClick = (e) => {
    if (!props.onClick) return;

    e.preventDefault();
    props.onClick(e);
  };

  const {
    fontSize,
    color,
    fontWeight,
    backgroundColor,
    borderRadius,
    padding,
    maxWidth,
    lineHeight,
    isHovered,
    label,
  } = props;

  return (
    <ColorTheme
      {...props}
      isHovered={isHovered}
      onClick={onClick}
      type={ThemeType.Badge}
    >
      <StyledInner
        backgroundColor={backgroundColor}
        borderRadius={borderRadius}
        padding={padding}
        maxWidth={maxWidth}
        lineHeight={lineHeight}
      >
        <StyledText
          textAlign="center"
          fontWeight={fontWeight}
          color={color}
          fontSize={fontSize}
        >
          {label}
        </StyledText>
      </StyledInner>
    </ColorTheme>
  );
};

Badge.propTypes = {
  /** Value */
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** CSS background-color */
  backgroundColor: PropTypes.string,
  /** CSS color */
  color: PropTypes.string,
  /** CSS font-size */
  fontSize: PropTypes.string,
  /** CSS font-weight */
  fontWeight: PropTypes.number,
  /** CSS border-radius */
  borderRadius: PropTypes.string,
  /** CSS padding */
  padding: PropTypes.string,
  /** CSS max-width */
  maxWidth: PropTypes.string,
  /** CSS line-height */
  lineHeight: PropTypes.string,
  /** onClick event */
  onClick: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Set hovered state and effects of link */
  isHovered: PropTypes.bool,
  /** Disabled hover styles */
  noHover: PropTypes.bool,
};

Badge.defaultProps = {
  label: 0,
  fontSize: "11px",
  fontWeight: 800,
  borderRadius: "11px",
  padding: "0 5px",
  maxWidth: "50px",
  lineHeight: "1.78",
  isHovered: false,
  noHover: false,
};

export default Badge;
