import styled from "styled-components";
import commonInputStyles from "./common-input-styles";
import Input from "./input";
import Base from "../themes/base";
import { isMobile } from "react-device-detect";
import NoUserSelect from "../utils/commonStyles";
/* eslint-enable react/prop-types, no-unused-vars */
const StyledTextInput = styled(Input).attrs((props) => ({
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
  ${commonInputStyles}
  -webkit-appearance: ${(props) => props.theme.textInput.appearance};

  background-color: ${(props) => props.theme.input.backgroundColor};
  -webkit-text-fill-color: ${(props) =>
    props?.value.length > 0
      ? props.theme.text.color
      : props.theme.textInput.placeholderColor} !important;
  caret-color: ${(props) => props.theme.text.color};
  -webkit-background-clip: text !important;
  box-shadow: inset 0 0 20px 20px
    ${(props) => props.theme.input.backgroundColor} !important;

  display: ${(props) => props.theme.textInput.display};
  font-family: ${(props) => props.theme.fontFamily};
  line-height: ${(props) =>
    (props.size === "base" && props.theme.textInput.lineHeight.base) ||
    (props.size === "middle" && props.theme.textInput.lineHeight.middle) ||
    (props.size === "big" && props.theme.textInput.lineHeight.big) ||
    (props.size === "huge" && props.theme.textInput.lineHeight.huge) ||
    (props.size === "large" && props.theme.textInput.lineHeight.large)};
  font-size: ${(props) =>
    (props.size === "base" && props.theme.textInput.fontSize.base) ||
    (props.size === "middle" && props.theme.textInput.fontSize.middle) ||
    (props.size === "big" && props.theme.textInput.fontSize.big) ||
    (props.size === "huge" && props.theme.textInput.fontSize.huge) ||
    (props.size === "large" && props.theme.textInput.fontSize.large)};

  font-weight: ${(props) =>
    props.fontWeight
      ? props.fontWeight
      : props.isBold
      ? 600
      : props.theme.textInput.fontWeight};

  flex: ${(props) => props.theme.textInput.flex};
  outline: ${(props) => props.theme.textInput.outline};
  overflow: ${(props) => props.theme.textInput.overflow};
  opacity: ${(props) => props.theme.textInput.opacity};

  width: ${(props) =>
    (props.scale && "100%") ||
    (props.size === "base" && props.theme.input.width.base) ||
    (props.size === "middle" && props.theme.input.width.middle) ||
    (props.size === "big" && props.theme.input.width.big) ||
    (props.size === "huge" && props.theme.input.width.huge) ||
    (props.size === "large" && props.theme.input.width.large)};
  padding: ${(props) =>
    (props.size === "base" && props.theme.textInput.padding.base) ||
    (props.size === "middle" && props.theme.textInput.padding.middle) ||
    (props.size === "big" && props.theme.textInput.padding.big) ||
    (props.size === "huge" && props.theme.textInput.padding.huge) ||
    (props.size === "large" && props.theme.textInput.padding.large)};

  transition: ${(props) => props.theme.textInput.transition};

  ::-webkit-input-placeholder {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.textInput.disablePlaceholderColor
        : props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    ${NoUserSelect}
  }

  :-moz-placeholder {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.textInput.disablePlaceholderColor
        : props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    ${NoUserSelect}
  }

  ::-moz-placeholder {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.textInput.disablePlaceholderColor
        : props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    ${NoUserSelect}
  }

  :-ms-input-placeholder {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.textInput.disablePlaceholderColor
        : props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    ${NoUserSelect}
  }

  ::placeholder {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.textInput.disablePlaceholderColor
        : props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    ${NoUserSelect}
  }

  ${(props) => !props.withBorder && `border: none;`}
`;

StyledTextInput.defaultProps = { theme: Base };

export default StyledTextInput;
