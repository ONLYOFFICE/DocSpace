import React from "react";
import Text from "@docspace/components/text";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import VersionMarkIcon from "@docspace/common/components/VersionMarkIcon";
const VersionBadge = ({
  className,
  isVersion,
  versionGroup,
  index,
  t,
  theme,
  ...rest
}) => (
  <ColorTheme
    themeId={ThemeType.VersionBadge}
    className={className}
    marginProp="0 8px"
    displayProp="flex"
    isVersion={isVersion}
    theme={theme}
    index={index}
    {...rest}
  >
    <VersionMarkIcon $isVersion={isVersion} theme={theme} index={index} />

    <Text
      className="version_badge-text"
      color={theme.filesVersionHistory.badge.color}
      isBold
      fontSize="12px"
    >
      {isVersion && t("Version", { version: versionGroup })}
    </Text>
  </ColorTheme>
);

export default VersionBadge;
