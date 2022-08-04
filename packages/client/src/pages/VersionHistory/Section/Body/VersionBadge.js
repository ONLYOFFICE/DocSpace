import React from "react";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import VersionSvg from "@docspace/client/public/images/versionrevision_active.react.svg";

const StyledVersionSvg = styled(VersionSvg)`
  path {
    fill: ${(props) =>
      !props.isVersion
        ? props.theme.filesVersionHistory.badge.defaultFill
        : props.index === 0
        ? props.theme.filesVersionHistory.badge.fill
        : props.theme.filesVersionHistory.badge.badgeFill};

    stroke: ${(props) =>
      !props.isVersion
        ? props.theme.filesVersionHistory.badge.stroke
        : props.index === 0
        ? props.theme.filesVersionHistory.badge.fill
        : props.theme.filesVersionHistory.badge.badgeFill};

    stroke-dasharray: ${(props) => (props.isVersion ? "2 0" : "3 1")};
    stroke-linejoin: ${(props) => (props.isVersion ? "unset" : "round")};

    ${(props) =>
      props.isVersion &&
      css`
        stroke-width: 2;
      `}
  }
`;

const VersionBadge = ({
  className,
  isVersion,
  versionGroup,
  index,
  t,
  theme,
  ...rest
}) => (
  <Box className={className} marginProp="0 8px" displayProp="flex" {...rest}>
    <StyledVersionSvg isVersion={isVersion} theme={theme} index={index} />
    <Text
      className="version_badge-text"
      color={theme.filesVersionHistory.badge.color}
      isBold
      fontSize="12px"
    >
      {isVersion && t("Version", { version: versionGroup })}
    </Text>
  </Box>
);

export default VersionBadge;
