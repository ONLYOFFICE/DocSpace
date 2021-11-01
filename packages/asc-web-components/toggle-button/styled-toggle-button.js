import styled, { css } from "styled-components";

import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";

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
              fill: ${props.theme.toggleButton.disableFillColorOff};
            }
          `
        : ""}
  }

  .toggle-button {
    min-width: 28px;
  }

  .toggle-button-text {
    margin-top: 2px;

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

export { ToggleButtonContainer, HiddenInput };
