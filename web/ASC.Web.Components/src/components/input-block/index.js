import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { InputGroup, InputGroupAddon } from 'reactstrap';
import TextInput from '../text-input';
import { Icons } from '../icons';
import IconButton from '../icon-button';
import commonInputStyle from '../text-input/common-input-styles';

const iconNames = Object.keys(Icons);

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
);
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
class InputBlock extends React.Component {
  constructor(props) {
    super(props);

    this.onIconClick = this.onIconClick.bind(this);
    this.onChange = this.onChange.bind(this);

  }
  onIconClick(e) {
    if (typeof this.props.onIconClick === "function") this.props.onIconClick(e);
  }
  onChange(e) {
    if (typeof this.props.onChange === "function") this.props.onChange(e);
  }

  render() {
    let iconButtonSize = 0;

    if (typeof this.props.iconSize == "number" && this.props.iconSize > 0) {
      iconButtonSize = this.props.iconSize;
    } else {
      switch (this.props.size) {
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

    return (
      <StyledInputGroup hasError={this.props.hasError} hasWarning={this.props.hasWarning} isDisabled={this.props.isDisabled} scale={this.props.scale} size={this.props.size}>
        <InputGroupAddon addonType="prepend">
          <StyledChildrenBlock>
            {this.props.children}
          </StyledChildrenBlock>
        </InputGroupAddon>
        <TextInput
          id={this.props.id}
          name={this.props.name}
          type={this.props.type}
          value={this.props.value}
          isDisabled={this.props.isDisabled}
          hasError={this.props.hasError}
          hasWarning={this.props.hasWarning}
          placeholder={this.props.placeholder}
          tabIndex={this.props.tabIndex}
          maxLength={this.props.maxLength}
          onBlur={this.props.onBlur}
          onFocus={this.props.onFocus}
          isReadOnly={this.props.isReadOnly}
          isAutoFocussed={this.props.isAutoFocussed}
          autoComplete={this.props.autoComplete}
          size={this.props.size}
          scale={this.props.scale}
          onChange={this.onChange}
          withBorder={false}
          mask={this.props.mask}
          keepCharPositions={this.props.keepCharPositions}
        />
        {
          iconNames.includes(this.props.iconName)
          &&
          <InputGroupAddon addonType="append">
            <StyledIconBlock>
              <IconButton
                size={iconButtonSize}
                color={this.props.iconColor}
                iconName={this.props.iconName}
                isFill={this.props.isIconFill}
                isDisabled={this.props.isDisabled}
                onClick={this.onIconClick}
                isClickable={typeof this.props.onIconClick === 'function'}
              />
            </StyledIconBlock>
          </InputGroupAddon>
        }
      </StyledInputGroup>
    );
  }
}

InputBlock.propTypes = {

  id: PropTypes.string,
  name: PropTypes.string,
  type: PropTypes.oneOf(['text', 'password']),
  maxLength: PropTypes.number,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  mask: PropTypes.oneOfType([PropTypes.array, PropTypes.func]),
  keepCharPositions: PropTypes.bool,

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
  onIconClick: PropTypes.func,

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
  keepCharPositions: false
}

export default InputBlock