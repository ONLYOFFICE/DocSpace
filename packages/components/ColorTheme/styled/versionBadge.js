import styled, { css } from "styled-components";
import Box from "@docspace/components/box";
import VersionSvg from "PUBLIC_DIR/images/versionrevision_active.react.svg";

const VersionMarkIcon = styled(VersionSvg)`
  path {
    fill: ${(props) =>
      !props.$isVersion
        ? props.theme.filesVersionHistory.badge.defaultFill
        : props.index === 0
        ? props.theme.filesVersionHistory.badge.fill
        : props.theme.filesVersionHistory.badge.badgeFill};
    stroke: ${(props) =>
      !props.$isVersion
        ? props.theme.filesVersionHistory.badge.stroke
        : props.index === 0
        ? props.theme.filesVersionHistory.badge.fill
        : props.theme.filesVersionHistory.badge.badgeFill};

    stroke-dasharray: ${(props) => (props.$isVersion ? "2 0" : "3 1")};
    stroke-linejoin: ${(props) => (props.$isVersion ? "unset" : "round")};

    ${(props) =>
      props.$isVersion &&
      css`
        stroke-width: 2;
      `}
  }
`;
const getDefaultStyles = ({ $currentColorScheme, $isVersion, theme, index }) =>
  $currentColorScheme &&
  css`
    ${VersionMarkIcon} {
      path {
        fill: ${!$isVersion
          ? theme.filesVersionHistory.badge.defaultFill
          : index === 0
          ? theme.filesVersionHistory.badge.fill
          : $currentColorScheme.main.accent};

        stroke: ${!$isVersion
          ? theme.filesVersionHistory.badge.stroke
          : index === 0
          ? theme.filesVersionHistory.badge.fill
          : $currentColorScheme.main.accent};
      }
    }

    .version_badge-text {
      color: ${$isVersion && index !== 0 && $currentColorScheme.text.accent};
    }
  `;

export default styled(Box)(getDefaultStyles);
