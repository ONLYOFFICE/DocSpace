import styled, { css } from "styled-components";
import StyledWrapper from "@docspace/client/src/components/IndicatorLoader/StyledWrapper";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    #ipl-progress-indicator {
      background-color: ${$currentColorScheme.main.accent};

      &:hover {
        background-color: ${$currentColorScheme.main.accent};
      }
    }
  `;

export default styled(StyledWrapper)(getDefaultStyles);
