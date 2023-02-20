import EmptyScreenAccountsInfoPanelPngUrl from "PUBLIC_DIR/images/empty_screen-accounts-info-panel.png";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";

import React from "react";
import { inject, observer } from "mobx-react";
import { StyledSeveralItemsContainer } from "../../styles/severalItems";

const SeveralItems = ({ isAccounts, theme }) => {
  const emptyScreenAlt = theme.isBase
    ? EmptyScreenAltSvgUrl
    : EmptyScreenAltSvgDarkUrl;

  const imgSrc = isAccounts
    ? EmptyScreenAccountsInfoPanelPngUrl
    : emptyScreenAlt;

  return (
    <StyledSeveralItemsContainer className="no-thumbnail-img-wrapper">
      <img size="96px" src={imgSrc} />
    </StyledSeveralItemsContainer>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(observer(SeveralItems));
