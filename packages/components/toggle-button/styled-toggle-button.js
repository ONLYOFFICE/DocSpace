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
      props.isChecked
        ? css`
            rect {
              fill: ${props.isDisabled && props.theme.isBase
                ? props.theme.toggleButton.fillColorOff
                : props.theme.toggleButton.fillColorDefault} !important;

              &:hover {
                opacity: ${!props.isDisabled && "0.85"};
              }
            }

            circle {
              fill: ${props.theme.toggleButton.fillCircleColor};
              opacity: ${props.isDisabled && "0.6"};
            }

            opacity: ${props.isDisabled && "0.6"};
          `
        : css`
            rect {
              fill: ${props.theme.toggleButton.fillColorOff};
            }
            circle {
              fill: ${props.theme.toggleButton.fillCircleColorOff};
              opacity: ${props.isDisabled && "0.6"};
            }

            opacity: ${props.isDisabled && "0.6"};

            &:hover {
              rect {
                fill: ${!props.isDisabled &&
                props.theme.toggleButton.hoverFillColorOff};
              }
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
