import styled, { css } from "styled-components";
import VersionSvg from "../../../../public/images/versionrevision_active.react.svg";

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

export default VersionMarkIcon;
