import React from "react";
import styled from 'styled-components';
import { Icons } from '../icons';
import {Popover, PopoverBody } from 'reactstrap';

const Caret = styled.div`
  width: 7px;
  position: absolute;
  right: 6px;
  transform: ${props => props.isOpen ? 'rotate(180deg)' : 'rotate(0)'};
  top: ${props => props.isOpen ? '2px' : '0'};
`;
const StyledHideFilterButton = styled.div`
  display: flex;
  position: relative;
  align-items: center;
  font-weight: 600;
  font-size: 16px;
  height: 100%;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;
  padding: 0 20px 0 9px;
  margin-right: 2px;
  cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
  font-family: Open Sans;
  font-style: normal;

  :hover{
    border-color: ${props => props.isDisabled ? '#ECEEF1' : '#A3A9AE'};
  }
  :active{
    background-color: ${props => props.isDisabled ? '#F8F9F9' : '#ECEEF1'};
  }
`;
const StyledHideFilter = styled.div`
  display: inline-block;
  height: 100%;
`;
const StyledPopoverBody = styled(PopoverBody)`
  border-radius: 6px;
  box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.13);
`;

class HideFilter extends React.Component {
  constructor(props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.state = {
      popoverOpen: false
    };
  }

  toggle() {
    if(!this.props.isDisabled){
      this.setState({
        popoverOpen: !this.state.popoverOpen
      });
    }
  }

  render() {
    //console.log("HideFilter render");
    return (
      <StyledHideFilter>
        <StyledHideFilterButton id="PopoverLegacy" isDisabled={this.props.isDisabled} >{this.props.count} <Caret isOpen={this.state.popoverOpen}><Icons.ExpanderDownIcon size='scale' isfill={true} color="#A3A9AE"/></Caret></StyledHideFilterButton>
        
        <Popover 
          isOpen={this.state.popoverOpen} 
          trigger="legacy" 
          placement="bottom-start" 
          target="PopoverLegacy"
          hideArrow={true} 
          toggle={this.toggle}>
          <StyledPopoverBody>{this.props.children}</StyledPopoverBody>
        </Popover>
      </StyledHideFilter>
    );
  }
}

export default HideFilter