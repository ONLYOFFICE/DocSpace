import React from "react";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import VersionSvg from "@docspace/client/public/images/versionrevision_active.react.svg";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";



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
    <ColorTheme
      type={ThemeType.VersionBadge}
      isVersion={isVersion}
      theme={theme}
      index={index}
    />

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
