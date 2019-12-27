import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import TextInput from '../text-input';
import { Icons } from '../icons';
import IconButton from '../icon-button';
import commonInputStyle from '../text-input/common-input-styles';

const iconNames = Object.keys(Icons);

const StyledIconBlock = styled.div`
  display: flex;
  align-items: center;

  cursor: ${props =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};
  height: 100%;
  padding-right: 8px;
  padding-left: 1px;
  -webkit-tap-highlight-color: rgba(0,0,0,0);
`;

const StyledChildrenBlock = styled.div`
  display: flex;
  align-items: center;
  padding: 2px 0px 2px 2px;
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const CustomInputGroup = ({ isIconFill, hasError, hasWarning, isDisabled, scale, ...props }) => (
  <div {...props}></div>
);
const StyledInputGroup = styled(CustomInputGroup)`
  display: flex;

  .prepend {
    display: flex;
    align-items: center;
  }

  .append {
    align-items: center;
    margin: 0;
  }
  ${commonInputStyle} :focus-within {
    border-color: #2da7db;
  }
`;
class InputBlock extends React.Component {
  constructor(props) {
    super(props);
  }
  onIconClick = (e) => {
    if (typeof this.props.onIconClick === "function" && !this.props.isDisabled) this.props.onIconClick(e);
  }
  onChange = (e) => {
    if (typeof this.props.onChange === "function") this.props.onChange(e);
  }

  render() {
    let iconButtonSize = 0;
    const {
      hasError,
      hasWarning,
      isDisabled,
      scale,
      size,
      className,
      style,
      children,
      id,
      name,
      type,
      value,
      placeholder,
      tabIndex,
      maxLength,
      onBlur,
      onFocus,
      isReadOnly,
      isAutoFocussed,
      autoComplete,
      mask,
      keepCharPositions,
      iconName,
      iconColor,
      hoverColor,
      isIconFill,
      onIconClick,
      iconSize
    } = this.props;

    if (typeof iconSize == "number" && iconSize > 0) {
      iconButtonSize = iconSize;
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
      }
    }

    return (
      <StyledInputGroup
        hasError={hasError}
        hasWarning={hasWarning}
        isDisabled={isDisabled}
        scale={scale}
        size={size}
        className={className}
        style={style}
      >
        <div className="prepend">
          <StyledChildrenBlock>
            {children}
          </StyledChildrenBlock>
        </div>
        <TextInput
          id={id}
          name={name}
          type={type}
          value={value}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          placeholder={placeholder}
          tabIndex={tabIndex}
          maxLength={maxLength}
          onBlur={onBlur}
          onFocus={onFocus}
          isReadOnly={isReadOnly}
          isAutoFocussed={isAutoFocussed}
          autoComplete={autoComplete}
          size={size}
          scale={scale}
          onChange={this.onChange}
          withBorder={false}
          mask={mask}
          keepCharPositions={keepCharPositions}
        />
        {
          iconNames.includes(iconName) && (
            <div className="append">
              <StyledIconBlock 
                isDisabled={isDisabled}
                onClick={this.onIconClick}
                isClickable={typeof onIconClick === 'function'}>
                <IconButton
                  size={iconButtonSize}
                  color={iconColor}
                  hoverColor={hoverColor}
                  iconName={iconName}
                  isFill={isIconFill}
                  isDisabled={isDisabled}
                  isClickable={typeof onIconClick === 'function'}
                />
              </StyledIconBlock>
            </div>
          )}
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
  hoverColor: PropTypes.string,
  iconSize: PropTypes.number,
  isIconFill: PropTypes.bool,
  onIconClick: PropTypes.func,

  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),

  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
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
  hoverColor: "#ffffff",
  isIconFill: false,
  isDisabled: false,
  keepCharPositions: false
}

export default InputBlock