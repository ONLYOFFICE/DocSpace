import React from "react";
import ContentLoader from "react-content-loader";
import PropTypes from "prop-types";
import { LoaderStyle } from "../../../constants";

const CircleLoader = ({ title, x, y, radius, width, height, ...rest }) => (
  <ContentLoader title={title} width={width} height={height} {...rest}>
    <circle cx={x} cy={y} r={radius} />
  </ContentLoader>
);

CircleLoader.propTypes = {
  title: PropTypes.string,
  x: PropTypes.string,
  y: PropTypes.string,
  width: PropTypes.string,
  height: PropTypes.string,
  radius: PropTypes.string,
  backgroundColor: PropTypes.string,
  foregroundColor: PropTypes.string,
  backgroundOpacity: PropTypes.number,
  foregroundOpacity: PropTypes.number,
  speed: PropTypes.number,
  animate: PropTypes.bool,
};

CircleLoader.defaultProps = {
  title: LoaderStyle.title,
  x: "3",
  y: "12",
  radius: "12",
  width: "100%",
  height: "100%",
  backgroundColor: LoaderStyle.backgroundColor,
  foregroundColor: LoaderStyle.foregroundColor,
  backgroundOpacity: LoaderStyle.backgroundOpacity,
  foregroundOpacity: LoaderStyle.foregroundOpacity,
  speed: LoaderStyle.speed,
  animate: LoaderStyle.animate,
};

export default CircleLoader;
