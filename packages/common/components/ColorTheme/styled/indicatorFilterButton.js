import styled, { css } from "styled-components";
import StyledIndicator from "@docspace/common/components/FilterInput/sub-components/StyledIndicator";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    background: ${$currentColorScheme.main.accent};

    &:hover {
      background: ${$currentColorScheme.main.accent};
    }
  `;

export default styled(StyledIndicator)(getDefaultStyles);
