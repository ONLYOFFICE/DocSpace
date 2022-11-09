import styled, { css } from "styled-components";
import {
  StyledBadge,
  StyledInner,
  StyledText,
} from "@docspace/components/badge/styled-badge";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({
  $currentColorScheme,
  isVersionBadge,
  backgroundColor,
  color,
  theme,
}) =>
  $currentColorScheme &&
  !isVersionBadge &&
  css`
    ${StyledText} {
      color: ${color
        ? color
        : $currentColorScheme.id === 7 && !theme.isBase
        ? "#444444"
        : $currentColorScheme.textColor} !important;
    }

    ${StyledInner} {
      background-color: ${backgroundColor
        ? backgroundColor
        : $currentColorScheme.id === 7 && !theme.isBase
        ? "#ECEEF1"
        : $currentColorScheme.accentColor};

      &:hover {
        background-color: ${backgroundColor
          ? backgroundColor
          : $currentColorScheme.id === 7 && !theme.isBase
          ? "#ECEEF1"
          : $currentColorScheme.accentColor};
      }
    }

    &:hover {
      border-color: ${backgroundColor
        ? backgroundColor
        : $currentColorScheme.id === 7 && !theme.isBase
        ? "#ECEEF1"
        : $currentColorScheme.accentColor};
    }
  `;

StyledBadge.defaultProps = { theme: Base };

export default styled(StyledBadge)(getDefaultStyles);
