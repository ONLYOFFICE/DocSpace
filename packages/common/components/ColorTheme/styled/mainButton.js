import styled, { css } from "styled-components";
import { StyledMainButton } from "@docspace/components/main-button/styled-main-button";

const getDefaultStyles = ({ currentColorScheme, isDisabled }) => css`
  background: ${currentColorScheme.accentColor};

  &:hover {
    background: ${currentColorScheme.accentColor};
    cursor: pointer;
  }

  &:active {
    background: ${currentColorScheme.accentColor};
    opacity: ${!isDisabled && "0.6"};
    cursor: pointer;
  }
`;

export default styled(StyledMainButton)(getDefaultStyles);
