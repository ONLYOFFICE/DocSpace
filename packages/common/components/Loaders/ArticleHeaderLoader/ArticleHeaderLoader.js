import React from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";

import StyledContainer from "./StyledArticleHeader";
import RectangleLoader from "../RectangleLoader";

const ArticleHeaderLoader = ({ id, className, style, showText, ...rest }) => {
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
    <StyledContainer
      id={id}
      className={className}
      style={style}
      showText={showText}
    >
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

export default inject(({ auth }) => {
  return {
    showText: auth.settingsStore.showText,
  };
})(observer(ArticleHeaderLoader));
