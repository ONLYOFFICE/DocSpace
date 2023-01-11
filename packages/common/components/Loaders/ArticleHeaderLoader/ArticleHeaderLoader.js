import React from "react";
import PropTypes from "prop-types";
import StyledContainer from "./StyledArticleHeader";
import RectangleLoader from "../RectangleLoader";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { isTablet, isMobileOnly } from "react-device-detect";
const ArticleHeaderLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    width,
    height,
    borderRadius,
    backgroundColor,
    foregroundColor,
    backgroundOpacity,
    foregroundOpacity,
    speed,
    animate,
  } = rest;
  const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;

  const loaderWidth = width ? width : isTabletView ? "28px" : "211px";
  const loaderHeight = height ? height : "28px";

  return (
    <StyledContainer id={id} className={className} style={style}>
      <RectangleLoader
        title={title}
        width={loaderWidth}
        height={loaderHeight}
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
    </StyledContainer>
  );
};

ArticleHeaderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

ArticleHeaderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default ArticleHeaderLoader;
