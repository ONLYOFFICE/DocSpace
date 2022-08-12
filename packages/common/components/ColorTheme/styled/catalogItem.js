import styled, { css } from "styled-components";

import {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemText,
} from "@appserver/components/catalog-item/styled-catalog-item";

const getDefaultStyles = ({ currentColorScheme, isActive }) => css`
  ${StyledCatalogItemText} {
    color: ${isActive && currentColorScheme.accentColor};

    &:hover {
      color: ${isActive && currentColorScheme.accentColor};
    }
  }

  ${StyledCatalogItemImg} {
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

export default styled(StyledCatalogItemContainer)(getDefaultStyles);
