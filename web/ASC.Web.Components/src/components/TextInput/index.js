import React from 'react'
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';

const StyledInput = styled.input.attrs((props) => ({
    id: props.id,
    name: props.name,
    type: "text",
    value:props.value,
    placeholder: props.placeholder,
    maxLength: props.maxLength,
    onChange: props.onChange,
    onBlur: props.onBlur,
    onFocus: props.onFocus,
    disabled: props.isDisabled,
    readOnly: props.isReadOnly,
    autoFocus: props.isAutoFocussed,
    autoComplete: props.autoComplete,
    tabIndex: props.tabIndex

  }))`
  -webkit-appearance: none;
  border-radius: 3px;
  box-shadow: none;
  box-sizing: border-box;
  border: solid 1px #c7c7c7;
  -moz-border-radius: 3px;
  -webkit-border-radius: 3px;
  background-color: #fff;
  color: #434341;
  display: flex;
  font-family: 'Open Sans', sans-serif;
  font-size: 18px;  
  flex: 1 1 0%;
  outline: none;
  overflow: hidden;
  padding: 8px 20px;
  transition: all 0.2s ease 0s;
  width: ${props => props.size === 'base' 
                ? '100px' 
                : (props.size === 'middle' 
                    ? '200px' 
                    : (props.size === 'big' 
                        ? '358px' 
                        : (props.size === 'huge' 
                            ? '500px' 
                            : '100%')))};

    ::-webkit-input-placeholder {
        color: #b2b2b2;
        font-family: 'Open Sans',sans-serif
    }

    :-moz-placeholder {
        color: #b2b2b2;
        font-family: 'Open Sans',sans-serif
    }

    ::-moz-placeholder {
        color: #b2b2b2;
        font-family: 'Open Sans',sans-serif
    }

    :-ms-input-placeholder {
        color: #b2b2b2;
        font-family: 'Open Sans',sans-serif
    }

`;

const TextInput = props => {

    return (
        <StyledInput {...props} />
    );
}

TextInput.propTypes = {
    
    id: PropTypes.string,
    name: PropTypes.string,
    value: PropTypes.string.isRequired,
    maxLength: PropTypes.number,
    placeholder: PropTypes.string,
    tabIndex: PropTypes.number,

    size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'scale']),

    onChange: PropTypes.func,
    onBlur: PropTypes.func,
    onFocus: PropTypes.func,
    
    isAutoFocussed: PropTypes.bool,
    isDisabled: PropTypes.bool,
    isReadOnly: PropTypes.bool,
    hasError: PropTypes.bool,
    hasWarning: PropTypes.bool,
    autoComplete: PropTypes.string
}

export default TextInput
