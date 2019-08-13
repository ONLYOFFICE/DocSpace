import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Text } from '../text';
import IconButton from '../icon-button';

const StyledSelectedItem = styled.div`
    position: relative;
    display: ${props => (props.isInline ? 'inline-grid' : 'grid')};
    grid-template-columns: 1fr auto;
    background: #F8F9F9;
    border: 1px solid #ECEEF1;
    box-sizing: border-box;
    border-radius: 3px;
`;

const StyledSelectedTextBox = styled.div`
    padding: 0 8px;
    display: grid;
    height: 32px;
    align-items: center;
    border-right: 1px solid #ECEEF1;
    cursor: default;
`;

const StyledCloseButton = styled.div`
    display: flex;
    align-items: center;
    padding: 0 8px;
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

const SelectedItem = (props) => {
    const {isDisabled, text, onClose } = props;
    const colorProps = isDisabled ? {color: "#D0D5DA"} : {};
 
      //console.log("SelectedItem render");
      return (
        <StyledSelectedItem {...props}>
            <StyledSelectedTextBox>
                <Text.Body as='span' truncate {...colorProps} >
                    {text}
                </Text.Body>
            </StyledSelectedTextBox>
            <StyledCloseButton onClick={onClose} isDisabled={isDisabled}>
                <IconButton
                    color="#979797"
                    size={10}
                    iconName='CrossIcon'
                    isFill={true}
                    isDisabled={isDisabled}
                />
            </StyledCloseButton>
        </StyledSelectedItem>
      );
  }
  
  SelectedItem.propTypes = {
    text: PropTypes.string,
    isInline: PropTypes.bool,
    onClose: PropTypes.func.isRequired,
    isDisabled: PropTypes.bool
  };
  
  SelectedItem.defaultProps = {
    isInline: true,
    isDisabled: false
  };
  
  export default SelectedItem;