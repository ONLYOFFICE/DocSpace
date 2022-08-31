import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";
import Base from "../themes/base";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

class TextInput extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    // console.log(`TextInput render id=${this.props.id}`);

    return <ColorTheme {...this.props} themeId={ThemeType.TextInput} />;
  }
}

TextInput.propTypes = {
  /** Used as HTML `id` property */
  id: PropTypes.string,
  /** Used as HTML `name` property */
  name: PropTypes.string,
  /** Supported type of the input fields. */
  type: PropTypes.oneOf(["text", "password", "email"]),
  /** Value of the input */
  value: PropTypes.string.isRequired,
  maxLength: PropTypes.number,
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** Used as HTML `tabindex` property */
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
  /** Called when field is blurred */
  onBlur: PropTypes.func,
  /** Called when field is focused */
  onFocus: PropTypes.func,
  /** Focus the input field on initial render */
  isAutoFocussed: PropTypes.bool,
  /** Indicates that the field cannot be used (e.g not authorised, or changes not saved) */
  isDisabled: PropTypes.bool,
  /** Indicates that the field is displaying read-only content */
  isReadOnly: PropTypes.bool,
  /** Indicates the input field has an error */
  hasError: PropTypes.bool,
  /** Indicates the input field has a warning */
  hasWarning: PropTypes.bool,
  /** Used as HTML `autocomplete` property */
  autoComplete: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets the font weight */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Sets font weight value ​​to 600 */
  isBold: PropTypes.bool,
  /** Indicates that component contain border */
  withBorder: PropTypes.bool,
};

TextInput.defaultProps = {
  type: "text",
  value: "",
  maxLength: 255,
  size: "base",
  scale: false,
  tabIndex: -1,
  hasError: false,
  hasWarning: false,
  autoComplete: "off",
  withBorder: true,
  keepCharPositions: false,
  isBold: false,
};

export default TextInput;
