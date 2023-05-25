import styled, { css } from "styled-components";
import { StyledCircleWrap } from "./sub-components/StyledLoadingButton";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    .circle__mask .circle__fill {
      background-color: ${$currentColorScheme.main.accent} !important;
    }

    .loading-button {
      color: ${$currentColorScheme.main.accent};
    }
  `;

export default styled(StyledCircleWrap)(getDefaultStyles);
