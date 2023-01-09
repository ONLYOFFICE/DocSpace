import React from "react";
import PropTypes from "prop-types";

import styled, { css } from "styled-components";
import { smallTablet } from "../../utils/device";
import IconButton from "../../icon-button";
import Base from "../../themes/base";

const StyledCloseButtonWrapper = styled.div`
  width: 17px;
  height: 17px;

  display: flex;
  align-items: center;
  justify-content: center;

  cursor: pointer;
  position: absolute;

  ${(props) =>
    props.currentDisplayType === "modal"
      ? css`
          top: 18px;
          right: -30px;

          @media ${smallTablet} {
            right: 10px;
            top: -27px;
          }
        `
      : css`
          top: 18px;
          left: -27px;
          @media ${smallTablet} {
            top: -27px;
            left: auto;
            right: 10px;
          }
        `}

  .close-button, .close-button:hover {
    cursor: pointer;
    path {
      fill: ${(props) => props.theme.modalDialog.closeButton.fillColor};
    }
  }
`;

StyledCloseButtonWrapper.defaultProps = { theme: Base };

const CloseButton = ({ id, currentDisplayType, zIndex, onClick }) => {
  return (
    <StyledCloseButtonWrapper
      zIndex={zIndex}
      onClick={onClick}
      currentDisplayType={currentDisplayType}
      className="modal-close"
    >
      <IconButton
        size="17px"
        className="close-button"
        iconName="/static/images/cross.react.svg"
      />
    </StyledCloseButtonWrapper>
  );
};

CloseButton.propTypes = {
  currentDisplayType: PropTypes.oneOf(["modal", "aside"]),
  onClick: PropTypes.func,
};

export default CloseButton;
