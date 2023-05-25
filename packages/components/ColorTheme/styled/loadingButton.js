import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";
import StyledCircleWrapLoadingButton from "./sub-components/StyledCircleWrapLoadingButton";

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

StyledCircleWrapLoadingButton.defaultProps = {
  theme: Base,
};

export default styled(StyledCircleWrapLoadingButton)(getDefaultStyles);
