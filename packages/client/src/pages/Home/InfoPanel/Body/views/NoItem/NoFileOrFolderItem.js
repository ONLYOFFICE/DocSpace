import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";

import { StyledNoItemContainer } from "../../styles/noItem";

const NoFileOrFolderItem = ({ t, theme }) => {
  const imgSrc = theme.isBase ? EmptyScreenAltSvgUrl : EmptyScreenAltSvgDarkUrl;

  return (
    <StyledNoItemContainer>
      <div className="no-thumbnail-img-wrapper">
        <img size="96px" className="no-thumbnail-img" src={imgSrc} />
      </div>

      <div className="no-item-text">{t("FilesEmptyScreenText")}</div>
    </StyledNoItemContainer>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(observer(NoFileOrFolderItem));
