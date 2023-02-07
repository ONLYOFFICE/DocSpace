import EmptyScreenAccountsInfoPanelPngUrl from "PUBLIC_DIR/images/empty_screen-accounts-info-panel.png";
import EmptyScreenInfoPanelPngUrl from "PUBLIC_DIR/images/empty_screen_info_panel.png";
import React from "react";
import { StyledSeveralItemsContainer } from "../../styles/severalItems";

const SeveralItems = ({ isAccounts }) => {
  const imgSrc = isAccounts
    ? EmptyScreenAccountsInfoPanelPngUrl
    : EmptyScreenInfoPanelPngUrl;

  return (
    <StyledSeveralItemsContainer className="no-thumbnail-img-wrapper">
      <img size="96px" src={imgSrc} />
    </StyledSeveralItemsContainer>
  );
};

export default SeveralItems;
