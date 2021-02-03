import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import TextInput from "../text-input";
import { Icons } from "../icons";
import IconButton from "../icon-button";
import commonInputStyle from "../text-input/common-input-styles";
import { Base } from "../../themes";

const iconNames = Object.keys(Icons);

const StyledIconBlock = styled.div`
  display: ${(props) => props.theme.inputBlock.display};
  align-items: ${(props) => props.theme.inputBlock.alignItems};
  cursor: ${(props) =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};

  height: ${(props) => props.theme.inputBlock.height};
  padding-right: ${(props) => props.theme.inputBlock.paddingRight};
  padding-left: ${(props) => props.theme.inputBlock.paddingLeft};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;
StyledIconBlock.defaultProps = { theme: Base };

const StyledChildrenBlock = styled.div`
  display: ${(props) => props.theme.inputBlock.display};
  align-items: ${(props) => props.theme.inputBlock.alignItems};
  padding: ${(props) => props.theme.inputBlock.padding};
`;
StyledChildrenBlock.defaultProps = { theme: Base };

/* eslint-disable react/prop-types, no-unused-vars */
const CustomInputGroup = ({
  isIconFill,
  hasError,
  hasWarning,
  isDisabled,
  scale,
  ...props
}) => <div {...props}></div>;
/* eslint-enable react/prop-types, no-unused-vars */
const StyledInputGroup = styled(CustomInputGroup)`
  display: ${(props) => props.theme.inputBlock.display};

  .prepend {
    display: ${(props) => props.theme.inputBlock.display};
    align-items: ${(props) => props.theme.inputBlock.alignItems};
  }

  .append {
    align-items: ${(props) => props.theme.inputBlock.alignItems};
    margin: ${(props) => props.theme.inputBlock.margin};
  }

  ${commonInputStyle} :focus-within {
    border-color: ${(props) => props.theme.inputBlock.borderColor};
  }
`;
StyledInputGroup.defaultProps = { theme: Base };
class InputBlock extends React.Component {
  constructor(props) {
    super(props);
  }
  onIconClick = (e) => {
    if (typeof this.props.onIconClick === "function" && !this.props.isDisabled)
      this.props.onIconClick(e);
  };
  onChange = (e) => {
    if (typeof this.props.onChange === "function") this.props.onChange(e);
  };

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
      iconSize,
      theme,
    } = this.props;

    if (typeof iconSize == "number" && iconSize > 0) {
      iconButtonSize = iconSize;
    } else {
      switch (size) {
        case "base":
          iconButtonSize = 16;
          break;
        case "middle":
          iconButtonSize = 18;
          break;
        case "big":
          iconButtonSize = 21;
          break;
        case "huge":
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
        theme={theme}
      >
        <div className="prepend">
          <StyledChildrenBlock className="prepend-children" theme={theme}>
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
        {iconNames.includes(iconName) && (
          <div className="append">
            <StyledIconBlock
              isDisabled={isDisabled}
              onClick={this.onIconClick}
              isClickable={typeof onIconClick === "function"}
            >
              <IconButton
                size={iconButtonSize}
                color={iconColor}
                hoverColor={hoverColor}
                iconName={iconName}
                isFill={isIconFill}
                isDisabled={isDisabled}
                isClickable={typeof onIconClick === "function"}
                theme={theme}
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
  type: PropTypes.oneOf(["text", "password"]),
  maxLength: PropTypes.number,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  mask: PropTypes.oneOfType([PropTypes.array, PropTypes.func]),
  keepCharPositions: PropTypes.bool,

  size: PropTypes.oneOf(["base", "middle", "big", "huge", "large"]),
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
    PropTypes.node,
  ]),

  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

InputBlock.defaultProps = {
  type: "text",
  maxLength: 255,
  size: "base",
  theme: Base,
  scale: false,
  tabIndex: -1,
  hasError: false,
  hasWarning: false,
  autoComplete: "off",

  value: "",
  iconName: "",
  iconColor: "#ffffff",
  hoverColor: "#ffffff",
  isIconFill: false,
  isDisabled: false,
  keepCharPositions: false,
};

export default InputBlock;
