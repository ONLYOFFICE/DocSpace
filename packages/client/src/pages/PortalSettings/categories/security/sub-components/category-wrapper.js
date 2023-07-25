import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";
import React from "react";
import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";
import Link from "@docspace/components/link";
import { Base } from "@docspace/components/themes";
import { StyledCategoryWrapper, StyledTooltip } from "../StyledSecurity";
import { useTheme } from "styled-components";

const CategoryWrapper = props => {
  const {
    t,
    title,
    tooltipTitle,
    tooltipUrl,
    theme,
    currentColorScheme,
    classNameTooltip,
  } = props;
  const { interfaceDirection } = useTheme();
  const dirTooltip = interfaceDirection === "rtl" ? "left" : "right";
  const tooltip = () => (
    <StyledTooltip>
      <Text className={tooltipUrl ? "subtitle" : ""} fontSize="12px">
        {tooltipTitle}
      </Text>
      {tooltipUrl && (
        <Link
          fontSize="13px"
          target="_blank"
          isBold
          isHovered
          href={tooltipUrl}
          color={currentColorScheme.main.accent}>
          {t("Common:LearnMore")}
        </Link>
      )}
    </StyledTooltip>
  );

  return (
    <StyledCategoryWrapper>
      <Text fontSize="16px" fontWeight="700">
        {title}
      </Text>
      <HelpButton
        className={classNameTooltip}
        iconName={InfoReactSvgUrl}
        displayType="dropdown"
        place={dirTooltip}
        offsetRight={0}
        getContent={tooltip}
        tooltipColor={theme.client.settings.security.owner.tooltipColor}
      />
    </StyledCategoryWrapper>
  );
};

CategoryWrapper.defaultProps = {
  theme: Base,
};

export default CategoryWrapper;
