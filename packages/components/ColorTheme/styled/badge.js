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
  isPaidBadge,
  isMutedBadge,
}) =>
  $currentColorScheme &&
  !isVersionBadge &&
  css`
    ${StyledText} {
      color: ${color
        ? color
        : isPaidBadge
        ? theme.badge.color
        : $currentColorScheme.text.accent} !important;
    }

    ${StyledInner} {
      background-color: ${isMutedBadge
        ? theme.badge.disableBackgroundColor
        : backgroundColor
        ? backgroundColor
        : $currentColorScheme.main.accent};

      &:hover {
        background-color: ${isMutedBadge
          ? theme.badge.disableBackgroundColor
          : backgroundColor
          ? backgroundColor
          : $currentColorScheme.main.accent};
      }
    }

    &:hover {
      border-color: ${isMutedBadge
        ? theme.badge.disableBackgroundColor
        : backgroundColor
        ? backgroundColor
        : $currentColorScheme.main.accent};
    }
  `;

StyledBadge.defaultProps = { theme: Base };

export default styled(StyledBadge)(getDefaultStyles);
