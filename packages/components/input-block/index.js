import React from "react";
import PropTypes from "prop-types";

import TextInput from "../text-input";
import { Icons } from "../icons";
import IconButton from "../icon-button";
import {
  StyledInputGroup,
  StyledChildrenBlock,
  StyledIconBlock,
} from "./styled-input-block";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

//const iconNames = Object.keys(Icons);

class InputBlock extends React.Component {
  constructor(props) {
    super(props);
  }
  onIconClick = (e) => {
    if (
      typeof this.props.onIconClick === "function" /*&& !this.props.isDisabled*/
    )
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
      onKeyDown,
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
      forwardedRef,
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
      <ColorTheme
        hasError={hasError}
        hasWarning={hasWarning}
        isDisabled={isDisabled}
        scale={scale}
        size={size}
        className={className}
        style={style}
        color={iconColor}
        themeId={ThemeType.InputBlock}
        hoverColor={hoverColor}
      >
        <div className="prepend">
          <StyledChildrenBlock className="prepend-children">
            {children}
          </StyledChildrenBlock>
        </div>
        <TextInput
          id={id}
          className={className}
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
          onKeyDown={onKeyDown}
          withBorder={false}
          mask={mask}
          keepCharPositions={keepCharPositions}
          forwardedRef={forwardedRef}
        />
        {
          //iconNames.includes(iconName) && (
          <div className="append">
            <StyledIconBlock
              className="input-block-icon"
              //isDisabled={isDisabled}
              onClick={this.onIconClick}
              isClickable={typeof onIconClick === "function"}
            >
              <IconButton
                size={iconButtonSize}
                iconName={iconName}
                isFill={isIconFill}
                //isDisabled={isDisabled}
                isClickable={typeof onIconClick === "function"}
                color={iconColor}
                hoverColor={hoverColor}
              />
            </StyledIconBlock>
          </div>
        }
      </ColorTheme>
    );
  }
}

InputBlock.propTypes = {
  /** Used as HTML `id` property */
  id: PropTypes.string,
  /** Used as HTML `name` property */
  name: PropTypes.string,
  /** Supported type of the input fields.  */
  type: PropTypes.oneOf(["text", "password"]),
  /** Define max length of value */
  maxLength: PropTypes.number,
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** Accepts css tab-index */
  tabIndex: PropTypes.number,
  /** input text mask */
  mask: PropTypes.oneOfType([PropTypes.array, PropTypes.func]),
  keepCharPositions: PropTypes.bool,
  /** Supported size of the input fields. */
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "large"]),
  /** Indicates the input field has scale */
  scale: PropTypes.bool,
  /** Called with the new value. Required when input is not read only. Parent should pass it back as `value` */
  onChange: PropTypes.func,
  /** Called when field is blurred  */
  onBlur: PropTypes.func,
  /** Called when field is focused  */
  onFocus: PropTypes.func,
  /** Focus the input field on initial render */
  isAutoFocussed: PropTypes.bool,
  /** Indicates that the field cannot be used (e.g not authorised, or changes not saved) */
  isDisabled: PropTypes.bool,
  /** Indicates that the field is displaying read-only content  */
  isReadOnly: PropTypes.bool,
  /** Indicates the input field has an error */
  hasError: PropTypes.bool,
  /** Indicates the input field has a warning */
  hasWarning: PropTypes.bool,
  /** Used as HTML `autocomplete` */
  autoComplete: PropTypes.string,
  /** Value of the input */
  value: PropTypes.string,
  /** Path to icon */
  iconName: PropTypes.string,
  /** Specifies the icon color  */
  iconColor: PropTypes.string,
  /** Icon color on hover action */
  hoverColor: PropTypes.string,
  /** Size icon */
  iconSize: PropTypes.number,
  /** Determines if icon fill is needed */
  isIconFill: PropTypes.bool,
  /**Will be triggered whenever an icon is clicked */
  onIconClick: PropTypes.func,

  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

InputBlock.defaultProps = {
  type: "text",
  maxLength: 255,
  size: "base",
  scale: false,
  tabIndex: -1,
  hasError: false,
  hasWarning: false,
  autoComplete: "off",

  value: "",
  iconName: "",
  isIconFill: false,
  isDisabled: false,
  keepCharPositions: false,
};

export default InputBlock;
