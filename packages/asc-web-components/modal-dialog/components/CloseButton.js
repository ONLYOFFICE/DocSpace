import React from "react";
import PropTypes from "prop-types";

import styled, { css } from "styled-components";
import { smallTablet } from "../../utils/device";
import IconButton from "../../icon-button";

const StyledCloseButtonWrapper = styled.div`
  position: absolute;
  width: 24px;
  height: 24px;

  display: flex;
  align-items: center;
  justify-content: center;

  background: #9a9ea3;
  border-radius: 50%;

  cursor: pointer;
  position: absolute;

  right: 0;
  top: 0;

  ${(props) =>
    props.displayType === "modal"
      ? css`
          margin-right: -34px;
          @media ${smallTablet} {
            margin-right: 10px;
            margin-top: -34px;
          }
        `
      : css`
          z-index: 1000;
          margin-top: 10px;
          right: 335px;
        `}

  .close-button {
    path {
      fill: #fff;
    }
  }
`;

const CloseButton = ({ displayType, onClick }) => {
  return (
    <StyledCloseButtonWrapper displayType={displayType}>
      <IconButton
        size={12}
        className="close-button"
        iconName="/static/images/cross.react.svg"
        onClick={onClick}
      />
    </StyledCloseButtonWrapper>
  );
};

CloseButton.propTypes = {
  displayType: PropTypes.oneOf(["modal", "aside"]),
  onClick: PropTypes.func,
};

export default CloseButton;
