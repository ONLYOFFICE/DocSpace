import styled, { css } from "styled-components";
import {
  StyledBadge,
  StyledInner,
} from "@appserver/components/badge/styled-badge";

const getDefaultStyles = ({ currentColorScheme }) => css`
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
