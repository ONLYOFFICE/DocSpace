import EmptyScreenRecentUrl from "PUBLIC_DIR/images/empty_screen_recent.svg?url";
import EmptyScreenRecentDarkUrl from "PUBLIC_DIR/images/empty_screen_recent_dark.svg?url";

import React from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";

import { StyledNoItemContainer } from "../../styles/noItem";

const NoHistory = ({ t, theme }) => {
  const imageSrc = theme.isBase
    ? EmptyScreenRecentUrl
    : EmptyScreenRecentDarkUrl;

  return (
    <StyledNoItemContainer className="info-panel_gallery-empty-screen">
      <div className="no-thumbnail-img-wrapper">
        <img src={imageSrc} alt="No History Image" />
      </div>
      <Text className="no-history-text" textAlign="center">
        {t("HistoryEmptyScreenText")}
      </Text>
    </StyledNoItemContainer>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(observer(NoHistory));
