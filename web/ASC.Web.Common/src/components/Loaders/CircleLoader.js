import React from "react";
import ContentLoader from "react-content-loader";
import PropTypes from "prop-types";
import { LoaderStyle } from "../../constants/index";

const CircleLoader = (props) => (
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
    <circle cx="10" cy="12" r={props.radius} />
  </ContentLoader>
);

CircleLoader.PropTypes = {
  radius: PropTypes.string,
  backgroundColor: PropTypes.string,
  foregroundColor: PropTypes.string,
  backgroundOpacity: PropTypes.number,
  foregroundOpacity: PropTypes.number,
  speed: PropTypes.number,
};

CircleLoader.defaultProps = {
  radius: "12",
  backgroundColor: LoaderStyle.backgroundColor,
  foregroundColor: LoaderStyle.foregroundColor,
  backgroundOpacity: LoaderStyle.backgroundOpacity,
  foregroundOpacity: LoaderStyle.foregroundOpacity,
  speed: LoaderStyle.speed,
};

export default CircleLoader;
