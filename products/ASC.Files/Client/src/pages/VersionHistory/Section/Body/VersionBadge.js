import React from "react";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
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
    <svg
      width="55"
      height="18"
      viewBox="0 0 55 18"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      stroke={isVersion ? "none" : theme.filesVersionHistory.badge.stroke}
      strokeDasharray={isVersion ? "none" : "2px"}
      strokeWidth={isVersion ? "none" : "2px"}
    >
      <path
        fillRule="evenodd"
        clipRule="evenodd"
        d="M0 1C0 0.447716 0.447715 0 1 0L53.9994 0C54.6787 0 55.1603 0.662806 54.9505 1.3089L52.5529 8.6911C52.4877 8.89187 52.4877 9.10813 52.5529 9.3089L54.9505 16.6911C55.1603 17.3372 54.6787 18 53.9994 18H0.999999C0.447714 18 0 17.5523 0 17V1Z"
        fill={
          !isVersion
            ? theme.filesVersionHistory.badge.defaultFill
            : index === 0
            ? theme.filesVersionHistory.badge.fill
            : theme.filesVersionHistory.badge.badgeFill
        }
      />
    </svg>
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
