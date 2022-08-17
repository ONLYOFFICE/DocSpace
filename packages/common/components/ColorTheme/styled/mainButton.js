import styled, { css } from "styled-components";
import { StyledMainButton } from "@docspace/components/main-button/styled-main-button";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, isDisabled, theme }) =>
  currentColorScheme &&
  css`
    background-color: ${currentColorScheme.accentColor};

    &:hover {
      background-color: ${currentColorScheme.accentColor};
      opacity: ${!isDisabled && "0.85"};
      cursor: pointer;
    }

    &:active {
      background-color: ${currentColorScheme.accentColor};
      opacity: ${!isDisabled && "1"};
      filter: ${theme.isBase ? "brightness(90%)" : "brightness(82%)"};
      cursor: pointer;
    }
  `;

StyledMainButton.defaultProps = { theme: Base };

export default styled(StyledMainButton)(getDefaultStyles);
