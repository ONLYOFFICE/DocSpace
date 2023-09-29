import InfoPanelRoomEmptyScreenSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate.svg?url";
import InfoPanelRoomEmptyScreenDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate_dark.svg?url";

import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";

import { StyledNoItemContainer } from "../../styles/noItem";

const NoRoomItem = ({ t, theme, setMembersList }) => {
  const imageSrc = theme.isBase
    ? InfoPanelRoomEmptyScreenSvgUrl
    : InfoPanelRoomEmptyScreenDarkSvgUrl;

  useEffect(() => {
    setMembersList(null);
  }, []);

  return (
    <StyledNoItemContainer className="info-panel_gallery-empty-screen">
      <div className="no-thumbnail-img-wrapper">
        <img src={imageSrc} alt="No Room Image" />
      </div>
      <Text className="no-item-text" textAlign="center">
        {t("RoomsEmptyScreenTent")}
      </Text>
    </StyledNoItemContainer>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
    setMembersList: auth.infoPanelStore.setMembersList,
  };
})(observer(NoRoomItem));
