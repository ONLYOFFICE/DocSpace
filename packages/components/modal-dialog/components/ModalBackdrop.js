import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

const StyledModalBackdrop = styled.div.attrs((props) => ({
  style: {
    backdropFilter: `${
      props.modalSwipeOffset
        ? `blur(${
            props.modalSwipeOffset < 0 && 8 - props.modalSwipeOffset * -0.0667
          }px)`
        : "blur(8px)"
    }`,

    background: `${
      props.modalSwipeOffset
        ? `rgba(6, 22, 38, ${
            props.modalSwipeOffset < 0 && 0.2 - props.modalSwipeOffset * -0.002
          })`
        : `rgba(6, 22, 38, 0.2)`
    }`,
  },
}))`
  display: block;
  height: 100%;
  min-height: fill-available;
  max-height: 100vh;
  width: 100vw;

  position: fixed;
  left: 0;
  top: 0;
  z-index: ${(props) => props.zIndex};

  transition: 0.2s;
  opacity: 0;
  &.modal-backdrop-active {
    opacity: 1;
  }
`;

const ModalBackdrop = ({ className, zIndex, modalSwipeOffset, children }) => {
  return (
    <StyledModalBackdrop
      zIndex={zIndex}
      className={className}
      modalSwipeOffset={modalSwipeOffset}
    >
      {children}
    </StyledModalBackdrop>
  );
};

ModalBackdrop.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.any,

  zIndex: PropTypes.number,
  visible: PropTypes.bool,
  modalSwipeOffset: PropTypes.number,
};

export default ModalBackdrop;
