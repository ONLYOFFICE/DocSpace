import React from "react";
import Text from "@docspace/components/text";
import { StyledVersionSvg } from "@docspace/client/src/pages/VersionHistory/Section/Body/StyledVersionHistory";
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
    <StyledVersionSvg isVersion={isVersion} theme={theme} index={index} />

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
