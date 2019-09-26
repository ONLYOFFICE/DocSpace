import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Scrollbar from "../../scrollbar";

const StyledAside = styled.aside`
  background-color: #fff;
  height: 100%;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  right: 0;
  top: 0;
  transform: translateX(${props => (props.visible ? "0" : props.scale ? "100%" : "240px")});
  transition: transform 0.3s ease-in-out;
  width: ${props => (props.scale ? "100%" : "240px")};
  z-index: 400;
`;

const Aside = React.memo(props => {
  //console.log("Aside render");
  const { visible, children, scale, className} = props;

  return (
    <StyledAside visible={visible} scale={scale} className={className}>
      <Scrollbar>{children}</Scrollbar>
    </StyledAside>
  );
});

Aside.displayName = "Aside";

Aside.propTypes = {
  visible: PropTypes.bool,
  scale: PropTypes.bool,
  className: PropTypes.string,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
};
Aside.defaultProps = {
  scale: false,
};

export default Aside;
