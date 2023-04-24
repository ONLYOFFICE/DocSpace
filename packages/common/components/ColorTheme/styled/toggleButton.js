import styled, { css } from "styled-components";
import {
  Container,
  ToggleButtonContainer,
} from "@docspace/components/toggle-button/styled-toggle-button";

const getDefaultStyles = ({
  $currentColorScheme,
  isDisabled,
  isChecked,
  theme,
}) =>
  $currentColorScheme &&
  css`
    ${ToggleButtonContainer} {
      svg {
        rect {
          fill: ${isChecked && $currentColorScheme.main.accent} !important;
        }

        circle {
          fill: ${(isChecked && isDisabled && theme.isBase && "#FFFFFF") ||
          (isChecked && $currentColorScheme.text.accent)};
        }
      }
    }
  `;

export default styled(Container)(getDefaultStyles);
