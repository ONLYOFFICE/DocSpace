import React from "react";
import PropTypes from "prop-types";
import Button from "../../button";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  selectButtonLabel,
  isDisabled,
  isVisible,
  onClick,
  ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    border-top: 1px solid #eceef1;
    padding: 16px;
    height: 69px;

    ${props => !props.isVisible && css`display: none;`}
`;

const ADSelectorFooter = props => {
  const { selectButtonLabel, isDisabled, onClick } = props;

  return (
    <StyledContainer {...props}>
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
  selectButtonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  onClick: PropTypes.func
};

export default ADSelectorFooter;
