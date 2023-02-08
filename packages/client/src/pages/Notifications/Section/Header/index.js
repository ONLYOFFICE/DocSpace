import React from "react";
import IconButton from "@docspace/components/icon-button";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import Headline from "@docspace/common/components/Headline";
import { StyledSectionHeader } from "../../StyledComponent";

const SectionHeaderContent = ({ history, t }) => {
  const onClickBack = () => {
    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        "/accounts/view/@self"
      )
    );
  };
  return (
    <StyledSectionHeader>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        isFill={true}
        onClick={onClickBack}
        className="arrow-button"
      />
      <Headline type="content" truncate>
        {t("Notifications")}
      </Headline>
    </StyledSectionHeader>
  );
};

export default SectionHeaderContent;
