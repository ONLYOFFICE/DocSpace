import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
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

const IconButtonBox = styled.div`
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

class SelectedItem extends React.PureComponent {
    constructor(props) {
      super(props);
    }

    stopAction = e => e.preventDefault();

    onCloseButtonClick = (e) => {
        if (!this.props.isDisabled) {
            this.props.clickAction && this.props.clickAction();
          } else {
            this.stopAction(e);
        }
    };
  
    render() {
      console.log("SelectedItem render");
      return (
        <StyledSelectedItem {...this.props} >
            <StyledSelectedTextBox>
                <Text.Body as='span' truncate color={this.props.isDisabled ? "#cecece" : "#333333"} >
                    {this.props.text}
                </Text.Body>
            </StyledSelectedTextBox>
            <IconButtonBox {...this.props} onClick={this.onCloseButtonClick}>
                <IconButton
                    color={"#D8D8D8"}
                    size={10}
                    iconName={'CrossIcon'}
                    isFill={true}
                    isDisabled={this.props.isDisabled}
                />
            </IconButtonBox>
        </StyledSelectedItem>
      );
    }
  }
  
  SelectedItem.propTypes = {
    text: PropTypes.string,
    isInline: PropTypes.bool,
    clickAction: PropTypes.func,
    isDisabled: PropTypes.bool
  };
  
  SelectedItem.defaultProps = {
    isInline: true,
    isDisabled: false
  };
  
  export default SelectedItem;