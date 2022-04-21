import React from "react";
import PropTypes from "prop-types";

import styled, { css } from "styled-components";
import { smallTablet } from "../../utils/device";
import IconButton from "../../icon-button";
import Base from "../../themes/base";

const StyledCloseButtonWrapper = styled.div`
  position: absolute;
  width: 24px;
  height: 24px;

  display: flex;
  align-items: center;
  justify-content: center;

  background-color: ${(props) =>
    props.theme.modalDialog.closeButton.backgroundColor};
  border-radius: 50%;

  cursor: pointer;
  position: absolute;

  ${(props) =>
    props.currentDisplayType === "modal"
      ? css`
          top: 0;
          right: -34px;
          @media ${smallTablet} {
            right: 10px;
            top: -34px;
          }
        `
      : css`
          top: 10px;
          left: -34px;
          @media ${smallTablet} {
            top: -34px;
            left: auto;
            right: 10px;
          }
        `}

  .close-button {
    cursor: pointer;
    path {
      fill: ${(props) => props.theme.modalDialog.closeButton.fillColor};
    }
  }
`;

StyledCloseButtonWrapper.defaultProps = { theme: Base };

const CloseButton = ({ currentDisplayType, zIndex, onClick }) => {
  return (
    <StyledCloseButtonWrapper
      zIndex={zIndex}
      onClick={onClick}
      currentDisplayType={currentDisplayType}
    >
      <IconButton
        size={12}
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
