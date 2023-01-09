import styled, { css } from "styled-components";
import { StyledMainButton } from "@docspace/components/main-button/styled-main-button";
import Base from "@docspace/components/themes/base";

const disableStyles = css`
  opacity: 0.6;

  &:hover {
    opacity: 0.6;
    cursor: default;
  }

  &:active {
    opacity: 0.6;
    cursor: default;
    filter: none;
  }
`;

const getDefaultStyles = ({ $currentColorScheme, isDisabled, theme }) =>
  $currentColorScheme &&
  css`
    background-color: ${$currentColorScheme.main.accent};

    &:hover {
      background-color: ${$currentColorScheme.main.accent};
      opacity: 0.85;
      cursor: pointer;
    }

    &:active {
      background-color: ${$currentColorScheme.main.accent};
      opacity: 1;
      filter: ${theme.isBase ? "brightness(90%)" : "brightness(82%)"};
      cursor: pointer;
    }

    .main-button_text {
      color: ${$currentColorScheme.text.accent};
    }

    .main-button_img svg path {
      fill: ${$currentColorScheme.text.accent};
    }

    ${isDisabled &&
    `
    ${disableStyles}
    `}
  `;

StyledMainButton.defaultProps = { theme: Base };

export default styled(StyledMainButton)(getDefaultStyles);
