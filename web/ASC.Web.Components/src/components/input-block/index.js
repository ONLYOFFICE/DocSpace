import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { InputGroup, InputGroupAddon } from 'reactstrap';
import TextInput from '../text-input';
import { Icons } from '../icons';
import IconButton from '../icon-button';
import commonInputStyle from '../text-input/common-input-styles';

const iconNames = Object.keys(Icons);

const Input = ({ isAutoFocussed, isDisabled, isReadOnly, hasError, hasWarning, iconName, scale, ...props }) => <input {...props} />;
const StyledTextInput = styled(Input)`
  border: none;
`;

const StyledIconBlock = styled.div`
  display: flex;
  align-items: center;
  width: ${props =>
    (props.size === 'base' && '22px') ||
    (props.size === 'middle' && '27px') ||
    (props.size === 'big' && '30px') ||
    (props.size === 'huge' && '30px')
  };
  height: 100%;
  padding-right: 7px;
`;

const StyledChildrenBlock = styled.div`
  display: flex;
  align-items: center;
  padding: 2px 0px 2px 2px;
`;

const CustomInputGroup = ({ isIconFill, hasError, hasWarning, isDisabled, scale, ...props }) => (
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

const InputBlock = React.forwardRef((props, ref) => {
  //console.log("InputBlock render");
  const { onChange, value, children, size } = props;
  
  let iconButtonSize = 0;
  
  if (typeof props.iconSize == "number" && props.iconSize > 0) {
    iconButtonSize = props.iconSize;
  } else {
    switch (size) {
      case 'base':
        iconButtonSize = 15;
        break;
      case 'middle':
        iconButtonSize = 18;
        break;
      case 'big':
        iconButtonSize = 21;
        break;
      case 'huge':
        iconButtonSize = 24;
        break;

      default:
        break;
    }
  }

  const onIconClick = (e) => {
    props.onIconClick && props.onIconClick(e, value);
  }

  return (
    <StyledInputGroup hasError={props.hasError} hasWarning={props.hasWarning} isDisabled={props.isDisabled} scale={props.scale} size={props.size}>
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
          <StyledIconBlock>
            <IconButton 
              size={iconButtonSize} 
              color={props.iconColor} 
              iconName={props.iconName} 
              isFill={props.isIconFill} 
              isDisabled={props.isDisabled} 
              onClick={onIconClick} />
          </StyledIconBlock>
        </InputGroupAddon>
      }
    </StyledInputGroup>
  );
});

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

  value: PropTypes.string,
  iconName: PropTypes.string,
  iconColor: PropTypes.string,
  iconSize: PropTypes.number,
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
  isDisabled: false
}

export default InputBlock