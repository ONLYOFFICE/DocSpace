import React from "react";

import { useNavigate, useLocation } from "react-router-dom";
import IconButton from "@docspace/components/icon-button";

import Headline from "@docspace/common/components/Headline";

import { StyledSectionHeader } from "../../StyledComponent";

const SectionHeaderContent = ({ t }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const onClickBack = () => {
    const url = location.pathname.replace("/notification", "");

    navigate(url);
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
