import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const paddingRightStyle = (props) =>
  props.theme.fileInput.paddingRight[props.size];

const widthIconStyle = (props) => props.theme.fileInput.icon.width[props.size];
const heightIconStyle = (props) =>
  props.theme.fileInput.icon.height[props.size];

const StyledFileInput = styled.div`
  display: flex;
  position: relative;
  outline: none;
  .file-text-input {
    width: 100%;
    margin: 0;
  }
  width: ${(props) =>
    (props.scale && "100%") ||
    (props.size === "base" && props.theme.input.width.base) ||
    (props.size === "middle" && props.theme.input.width.middle) ||
    (props.size === "big" && props.theme.input.width.big) ||
    (props.size === "huge" && props.theme.input.width.huge) ||
    (props.size === "large" && props.theme.input.width.large)};

  .text-input {
    border-color: ${(props) =>
      (props.hasError && props.theme.input.errorBorderColor) ||
      (props.hasWarning && props.theme.input.warningBorderColor) ||
      (props.isDisabled && props.theme.input.disabledBorderColor) ||
      props.theme.input.borderColor};

    text-overflow: ellipsis;
    padding-right: 40px;
    padding-right: ${(props) => paddingRightStyle(props)};
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }

  :hover {
    .icon {
      border-color: ${(props) =>
        (props.hasError && props.theme.input.hoverErrorBorderColor) ||
        (props.hasWarning && props.theme.input.hoverWarningBorderColor) ||
        (props.isDisabled && props.theme.input.hoverDisabledBorderColor) ||
        props.theme.input.hoverBorderColor};
    }
  }

  :active {
    .icon {
      border-color: ${(props) =>
        (props.hasError && props.theme.input.focusErrorBorderColor) ||
        (props.hasWarning && props.theme.input.focusWarningBorderColor) ||
        (props.isDisabled && props.theme.input.focusDisabledBorderColor) ||
        props.theme.input.focusBorderColor};
    }
  }

  .icon {
    display: flex;
    align-items: center;
    justify-content: center;

    position: absolute;
    right: 0;
    background: ${(props) => props.theme.fileInput.icon.background};
    width: ${(props) => widthIconStyle(props)};

    height: ${(props) => heightIconStyle(props)};

    margin: 0;
    border: ${(props) => props.theme.fileInput.icon.border};
    border-radius: ${(props) => props.theme.fileInput.icon.borderRadius};

    border-color: ${(props) =>
      (props.hasError && props.theme.input.errorBorderColor) ||
      (props.hasWarning && props.theme.input.warningBorderColor) ||
      (props.isDisabled && props.theme.input.disabledBorderColor) ||
      props.theme.input.borderColor};
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }

  .icon-button {
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }
`;
StyledFileInput.defaultProps = { theme: Base };

export default StyledFileInput;
