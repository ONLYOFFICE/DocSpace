import styled, { css } from "styled-components";
import { StyledSlider } from "@docspace/components/slider/styled-slider";

const getDefaultStyles = ({ $currentColorScheme, withPouring, theme }) =>
  $currentColorScheme &&
  css`
    background-image: ${withPouring &&
    ((theme.isBase &&
      `linear-gradient( ${$currentColorScheme.accentColor}, ${$currentColorScheme.accentColor})`) ||
      (!theme.isBase && `linear-gradient(#FFFFFF, #FFFFFF)`))};

    &::-webkit-slider-thumb {
      background: ${(theme.isBase && $currentColorScheme.accentColor) ||
      (!theme.isBase && "#FFFFFF")};
      box-shadow: ${!theme.isBase &&
      "0px 3px 12px rgba(0, 0, 0, 0.25); !important"};
    }

    &:hover {
      background-image: ${withPouring &&
      ((theme.isBase &&
        `linear-gradient( ${$currentColorScheme.accentColor}, ${$currentColorScheme.accentColor})`) ||
        (!theme.isBase && `linear-gradient(#FFFFFF, #FFFFFF)`))};
    }
  `;

export default styled(StyledSlider)(getDefaultStyles);
