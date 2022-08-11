import React from "react";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import { ColorTheme, ThemeType } from "@appserver/common/components/ColorTheme";

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
