import React, { useState, useEffect, useRef }  from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import TextInput from '../text-input'
import { Icons } from '../icons'

const StyledOuter = styled.div`
  display: inline-block;
  position: relative;
`;

const StyledTextInput = styled(TextInput)`
  padding-right: 28px;
  width: 106px;
`;

const StyledIcon = styled(Icons.CalendarIcon)`
  position: absolute;
  right: 6px;
  top: 6px;
`;

const DateInput = props => {
  return (
    <StyledOuter>
      <StyledTextInput type="text" size="base" maxLength="10" {...props}/>
      <StyledIcon size="medium" onClick={props.onFocus}/>
    </StyledOuter>
  );
};

DateInput.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  value: PropTypes.string.isRequired,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,

  onChange: PropTypes.func,
  onBlur: PropTypes.func,
  onFocus: PropTypes.func,

  isDisabled: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool
}

DateInput.defaultProps = {
  value: '',
  tabIndex: -1,
  hasError: false,
  hasWarning: false
}

export default DateInput