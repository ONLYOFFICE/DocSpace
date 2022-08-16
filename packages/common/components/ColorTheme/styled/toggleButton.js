import styled, { css } from "styled-components";
import {
  Container,
  ToggleButtonContainer,
} from "@docspace/components/toggle-button/styled-toggle-button";

const getDefaultStyles = ({ currentColorScheme, isDisabled, isChecked }) =>
  currentColorScheme &&
  css`
    ${ToggleButtonContainer} {
      svg {
        rect {
          fill: ${isChecked && currentColorScheme.accentColor};
          opacity: ${isDisabled && "0.6"};

          &:hover {
            fill: ${isChecked && currentColorScheme.accentColor};
            opacity: ${isDisabled && "0.6"};
          }
        }
      }
    }
  `;

export default styled(Container)(getDefaultStyles);
