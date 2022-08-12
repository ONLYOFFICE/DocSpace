import styled, { css } from "styled-components";
import { StyledFloatingButton } from "@appserver/components/main-button-mobile/styled-main-button";

const getDefaultStyles = ({ currentColorScheme }) => css`
  background: ${currentColorScheme.accentColor};
  .circle__background {
    background: ${currentColorScheme.accentColor};
  }

  &:hover {
    background: ${currentColorScheme.accentColor};
    .circle__background {
      background: ${currentColorScheme.accentColor};
    }
  }
`;

export default styled(StyledFloatingButton)(getDefaultStyles);
