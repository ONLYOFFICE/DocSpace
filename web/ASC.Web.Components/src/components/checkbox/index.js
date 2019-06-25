import React, { useState, useEffect, useRef }  from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import { Icons } from '../icons'

const borderColor = '#D0D5DA',
    activeColor = '#333333',
    disableColor = '#A3A9AE';

const StyledOuter = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;
  line-height: 16px;
`;

const StyledInput = styled.input.attrs((props) => ({
  id: props.id,
  name: props.name,
  value: props.value,
  label: props.label,

  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isDisabled: PropTypes.bool,

  onChange: props.onChange
}))`
  right: 0;
  opacity: 0.0001;
  position: absolute;
  top: 0;
`;

const StyledInner = styled.div`
  display: flex;
  align-items: center;  
  border: 1px solid ${borderColor};
  box-sizing: border-box;
  border-radius: 3px;
  width: 16px;
  height: 16px;
  padding: ${props => (props.isChecked && !props.isIndeterminate ? '0 2px' : '3px')};
  cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
`;

const StyledText = styled.span`
  margin: 0 8px;
  cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
  color: ${props => props.isDisabled ? disableColor : activeColor};
`;

const Checkbox = props => {

  const ref = useRef(null);

  useEffect(() => {
    ref.current.indeterminate = props.isIndeterminate;
  });

  return (
    <StyledOuter for={props.id}>
      <StyledInput type='checkbox' checked={props.isChecked && !props.isIndeterminate} disabled={props.isDisabled} ref={ref} {...props}/>
      <StyledInner {...props}>
      {
        props.isIndeterminate
          ? <Icons.RectangleIcon color={props.isDisabled ? disableColor : activeColor}/>
          : props.isChecked
            ? <Icons.TickIcon color={props.isDisabled ? disableColor : activeColor}/>
            : ""
      }
      </StyledInner>
      {
        props.label
          ? <StyledText {...props}>{props.label}</StyledText>
          : ""
      }
    </StyledOuter>
  );
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
