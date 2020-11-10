import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";

const StyledContainer = styled.div`
  padding-top: 13px;
  padding-bottom: 10px;
`;

const ArticleHeaderLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    x,
    y,
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
        x={x}
        y={y}
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

export default ArticleHeaderLoader;
