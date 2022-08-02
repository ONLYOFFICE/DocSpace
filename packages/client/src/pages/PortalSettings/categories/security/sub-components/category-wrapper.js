import React from "react";
import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";
import Link from "@docspace/components/link";
import { Base } from "@docspace/components/themes";
import { StyledCategoryWrapper, StyledTooltip } from "../StyledSecurity";

const CategoryWrapper = (props) => {
  const { t, title, tooltipTitle, tooltipUrl, theme } = props;

  const tooltip = () => (
    <StyledTooltip>
      <Text className={tooltipUrl ? "subtitle" : ""}>{tooltipTitle}</Text>
      {tooltipUrl && (
        <Link target="_blank" isHovered href={tooltipUrl}>
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
        iconName="/static/images/info.react.svg"
        displayType="dropdown"
        place="right"
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
