import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import IconButton from '../icon-button';

const StyledButton = styled.div`
    background: #F8F9F9;
    border: 1px solid #ECEEF1;
    box-sizing: border-box;
    border-radius: 3px;
    display: flex;
    align-items: center;
    padding: 0 8px;
    height: 32px;
    width: 32px;
    cursor: ${props => !props.isDisabled ? "pointer" : "default"};

    &:hover{
      path{
        ${props => !props.isDisabled && "fill: #333;"} 
      }
    }

    &:active{
      ${props => !props.isDisabled && "background-color: #ECEEF1;"}
    }
`;

const SelectorAddButton = (props) => {
  const {isDisabled, title, onClick } = props;

  return (
    <StyledButton isDisabled={isDisabled} title={title} onClick={onClick} >
      <IconButton
        color="#979797"
        size={14}
        iconName='PlusIcon'
        isFill={true}
        isDisabled={isDisabled}
      />
    </StyledButton>
  );
}

SelectorAddButton.propTypes = {
  title: PropTypes.string,
  onClick: PropTypes.func,
  isDisabled: PropTypes.bool
};

SelectorAddButton.defaultProps = {
  isDisabled: false
};

export default SelectorAddButton;