import styled, { css } from "styled-components";
import {
  StyledCircleWrap,
  StyledFloatingButton,
  IconBox,
} from "@docspace/common/components/FloatingButton/StyledFloatingButton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, color, icon, theme }) =>
  $currentColorScheme &&
  css`
    background: ${color
      ? color
      : theme.isBase
      ? $currentColorScheme.accentColor
      : icon === "upload"
      ? theme.floatingButton.backgroundColor
      : $currentColorScheme.accentColor} !important;

    ${StyledFloatingButton} {
      background: ${color
        ? color
        : theme.isBase
        ? $currentColorScheme.accentColor
        : icon === "upload"
        ? theme.floatingButton.backgroundColor
        : $currentColorScheme.accentColor} !important;
    }

    ${IconBox} {
      svg {
        path {
          fill: ${$currentColorScheme.textColor};
        }
      }
    }
  `;

StyledCircleWrap.defaultProps = { theme: Base };

export default styled(StyledCircleWrap)(getDefaultStyles);
