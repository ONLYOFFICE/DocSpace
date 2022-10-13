import styled, { css } from "styled-components";

import {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemText,
} from "@docspace/components/catalog-item/styled-catalog-item";

import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isActive, theme }) =>
  $currentColorScheme &&
  css`
    ${StyledCatalogItemText} {
      color: ${isActive && theme.isBase && $currentColorScheme.accentColor};

      &:hover {
        color: ${isActive && theme.isBase && $currentColorScheme.accentColor};
      }
    }

    ${StyledCatalogItemImg} {
      svg {
        path {
          fill: ${isActive &&
          theme.isBase &&
          $currentColorScheme.accentColor} !important;
        }
        circle {
          fill: ${isActive &&
          theme.isBase &&
          $currentColorScheme.accentColor} !important;
        }
      }

      &:hover {
        svg {
          path {
            fill: ${isActive &&
            theme.isBase &&
            $currentColorScheme.accentColor} !important;
          }
        }
      }
    }
  `;

StyledCatalogItemContainer.defaultProps = { theme: Base };

export default styled(StyledCatalogItemContainer)(getDefaultStyles);
