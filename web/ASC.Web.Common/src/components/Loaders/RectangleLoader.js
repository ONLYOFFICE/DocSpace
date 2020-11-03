import React from "react";
import ContentLoader from "react-content-loader";
import PropTypes from 'prop-types';
import { LoaderStyle } from "../../constants/index";

const RectangleLoader = (props) => (
  <ContentLoader
    speed={props.speed}
    width={"100%"}
    height={32}
    backgroundColor={props.backgroundColor}
    foregroundColor={props.foregroundColor}
    backgroundOpacity={props.backgroundOpacity}
    foregroundOpacity={props.foregroundOpacity}
    {...props}
  >
    <rect x="0" y="0" rx={props.borderRadius} ry={props.borderRadius} width={props.width} height={props.height} />
  </ContentLoader>
);

RectangleLoader.PropTypes = {
  width: PropTypes.string,
  height: PropTypes.string,
  borderRadius: PropTypes.string,
  backgroundColor: PropTypes.string,
  foregroundColor: PropTypes.string,
  backgroundOpacity: PropTypes.number,
  foregroundOpacity: PropTypes.number,
  speed: PropTypes.number
};

RectangleLoader.defaultProps = {
  width: "100%",
  height: "32",
  borderRadius: LoaderStyle.borderRadius,
  backgroundColor: LoaderStyle.backgroundColor,
  foregroundColor: LoaderStyle.foregroundColor,
  backgroundOpacity: LoaderStyle.backgroundOpacity,
  foregroundOpacity: LoaderStyle.foregroundOpacity,
  speed: LoaderStyle.speed
};

export default RectangleLoader;
