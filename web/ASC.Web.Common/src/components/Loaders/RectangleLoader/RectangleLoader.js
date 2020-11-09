import React from "react";
import ContentLoader from "react-content-loader";
import PropTypes from "prop-types";
import { LoaderStyle } from "../../../constants/index";

const RectangleLoader = ({ x, y, borderRadius, width, height, ...rest }) => (
  <ContentLoader width={width} height={height} {...rest}>
    <rect
      x={x}
      y={y}
      rx={borderRadius}
      ry={borderRadius}
      width={width}
      height={height}
    />
  </ContentLoader>
);

RectangleLoader.propTypes = {
  x: PropTypes.string,
  y: PropTypes.string,
  width: PropTypes.string,
  height: PropTypes.string,
  borderRadius: PropTypes.string,
  backgroundColor: PropTypes.string,
  foregroundColor: PropTypes.string,
  backgroundOpacity: PropTypes.number,
  foregroundOpacity: PropTypes.number,
  speed: PropTypes.number,
  animate: PropTypes.bool,
};

RectangleLoader.defaultProps = {
  x: "0",
  y: "0",
  width: "100%",
  height: "32",
  borderRadius: LoaderStyle.borderRadius,
  backgroundColor: LoaderStyle.backgroundColor,
  foregroundColor: LoaderStyle.foregroundColor,
  backgroundOpacity: LoaderStyle.backgroundOpacity,
  foregroundOpacity: LoaderStyle.foregroundOpacity,
  speed: LoaderStyle.speed,
  animate: LoaderStyle.animate,
};

export default RectangleLoader;
