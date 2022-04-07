import React from "react";
import Text from "@appserver/components/text";
import HelpButton from "@appserver/components/help-button";
import { Base } from "@appserver/components/themes";
import { StyledCategoryWrapper } from "../StyledSecurity";

const CategoryWrapper = (props) => {
  const { title, tooltipContent, theme } = props;

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
        tooltipContent={<Text>{tooltipContent}</Text>}
        tooltipColor={theme.studio.settings.security.owner.tooltipColor}
      />
    </StyledCategoryWrapper>
  );
};

CategoryWrapper.defaultProps = {
  theme: Base,
};

export default CategoryWrapper;
