import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { InputGroup, InputGroupAddon } from 'reactstrap';
import TextInput from '../text-input';
import { Icons } from '../icons';
import commonInputStyle from '../text-input/common-input-styles';

const iconNames = Object.keys(Icons);

const Input = ({ isAutoFocussed, isDisabled, isReadOnly, hasError, hasWarning, iconName, scale, ...props }) => <input {...props}/>;
const StyledTextInput = styled(Input)`
  border: none;
`;

const StyledIconBlock = styled.div`
  display: flex;
  align-items: center;
  cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
  opacity: ${props => props.isDisabled ? '0.5' : '1'};
  width: 22px;
  height: 100%;
  padding-right: 7px;
`;

const StyledChildrenBlock = styled.div`
  display: flex;
  align-items: center;
  padding-left: 2px;
`;

const CustomInputGroup = ({ isIconFill, hasError, hasWarning, isDisabled,  ...props }) => (
  <InputGroup {...props}></InputGroup>
)
const StyledInputGroup = styled(CustomInputGroup)`
  ${commonInputStyle}
  :focus-within{
      border-color: #2DA7DB;
  }
  .input-group-prepend,
  .input-group-append{
      margin: 0;
  }
`;

const InputBlock = props => {
  const {onChange, value, children } = props;
  return (
    <StyledInputGroup  hasError={props.hasError} hasWarning={props.hasWarning} isDisabled={props.isDisabled} scale={props.scale} size={props.size}>
      <InputGroupAddon addonType="prepend">
        <StyledChildrenBlock>
          {children}
        </StyledChildrenBlock>
      </InputGroupAddon>
       <StyledTextInput 
          value={value}
          isDisabled={props.isDisabled}
          hasError={props.hasError}
          hasWarning={props.hasWarning}
          placeholder={props.placeholder}
          tabIndex={props.tabIndex}
          type={props.type}
          maxLength={props.maxLength}
          onBlur={props.onBlur}
          onFocus={props.onFocus}
          readOnly={props.isReadOnly}
          autoFocus={props.autoFocus}
          autoComplete={props.autoComplete}
          size={props.size}
          scale={props.scale}
          onChange={onChange}

          as={TextInput}
        />
        {
            iconNames.includes(props.iconName) 
            && 
              <InputGroupAddon addonType="append">
                <StyledIconBlock onClick={!props.isDisabled ? (e) => props.onIconClick(e, value) : undefined} isDisabled={props.isDisabled} size={props.size}>
                  {React.createElement(Icons[props.iconName], {size: "scale", color: props.iconColor, isfill: props.isIconFill})}
                </StyledIconBlock>
              </InputGroupAddon>
        }
    </StyledInputGroup>
  );
}

InputBlock.propTypes = {

  id: PropTypes.string,
  name: PropTypes.string,
  type: PropTypes.oneOf(['text', 'password']),
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
  autoComplete: PropTypes.string,

  value: PropTypes.string.isRequired,
  iconName: PropTypes.string,
  iconColor: PropTypes.string,
  isIconFill: PropTypes.bool,
  isDisabled: PropTypes.bool,
  onIconClick: PropTypes.func,
  onChange: PropTypes.func,

  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
}

InputBlock.defaultProps = {

  type: 'text',
  maxLength: 255,
  size: 'base',
  scale: false,
  tabIndex: -1,
  hasError: false,
  hasWarning: false,
  autoComplete: 'off',

  value: '',
  iconName: "",
  iconColor: "#ffffff",
  isIconFill: false,
  isDisabled: false,
  onIconClick: (e) => console.log("Icon click")
}

export default InputBlock