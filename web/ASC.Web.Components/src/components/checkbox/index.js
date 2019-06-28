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

const Checkbox = props => {
  const { id, isDisabled, isChecked, isIndeterminate, label } = props;
  const ref = useRef(null);

  useEffect(() => {
    ref.current.indeterminate = isIndeterminate;
  });

  return (
    <Label htmlFor={id} isDisabled={isDisabled} >
      <HiddenInput type='checkbox' checked={isChecked && !isIndeterminate} disabled={isDisabled} ref={ref} {...props}/>
      <IconWrapper isChecked={isChecked} isIndeterminate={isIndeterminate}>
      {
        isIndeterminate
          ? <Icons.IndeterminateIcon isfill={true} color={isDisabled ? disableColor : activeColor}/>
          : isChecked
            ? <Icons.CheckedIcon isfill={true} color={isDisabled ? disableColor : activeColor}/>
            : ""
      }
      </IconWrapper>
      {
        label && <TextWrapper isDisabled={isDisabled}>{label}</TextWrapper>
      }
    </Label>
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
