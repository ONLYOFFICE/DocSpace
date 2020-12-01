import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Loader from "../loader";
import Text from "../text";

const StyledOuter = styled.div`
  position: fixed;
  text-align: center;
  top: 10px;
  width: 100%;
  z-index: ${(props) => props.zIndex};
  display: ${(props) => (props.visible ? "block" : "none")};
`;

const StyledInner = styled.div`
  background-color: #fff;
  border: 1px solid #cacaca;
  display: inline-block;
  white-space: nowrap;
  overflow: hidden;
  padding: 5px 10px;
  line-height: 16px;
  z-index: ${(props) => props.zIndex};
  border-radius: 5px;
  -moz-border-radius: 5px;
  -webkit-border-radius: 5px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  -moz-box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  -webkit-box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);

  .text-style {
    display: contents;
  }
`;

const OvalLoader = styled(Loader)`
  display: inline;
  margin-right: 10px;
  svg {
    vertical-align: middle;
  }
`;

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
