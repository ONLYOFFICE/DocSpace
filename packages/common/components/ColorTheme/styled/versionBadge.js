import styled, { css } from "styled-components";
import { StyledVersionSvg } from "@docspace/client/src/pages/VersionHistory/Section/Body/StyledVersionHistory";
const getDefaultStyles = ({ $currentColorScheme, $isVersion, theme, index }) =>
  $currentColorScheme &&
  css`
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
  `;

export default styled(StyledVersionSvg)(getDefaultStyles);
