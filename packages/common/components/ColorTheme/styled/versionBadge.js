import styled, { css } from "styled-components";
import Box from "@docspace/components/box";
import VersionMarkIcon from "../../VersionMarkIcon";
const getDefaultStyles = ({ $currentColorScheme, $isVersion, theme, index }) =>
  $currentColorScheme &&
  css`
    ${VersionMarkIcon} {
      path {
        fill: ${!$isVersion
          ? theme.filesVersionHistory.badge.defaultFill
          : theme.filesVersionHistory.badge.fill};

        stroke: ${!$isVersion
          ? theme.filesVersionHistory.badge.stroke
          : theme.filesVersionHistory.badge.fill};
      }
    }

    .version_badge-text {
      color: ${$isVersion && index !== 0 && $currentColorScheme.text.accent};
    }
  `;

export default styled(Box)(getDefaultStyles);
