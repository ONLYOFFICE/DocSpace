import styled, { css } from "styled-components";

import StyledText from "@appserver/components/text/styled-text";

const getDefaultStyles = ({ currentColorScheme, isActive }) => css`
  color: ${isActive && currentColorScheme.accentColor};

  &:hover {
    color: ${isActive && currentColorScheme.accentColor};
  }
`;

export default styled(StyledText)(getDefaultStyles);
