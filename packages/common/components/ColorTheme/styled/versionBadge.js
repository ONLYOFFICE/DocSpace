import styled, { css } from "styled-components";
import { StyledVersionSvg } from "@docspace/client/src/pages/VersionHistory/Section/Body/StyledVersionHistory";
import Box from "@docspace/components/box";
const getDefaultStyles = ({ $currentColorScheme, $isVersion, theme, index }) =>
  $currentColorScheme &&
  css`
    ${StyledVersionSvg} {
      path {
        fill: ${!$isVersion
          ? theme.filesVersionHistory.badge.defaultFill
          : index === 0
          ? theme.filesVersionHistory.badge.fill
          : $currentColorScheme.accentColor};

        stroke: ${!$isVersion
          ? theme.filesVersionHistory.badge.stroke
          : index === 0
          ? theme.filesVersionHistory.badge.fill
          : $currentColorScheme.accentColor};
      }
    }

    .version_badge-text {
      color: ${$isVersion && index !== 0 && $currentColorScheme.textColor};
    }
  `;

export default styled(Box)(getDefaultStyles);
