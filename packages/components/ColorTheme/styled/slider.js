import styled, { css } from "styled-components";
import { StyledSlider } from "@docspace/components/slider/styled-slider";

const getDefaultStyles = ({
  $currentColorScheme,
  withPouring,
  theme,
  isDisabled,
}) =>
  $currentColorScheme &&
  css`
    background-image: ${withPouring &&
    ((theme.isBase &&
      `linear-gradient( ${$currentColorScheme.main.accent}, ${$currentColorScheme.main.accent})`) ||
      (!theme.isBase && `linear-gradient(#FFFFFF, #FFFFFF)`))};

    &::-webkit-slider-thumb {
      background: ${(theme.isBase && $currentColorScheme.main.accent) ||
      (!theme.isBase && "#FFFFFF")};
      box-shadow: ${!theme.isBase &&
      "0px 3px 12px rgba(0, 0, 0, 0.25); !important"};
    }

    &:hover {
      background-image: ${withPouring &&
      ((theme.isBase &&
        `linear-gradient( ${$currentColorScheme.main.accent}, ${$currentColorScheme.main.accent})`) ||
        (!theme.isBase && `linear-gradient(#FFFFFF, #FFFFFF)`))};
    }

    ${isDisabled &&
    css`
      opacity: 0.32;
    `}
  `;

export default styled(StyledSlider)(getDefaultStyles);
