import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

const StyledBackdrop = styled.div`
  background-color: rgba(6, 22, 38, 0.1);
  display: ${(props) => (props.visible ? "block" : "none")};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: ${(props) => props.zIndex};
  left: 0;
  top: 0;
`;

const Backdrop = (props) => {
  //console.log("Backdrop render");
  return <StyledBackdrop {...props} />;
};

Backdrop.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 200,
};

export default Backdrop;
