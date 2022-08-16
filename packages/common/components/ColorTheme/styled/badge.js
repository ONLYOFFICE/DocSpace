import styled, { css } from "styled-components";
import {
  StyledBadge,
  StyledInner,
} from "@docspace/components/badge/styled-badge";

const getDefaultStyles = ({ currentColorScheme }) =>
  currentColorScheme &&
  css`
    ${StyledInner} {
      background-color: ${currentColorScheme.accentColor};

      &:hover {
        background-color: ${currentColorScheme.accentColor};
      }
    }

    &:hover {
      border-color: ${currentColorScheme.accentColor};
    }
  `;

export default styled(StyledBadge)(getDefaultStyles);
