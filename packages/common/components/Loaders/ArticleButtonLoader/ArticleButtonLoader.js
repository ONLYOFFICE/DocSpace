import React from "react";
import PropTypes from "prop-types";
import StyledContainer from "./StyledMainButton";
import RectangleLoader from "../RectangleLoader";

const ArticleButtonLoader = ({ id, className, style, ...rest }) => {
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
  return (
    <StyledContainer id={id} className={className} style={style}>
      <RectangleLoader
        title={title}
        width={width}
        height={height}
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

ArticleButtonLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

ArticleButtonLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default ArticleButtonLoader;
