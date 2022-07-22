import React from "react";
import PropTypes from "prop-types";
import { StyledHeader, StyledSpacer } from "./StyledHeaderLoader";
import RectangleLoader from "../RectangleLoader";
import CircleLoader from "../CircleLoader/index";

const HeaderLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    borderRadius,
    backgroundColor,
    foregroundColor,
    backgroundOpacity,
    foregroundOpacity,
    speed,
    animate,
  } = rest;

  return (
    <StyledHeader id={id} className={className} style={style}>
      <RectangleLoader
        title={title}
        width="208"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledSpacer />
      <CircleLoader
        x="18"
        y="18"
        radius="18"
        width="36"
        height="36"
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
      />
    </StyledHeader>
  );
};

HeaderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

HeaderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  backgroundColor: "#fff",
  foregroundColor: "#fff",
  backgroundOpacity: 0.25,
  foregroundOpacity: 0.2,
};

export default HeaderLoader;
