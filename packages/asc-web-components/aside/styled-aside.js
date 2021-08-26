import React from "react";
import styled from "styled-components";
import Base from "../themes/base";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  visible,
  scale,
  zIndex,
  contentPaddingBottom,
  ...props
}) => <aside {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledAside = styled(Container)`
  background-color: ${(props) => props.theme.aside.backgroundColor};
  height: ${(props) => props.theme.aside.height};
  overflow-x: ${(props) => props.theme.aside.overflowX};
  overflow-y: ${(props) => props.theme.aside.overflowY};
  position: fixed;
  right: ${(props) => props.theme.aside.right};
  top: ${(props) => props.theme.aside.top};
  transform: translateX(
    ${(props) => (props.visible ? "0" : props.scale ? "100%" : "325px")}
  );
  transition: ${(props) => props.theme.aside.transition};
  width: ${(props) => (props.scale ? "100%" : "325px")};
  z-index: ${(props) => props.zIndex};
  box-sizing: border-box;

  &.modal-dialog-aside {
    padding-bottom: ${(props) =>
      props.contentPaddingBottom
        ? props.contentPaddingBottom
        : props.theme.aside.paddingBottom};

    .modal-dialog-aside-footer {
      position: fixed;
      bottom: ${(props) => props.theme.aside.bottom};
    }
  }
`;
StyledAside.defaultProps = { theme: Base };
export default StyledAside;
