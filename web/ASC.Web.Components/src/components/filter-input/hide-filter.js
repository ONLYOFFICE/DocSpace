import React from "react";
import styled from "styled-components";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import { handleAnyClick } from "../../utils/event";
import PropTypes from 'prop-types';

const Caret = styled.div`
  width: 7px;
  position: absolute;
  right: 6px;
  transform: ${props => (props.isOpen ? "rotate(180deg)" : "rotate(0)")};
  top: ${props => (props.isOpen ? "2px" : "0")};
`;

const StyledHideFilterButton = styled.div`
  box-sizing: border-box;
  display: flex;
  position: relative;
  align-items: center;
  font-weight: 600;
  font-size: 16px;
  height: 100%;
  border: 1px solid #eceef1;
  border-radius: 3px;
  background-color: #f8f9f9;
  padding: 0 20px 0 9px;
  margin-right: 2px;
  cursor: ${props => (props.isDisabled ? "default" : "pointer")};
  font-family: Open Sans;
  font-style: normal;

  :hover {
    border-color: ${props => (props.isDisabled ? "#ECEEF1" : "#A3A9AE")};
  }
  :active {
    background-color: ${props => (props.isDisabled ? "#F8F9F9" : "#ECEEF1")};
  }
`;
const StyledHideFilter = styled.div`
  display: inline-block;
  height: 100%;
`;
const DropDownStyle = styled.div`
  .drop-down {
    padding: 16px;
  }
  position: relative;
`;

class HideFilter extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.dropDownRef = React.createRef();
    this.state = {
      popoverOpen: this.props.open
    };
  }

  onClick = (state, e) => {
    if (!state && e && this.dropDownRef.current.contains(e.target)) {
      return;
    }
    if (!this.props.isDisabled) {
      this.setState({
        popoverOpen: state
      });
    }
  };

  handleClick = e => {
    this.state.popoverOpen &&
      !this.dropDownRef.current.firstElementChild.contains(e.target) &&
      this.onClick(false);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevState) {
    if (this.state.popoverOpen !== prevState.popoverOpen) {
      handleAnyClick(this.state.popoverOpen, this.handleClick);
    }
  }

  render() {
    //console.log("HideFilter render");
    return (
      <StyledHideFilter
        onClick={this.onClick.bind(this, !this.state.popoverOpen)}
        ref={this.ref}
      >
        <StyledHideFilterButton
          id="PopoverLegacy"
          isDisabled={this.props.isDisabled}
        >
          {this.props.count}
          <Caret isOpen={this.state.popoverOpen}>
            <Icons.ExpanderDownIcon
              size="scale"
              isfill={true}
              color="#A3A9AE"
            />
          </Caret>
        </StyledHideFilterButton>

        <DropDownStyle ref={this.dropDownRef}>
          <DropDown
            className="drop-down"
            manualY="8px"
            open={this.state.popoverOpen}
            clickOutsideAction={this.handleClick}
          >
            {this.props.children}
          </DropDown>
        </DropDownStyle>
      </StyledHideFilter>
    );
  }
}
HideFilter.propTypes = {
  children: PropTypes.any,
  open: PropTypes.bool,
  isDisabled: PropTypes.bool,
  count: PropTypes.number
}
export default HideFilter;
