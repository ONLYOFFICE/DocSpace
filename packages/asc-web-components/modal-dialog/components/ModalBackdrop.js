import React from "react";
import styled from "styled-components";

const StyledModalBackdrop = styled.div.attrs((props) => ({
  style: {
    opacity: `${props.opacity}`,

    backdropFilter: `${
      props.modalSwipeOffset
        ? `blur(${18 - props.modalSwipeOffset * -0.15}px)`
        : "blur(18px)"
    }`,

    background: `${
      props.modalSwipeOffset
        ? `rgba(6, 22, 38, ${0.2 - props.modalSwipeOffset * -0.0016})`
        : `rgba(6, 22, 38, 0.2)`
    }`,
  },
}))`
  display: block;
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: 310;
  left: 0;
  top: 0;
`;

const ModalBackdrop = ({ zIndex, modalSwipeOffset, opacity, children }) => {
  return (
    <StyledModalBackdrop modalSwipeOffset={modalSwipeOffset}>
      {children}
    </StyledModalBackdrop>
  );
};

export default ModalBackdrop;
