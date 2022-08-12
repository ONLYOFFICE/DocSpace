import styled, { css } from "styled-components";
import StyledIndicator from "@docspace/common/components/FilterInput/sub-components/StyledIndicator";

const getDefaultStyles = ({ currentColorScheme }) => css`
  background: ${currentColorScheme.accentColor};

  &:hover {
    background: ${currentColorScheme.accentColor};
  }
`;

export default styled(StyledIndicator)(getDefaultStyles);
