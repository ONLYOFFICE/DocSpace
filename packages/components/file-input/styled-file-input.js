import styled, { css } from "styled-components";
import Base from "../themes/base";

const paddingRightStyle = (props) =>
  props.theme.fileInput.paddingRight[props.size];

const widthIconStyle = (props) => props.theme.fileInput.icon.width[props.size];
const heightIconStyle = (props) =>
  props.theme.fileInput.icon.height[props.size];
const widthIconButtonStyle = (props) =>
  props.theme.fileInput.iconButton.width[props.size];

const StyledFileInput = styled.div`
  display: flex;
  position: relative;
  outline: none;
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

    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? `padding-left: ${paddingRightStyle(props) || "40px"};`
        : `padding-right: ${paddingRightStyle(props) || "40px"};`}

    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
    margin: 0;
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

    width: ${(props) => widthIconStyle(props)};

    height: ${(props) => heightIconStyle(props)};

    margin: 0;
    border: ${(props) => props.theme.fileInput.icon.border};

    border-color: ${(props) =>
      (props.hasError && props.theme.input.errorBorderColor) ||
      (props.hasWarning && props.theme.input.warningBorderColor) ||
      (props.isDisabled && props.theme.input.disabledBorderColor) ||
      props.theme.input.borderColor};
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};

    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            left: 0;
            border-radius: ${props.theme.fileInput.icon.borderRadiusRtl};

            & svg {
              transform: scaleX(-1);
            }
          `
        : css`
            right: 0;
            border-radius: ${props.theme.fileInput.icon.borderRadiusLtr};
          `}
  }

  .icon-button {
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
    width: ${(props) => widthIconButtonStyle(props)};
  }
`;
StyledFileInput.defaultProps = {
  theme: { ...Base, interfaceDirection: "ltr" },
};

export default StyledFileInput;
