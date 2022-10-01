import React from "react";

import { StyledNoItemContainer } from "../../styles/noItem";

const NoFileOrFolderItem = ({ t }) => {
  return (
    <StyledNoItemContainer>
      <div className="no-thumbnail-img-wrapper">
        <img
          size="96px"
          className="no-thumbnail-img"
          src="images/empty_screen_info_panel.png"
        />
      </div>

      <div className="no-item-text">{t("FilesEmptyScreenTent")}</div>
    </StyledNoItemContainer>
  );
};

export default NoFileOrFolderItem;
