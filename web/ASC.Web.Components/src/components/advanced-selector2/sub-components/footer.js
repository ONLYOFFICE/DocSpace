import React from "react";
import PropTypes from "prop-types";
import Button from "../../button";
import styled, { css } from "styled-components";

const StyledContainer = styled.div`
  border-top: 1px solid #eceef1;
  padding: 16px;
  height: 69px;

  ${props =>
    !props.isVisible &&
    css`
      display: none;
    `}
`;

const ADSelectorFooter = props => {
  const {
    selectButtonLabel,
    isDisabled,
    onClick,
    isVisible,
    className
  } = props;

  return (
    <StyledContainer isVisible={isVisible} className={className}>
      <Button
        className="add_members_btn"
        primary={true}
        size="big"
        label={selectButtonLabel}
        scale={true}
        isDisabled={isDisabled}
        onClick={onClick}
      />
    </StyledContainer>
  );
};

ADSelectorFooter.propTypes = {
  className: PropTypes.string,
  selectButtonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  onClick: PropTypes.func
};

export default ADSelectorFooter;
