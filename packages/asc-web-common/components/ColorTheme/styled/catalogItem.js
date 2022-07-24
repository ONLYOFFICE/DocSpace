import styled, { css } from "styled-components";

import { StyledCatalogItemContainer } from "@appserver/components/catalog-item/styled-catalog-item";

const getDefaultStyles = ({ currentColorScheme, isActive }) => css`
  .color-theme-text {
    color: ${isActive && currentColorScheme.accentColor};

    &:hover {
      color: ${isActive && currentColorScheme.accentColor};
    }
  }

  .color-theme-img {
    svg {
      path {
        fill: ${isActive && currentColorScheme.accentColor} !important;
      }
    }

    &:hover {
      svg {
        path {
          fill: ${isActive && currentColorScheme.accentColor} !important;
        }
      }
    }
  }
`;

export default styled(StyledCatalogItemContainer)([getDefaultStyles]);
