import React from "react";

import Text from "@docspace/components/text";
import { StyledNoItemContainer } from "../../styles/noItem";

const NoAccountsItem = ({ t }) => {
  return (
    <StyledNoItemContainer>
      <img src="/static/images/empty_screen-accounts-info-panel.png" />
      <Text className="no-item-text" textAlign="center">
        {t("InfoPanel:AccountsEmptyScreenText")}
      </Text>
    </StyledNoItemContainer>
  );
};

export default NoAccountsItem;
