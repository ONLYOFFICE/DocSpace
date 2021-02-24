import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";
import Base from "@appserver/components/themes/base";
import StyledTextInput from "./styled-text-input";

class TextInput extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    // console.log(`TextInput render id=${this.props.id}`);

    return <StyledTextInput {...this.props} />;
  }
}

TextInput.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  type: PropTypes.oneOf(["text", "password", "email"]),
  value: PropTypes.string.isRequired,
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

  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),

  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isBold: PropTypes.bool,
};

TextInput.defaultProps = {
  type: "text",
  value: "",
  maxLength: 255,
  size: "base",
  theme: Base,
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
