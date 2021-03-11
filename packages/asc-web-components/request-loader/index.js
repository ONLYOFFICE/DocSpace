import React from "react";
import PropTypes from "prop-types";
import Text from "../text";
import { OvalLoader, StyledInner, StyledOuter } from "./styled-request-loader";

const RequestLoader = (props) => {
  //console.log("RequestLoader render");
  const { loaderColor, loaderSize, label, fontColor, fontSize } = props;
  return (
    <StyledOuter {...props}>
      <StyledInner {...props}>
        <OvalLoader
          type="oval"
          color={loaderColor}
          size={loaderSize}
          label={label}
        />
        <Text className="text-style" color={fontColor} fontSize={fontSize}>
          {label}
        </Text>
      </StyledInner>
    </StyledOuter>
  );
};

RequestLoader.propTypes = {
  /** Visibility */
  visible: PropTypes.bool,
  /** CSS z-index */
  zIndex: PropTypes.number,
  /** Svg height and width value */
  loaderSize: PropTypes.string,
  /** Svg color */
  loaderColor: PropTypes.string,
  /** Svg aria-label and text label */
  label: PropTypes.string,
  /** Text label font size */
  fontSize: PropTypes.string,
  /** Text label font color */
  fontColor: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

RequestLoader.defaultProps = {
  visible: false,
  zIndex: 256,
  loaderSize: "16px",
  loaderColor: "#999",
  label: "Loading... Please wait...",
  fontSize: "12px",
  fontColor: "#999",
};

export default RequestLoader;
