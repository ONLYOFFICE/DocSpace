import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import { StyledCircleWrap } from "../../floating-button/styled-floating-button";

const getDefaultStyles = ({ $currentColorScheme, color, displayProgress }) =>
  $currentColorScheme &&
  css`
    background: ${color || $currentColorScheme.main.accent} !important;

    .circle__background {
      background: ${color || $currentColorScheme.main.accent} !important;
    }

    .icon-box {
      svg {
        path {
          fill: ${$currentColorScheme.text.accent};
        }
      }
    }

    .circle__mask .circle__fill {
      background-color: ${!displayProgress
        ? "transparent !important"
        : $currentColorScheme.text.accent};
    }
  `;

StyledCircleWrap.defaultProps = { theme: Base };

export default styled(StyledCircleWrap)(getDefaultStyles);
