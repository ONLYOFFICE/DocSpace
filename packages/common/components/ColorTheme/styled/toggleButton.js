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
          fill: ${isChecked && $currentColorScheme.main.accent};
        }

        circle {
          fill: ${(isChecked && isDisabled && theme.isBase && "#FFFFFF") ||
          (isChecked &&
            $currentColorScheme.id > 7 &&
            $currentColorScheme.textColor)};
        }
      }
    }
  `;

export default styled(Container)(getDefaultStyles);
