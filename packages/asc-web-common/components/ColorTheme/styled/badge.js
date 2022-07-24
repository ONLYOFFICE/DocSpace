import styled, { css } from "styled-components";
import { StyledBadge } from "@appserver/components/badge/styled-badge";

const getDefaultStyles = ({ currentColorScheme }) => css`
  .color-theme-background {
    background-color: ${currentColorScheme.accentColor};
  }

  &:hover {
    .color-theme-background {
      background-color: ${currentColorScheme.accentColor};
    }
    border-color: ${currentColorScheme.accentColor};
  }
`;

export default styled(StyledBadge)([getDefaultStyles]);
