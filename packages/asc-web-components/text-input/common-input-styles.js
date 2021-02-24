import { css } from "styled-components";

const commonInputStyle = css`
  width: ${(props) =>
    (props.scale && "100%") ||
    (props.size === "base" && props.theme.input.width.base) ||
    (props.size === "middle" && props.theme.input.width.middle) ||
    (props.size === "big" && props.theme.input.width.big) ||
    (props.size === "huge" && props.theme.input.width.huge) ||
    (props.size === "large" && props.theme.input.width.large)};

  background-color: ${(props) =>
    props.isDisabled
      ? props.theme.input.disableBackgroundColor
      : props.theme.input.backgroundColor};
  color: ${(props) =>
    props.isDisabled
      ? props.theme.input.disableColor
      : props.color
      ? props.color
      : props.theme.input.color};

  border-radius: ${(props) => props.theme.input.borderRadius};
  -moz-border-radius: ${(props) => props.theme.input.borderRadius};
  -webkit-border-radius: ${(props) => props.theme.input.borderRadius};

  box-shadow: ${(props) => props.theme.input.boxShadow};
  box-sizing: ${(props) => props.theme.input.boxSizing};
  border: ${(props) => props.theme.input.border};
  border-color: ${(props) =>
    (props.hasError && props.theme.input.errorBorderColor) ||
    (props.hasWarning && props.theme.input.warningBorderColor) ||
    (props.isDisabled && props.theme.input.disabledBorderColor) ||
    props.theme.input.borderColor};

  :hover {
    border-color: ${(props) =>
      (props.hasError && props.theme.input.hoverErrorBorderColor) ||
      (props.hasWarning && props.theme.input.hoverWarningBorderColor) ||
      (props.isDisabled && props.theme.input.hoverDisabledBorderColor) ||
      props.theme.input.hoverBorderColor};
  }
  :focus {
    border-color: ${(props) =>
      (props.hasError && props.theme.input.focusErrorBorderColor) ||
      (props.hasWarning && props.theme.input.focusWarningBorderColor) ||
      (props.isDisabled && props.theme.input.focusDisabledBorderColor) ||
      props.theme.input.focusBorderColor};
  }

  cursor: ${(props) =>
    props.isReadOnly || props.isDisabled ? "default" : "text"};
`;

export default commonInputStyle;
