import styled, { css } from "styled-components";

import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";

const Container = styled.div`
  display: inline-block;
`;

const ToggleButtonContainer = styled.label`
  position: absolute;
  -webkit-appearance: none;
  align-items: start;
  outline: none;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${NoUserSelect};

  display: grid;
  grid-template-columns: min-content auto;
  grid-gap: 8px;

  ${(props) =>
    props.isDisabled
      ? css`
          cursor: default;
        `
      : css`
          cursor: pointer;
        `}

  svg {
    ${(props) =>
      props.isDisabled
        ? css`
            rect {
              fill: ${props.isChecked
                ? props.theme.toggleButton.disableFillColor
                : props.theme.toggleButton.disableFillColorOff};
              stroke: ${props.isChecked
                ? props.theme.toggleButton.disableBorderColor
                : props.theme.toggleButton.disableBorderColorOff};
              stroke-width: 1px;
              stroke-linecap: round;
            }
            circle {
              fill: ${props.isChecked
                ? props.theme.toggleButton.disableFillCircleColor
                : props.theme.toggleButton.disableFillCircleColorOff};
            }
          `
        : css`
            rect {
              fill: ${props.isChecked
                ? props.theme.toggleButton.fillColor
                : props.theme.toggleButton.fillColorOff};
              stroke: ${props.isChecked
                ? props.theme.toggleButton.borderColor
                : props.theme.toggleButton.borderColorOff};
              stroke-width: 1px;
            }
            circle {
              fill: ${props.isChecked
                ? props.theme.toggleButton.fillCircleColor
                : props.theme.toggleButton.fillCircleColorOff};
            }
          `}
  }

  .toggle-button {
    min-width: 28px;
  }

  .toggle-button-text {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.text.disableColor
        : props.theme.text.color};
  }
`;

ToggleButtonContainer.defaultProps = { theme: Base };

const HiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
  z-index: -1;
`;

export { ToggleButtonContainer, HiddenInput, Container };
