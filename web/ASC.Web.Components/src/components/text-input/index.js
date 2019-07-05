import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';

const Input = ({ isAutoFocussed, isDisabled, isReadOnly, hasError, hasWarning, scale,  ...props }) => <input {...props}/>;

const StyledInput = styled(Input).attrs((props) => ({
    id: props.id,
    name: props.name,
    type:  props.type,
    value: props.value,
    placeholder: props.placeholder,
    maxLength: props.maxLength,
    onChange: props.onChange,
    onBlur: props.onBlur,
    onFocus: props.onFocus,
    disabled: props.isDisabled,
    readOnly: props.isReadOnly,
    autoFocus: props.isAutoFocussed,
    autoComplete: props.autoComplete,
    tabIndex: props.tabIndex,
    disabled: props.isDisabled ? 'disabled' : ''

}))`
    -webkit-appearance: none;
    border-radius: 3px;
    box-shadow: none;
    box-sizing: border-box;
    border: solid 1px;
    border-color: ${props => (props.hasError && '#c30') || (props.hasWarning && '#f1ca92') || (props.isDisabled && '#ECEEF1')|| '#D0D5DA'};
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    background-color: ${props => props.isDisabled ? '#F8F9F9' : '#fff'};
    color: ${props => props.isDisabled ? '#D0D5DA' : '#333333'};
    display: flex;
    font-family: 'Open Sans', sans-serif;
    line-height: ${props =>
        (props.size === 'base' && '20px') ||
        (props.size === 'middle' && '20px') ||
        (props.size === 'big' && '20px') ||
        (props.size === 'huge' && '20px')
    };
    font-size: ${props =>
        (props.size === 'base' && '13px') ||
        (props.size === 'middle' && '14px') ||
        (props.size === 'big' && '16px') ||
        (props.size === 'huge' && '18px')
    };
    flex: 1 1 0%;
    outline: none;
    overflow: hidden;
    padding: ${props =>
        (props.size === 'base' && '5px 7px') ||
        (props.size === 'middle' && '8px 12px') ||
        (props.size === 'big' && '8px 16px') ||
        (props.size === 'huge' && '8px 20px')
    };
    transition: all 0.2s ease 0s;
    width: ${props =>
            (props.scale && '100%') ||
            (props.size === 'base' && '173px') ||
            (props.size === 'middle' && '300px') ||
            (props.size === 'big' && '350px') ||
            (props.size === 'huge' && '500px')
        };

    ::-webkit-input-placeholder {
        color: ${props => props.isDisabled ? '#D0D5DA' : '#A3A9AE'};
        font-family: 'Open Sans',sans-serif
    }

    :-moz-placeholder {
        color: ${props => props.isDisabled ? '#D0D5DA' : '#A3A9AE'};
        font-family: 'Open Sans',sans-serif
    }

    ::-moz-placeholder {
        color: ${props => props.isDisabled ? '#D0D5DA' : '#A3A9AE'};
        font-family: 'Open Sans',sans-serif
    }

    :-ms-input-placeholder {
        color: ${props => props.isDisabled ? '#D0D5DA' : '#A3A9AE'};
        font-family: 'Open Sans',sans-serif
    }

    :hover{
        border-color: ${props => props.isDisabled ? '#ECEEF1' : '#A3A9AE'};
    }
    :focus{
        border-color: #2DA7DB;
    }

`;

const TextInput = props => <StyledInput {...props} />

TextInput.propTypes = {
    id: PropTypes.string,
    name: PropTypes.string,
    type: PropTypes.oneOf(['text', 'password']),
    value: PropTypes.string.isRequired,
    maxLength: PropTypes.number,
    placeholder: PropTypes.string,
    tabIndex: PropTypes.number,

    size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
    scale: PropTypes.bool,

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

TextInput.defaultProps = {
    type: 'text',
    value: '',
    maxLength: 255,
    size: 'base',
    scale: false,
    tabIndex: -1,
    hasError: false,
    hasWarning: false,
    autoComplete: 'off'
}

export default TextInput
