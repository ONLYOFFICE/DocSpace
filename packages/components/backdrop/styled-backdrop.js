import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledBackdrop = styled.div`
  background-color: ${(props) =>
    props.needBackground
      ? props.theme.backdrop.backgroundColor
      : props.theme.backdrop.unsetBackgroundColor};
  ${(props) =>
    props.needBackground &&
    css`
      backdrop-filter: blur(3px);
    `};
  display: ${(props) => (props.visible ? "block" : "none")};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: ${(props) => props.zIndex};
  left: 0;
  top: 0;
  cursor: ${(props) =>
    props.needBackground && !props.isModalDialog ? "pointer" : "default"}; ;
`;

StyledBackdrop.defaultProps = {
  theme: Base,
};

export default StyledBackdrop;
