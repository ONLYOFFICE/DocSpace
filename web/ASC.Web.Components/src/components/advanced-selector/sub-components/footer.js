import React from "react";
import PropTypes from "prop-types";
import Button from "../../button";
import styled from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  buttonLabel,
  isDisabled,
  onClick,
  isMultiSelect,
  ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    border-top: 1px solid #eceef1;
    display: ${props => !props.isMultiSelect || !props.selectedOptions || !props.selectedOptions.length ? 'none' : 'flex'};

    .add_members_btn {
      margin: 16px;
    }
`;

const ADSelectorFooter = props => {
  const { buttonLabel, isDisabled, onClick } = props;

  return (
    <StyledContainer {...props}>
      <Button
        className="add_members_btn"
        primary={true}
        size="big"
        label={buttonLabel}
        scale={true}
        isDisabled={isDisabled}
        onClick={onClick}
      />
    </StyledContainer>
  );
};

ADSelectorFooter.propTypes = {
  buttonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  selectedOptions: PropTypes.array, 
  onClick: PropTypes.func
};

export default ADSelectorFooter;
