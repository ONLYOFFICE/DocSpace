import styled, { css } from "styled-components";
import {
  StyledCircleWrap,
  StyledFloatingButton,
  IconBox,
  StyledCircle,
} from "@docspace/common/components/FloatingButton/StyledFloatingButton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({
  $currentColorScheme,
  color,
  icon,
  theme,
  displayProgress,
}) =>
  $currentColorScheme &&
  css`
    background: ${color ? color : $currentColorScheme.accentColor} !important;

    ${StyledFloatingButton} {
      background: ${color ? color : $currentColorScheme.accentColor} !important;
    }

    ${IconBox} {
      svg {
        path {
          fill: ${$currentColorScheme.id > 7 && $currentColorScheme.textColor};
        }
      }
    }

    ${StyledCircle} {
      .circle__mask .circle__fill {
        background-color: ${!displayProgress
          ? "transparent !important"
          : $currentColorScheme.id > 7 && $currentColorScheme.textColor};
      }
    }
  `;

StyledCircleWrap.defaultProps = { theme: Base };

export default styled(StyledCircleWrap)(getDefaultStyles);
