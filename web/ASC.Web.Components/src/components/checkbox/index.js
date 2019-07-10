import React, { useEffect, useRef }  from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import { Icons } from '../icons'

const borderColor = '#D0D5DA',
    activeColor = '#333333',
    disableColor = '#A3A9AE';

const Label = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
`;

const HiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
`;

const IconWrapper = styled.div`
  display: flex;
  align-items: center;  
  border: 1px solid ${borderColor};
  box-sizing: border-box;
  border-radius: 3px;
  width: 16px;
  height: 16px;
  padding: ${props => (props.isChecked && !props.isIndeterminate ? '0 2px' : '3px')};
`;

const TextWrapper = styled.span`
  margin-left: 8px;
  color: ${props => props.isDisabled ? disableColor : activeColor};
`;

class Checkbox extends React.Component  {

  componentDidMount() {
    if (this.props.isIndeterminate) {
      this.ref.current.indeterminate = true;
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps.isIndeterminate !== this.props.isIndeterminate) {
      this.ref.current.indeterminate = this.props.isIndeterminate;
    }
  }

  ref = React.createRef();

  render() {
    return (
    <Label htmlFor={this.props.id} isDisabled={this.props.isDisabled} >
      <HiddenInput type='checkbox' checked={this.props.isChecked && !this.props.isIndeterminate} disabled={this.props.isDisabled} ref={this.ref} {...this.props}/>
      <IconWrapper isChecked={this.props.isChecked} isIndeterminate={this.props.isIndeterminate}>
      {
        this.props.isIndeterminate
          ? <Icons.IndeterminateIcon isfill={true} size="scale" color={this.props.isDisabled ? disableColor : activeColor}/>
          : this.props.isChecked
            ? <Icons.CheckedIcon isfill={true} size="scale" color={this.props.isDisabled ? disableColor : activeColor}/>
            : ""
      }
      </IconWrapper>
      {
        this.props.label && <TextWrapper isDisabled={this.props.isDisabled}>{this.props.label}</TextWrapper>
      }
    </Label>
    );
  };
};

Checkbox.propTypes = {
  id: PropTypes.string.isRequired,
  name: PropTypes.string,
  value: PropTypes.string.isRequired,
  label: PropTypes.string,

  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isDisabled: PropTypes.bool,

  onChange: PropTypes.func,
}

export default Checkbox
