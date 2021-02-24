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
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  loaderSize: PropTypes.string,
  loaderColor: PropTypes.string,
  label: PropTypes.string,
  fontSize: PropTypes.string,
  fontColor: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
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
