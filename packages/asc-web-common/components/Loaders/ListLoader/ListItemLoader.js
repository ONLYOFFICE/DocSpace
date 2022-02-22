import React from "react";
import PropTypes from "prop-types";
import { StyledRow } from "./StyledListLoader";
import RectangleLoader from "../RectangleLoader";

const ListItemLoader = ({ id, className, style, isRectangle, ...rest }) => {
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
    <StyledRow id={id} className={className} style={style}>
      {isRectangle && (
        <RectangleLoader
          title={title}
          width="16"
          height="16"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
          className="list-loader_rectangle"
        />
      )}

      <RectangleLoader
        className="list-loader_rectangle-content"
        title={title}
        width="100%"
        height="100%"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />

      <RectangleLoader
        className="list-loader_rectangle-row"
        title={title}
        height="16px"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />

      <RectangleLoader
        title={title}
        width="16"
        height="16"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
    </StyledRow>
  );
};

ListItemLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  isRectangle: PropTypes.bool,
};

ListItemLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  isRectangle: true,
};

export default ListItemLoader;
