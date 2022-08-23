import React from "react";
import { withTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";

import { StyledTitle } from "./StyledBody";

const SeveralItems = ({ t, count }) => {
  return (
    <>
      <StyledTitle>
        <Avatar size={"min"} />
        <Text className="text" fontWeight={600} fontSize="16px">
          {`${t("SelectedUsers")}: ${count}`}
        </Text>
      </StyledTitle>

      <div className="no-thumbnail-img-wrapper several-items-image">
        <img
          size="96px"
          className="no-thumbnail-img"
          src="/static/images/empty_screen-accounts-info-panel.png"
        />
      </div>
    </>
  );
};

export default withTranslation(["InfoPanel"])(SeveralItems);
