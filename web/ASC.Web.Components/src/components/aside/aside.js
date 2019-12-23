import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Scrollbar from "../scrollbar";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  visible,
  scale,
  zIndex,
  ...props
}) => <aside {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledAside = styled(Container)`
  background-color: #fff;
  height: 100%;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  right: 0;
  top: 0;
  transform: translateX(${props => (props.visible ? "0" : props.scale ? "100%" : "320px")});
  transition: transform 0.3s ease-in-out;
  width: ${props => (props.scale ? "100%" : "320px")};
  z-index: ${props => props.zIndex};
  box-sizing: border-box;

  &.modal-dialog-aside {
    padding-bottom: 64px;

    .modal-dialog-aside-footer {
      position: fixed;
      bottom: 16px;
    }
  }
`;

const Aside = React.memo(props => {
  //console.log("Aside render");
  const { visible, children, scale, zIndex, className} = props;

  return (
    <StyledAside visible={visible} scale={scale} zIndex={zIndex} className={className}>
      <Scrollbar>{children}</Scrollbar>
    </StyledAside>
  );
});

Aside.displayName = "Aside";

Aside.propTypes = {
  visible: PropTypes.bool,
  scale: PropTypes.bool,
  className: PropTypes.string,
  zIndex: PropTypes.number,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
};
Aside.defaultProps = {
  scale: false,
  zIndex: 400
};

export default Aside;
