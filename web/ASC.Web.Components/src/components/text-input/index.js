import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import commonInputStyle from "../text-input/common-input-styles";
import MaskedInput from "react-text-mask";
import isEqual from "lodash/isEqual";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Input = ({
  isAutoFocussed,
  isDisabled,
  isReadOnly,
  hasError,
  hasWarning,
  scale,
  withBorder,
  keepCharPositions,
  fontWeight,
  isBold,
  ...props
}) =>
  props.mask != null ? (
    <MaskedInput keepCharPositions {...props} />
  ) : (
    <input {...props} />
  );
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledInput = styled(Input).attrs((props) => ({
  id: props.id,
  name: props.name,
  type: props.type,
  value: props.value,
  placeholder: props.placeholder,
  maxLength: props.maxLength,
  onChange: props.onChange,
  onBlur: props.onBlur,
  onFocus: props.onFocus,
  readOnly: props.isReadOnly,
  autoFocus: props.isAutoFocussed,
  autoComplete: props.autoComplete,
  tabIndex: props.tabIndex,
  disabled: props.isDisabled ? "disabled" : "",
}))`
  ${commonInputStyle}
  -webkit-appearance: none;
  display: flex;
  font-family: "Open Sans", sans-serif;
  line-height: ${(props) =>
    (props.size === "base" && "20px") ||
    (props.size === "middle" && "20px") ||
    (props.size === "big" && "20px") ||
    (props.size === "huge" && "21px") ||
    (props.size === "large" && "20px")};
  font-size: ${(props) =>
    (props.size === "base" && "14px") ||
    (props.size === "middle" && "14px") ||
    (props.size === "big" && "16px") ||
    (props.size === "huge" && "18px") ||
    (props.size === "large" && "16px")};

  font-weight: ${(props) =>
    props.fontWeight ? props.fontWeight : props.isBold ? 600 : "normal"};

  flex: 1 1 0%;
  outline: none;
  overflow: hidden;
  opacity: 1;
  padding: ${(props) =>
    (props.size === "base" && "5px 6px") ||
    (props.size === "middle" && "8px 12px") ||
    (props.size === "big" && "8px 16px") ||
    (props.size === "huge" && "8px 20px") ||
    (props.size === "large" && "11px 15px")};
  transition: all 0.2s ease 0s;

  ::-webkit-input-placeholder {
    color: ${(props) => (props.isDisabled ? "#A3A9AE" : "#D0D5DA")};
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  :-moz-placeholder {
    color: ${(props) => (props.isDisabled ? "#A3A9AE" : "#D0D5DA")};
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  ::-moz-placeholder {
    color: ${(props) => (props.isDisabled ? "#A3A9AE" : "#D0D5DA")};
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  :-ms-input-placeholder {
    color: ${(props) => (props.isDisabled ? "#A3A9AE" : "#D0D5DA")};
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  ${(props) => !props.withBorder && `border: none;`}
`;

class TextInput extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    // console.log(`TextInput render id=${this.props.id}`);
    return <StyledInput {...this.props} />;
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
