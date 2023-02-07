import EmptyScreenAccountsInfoPanelPngUrl from "PUBLIC_DIR/images/empty_screen-accounts-info-panel.png";
import React from "react";

import Text from "@docspace/components/text";
import { StyledNoItemContainer } from "../../styles/noItem";

const NoAccountsItem = ({ t }) => {
  return (
    <StyledNoItemContainer>
      <div className="no-thumbnail-img-wrapper">
        <img src={EmptyScreenAccountsInfoPanelPngUrl} />
      </div>
      <Text className="no-item-text" textAlign="center">
        {t("InfoPanel:AccountsEmptyScreenText")}
      </Text>
    </StyledNoItemContainer>
  );
};

export default NoAccountsItem;
