import React from "react";
import Text from "@docspace/components/text";

import { StyledNoItemContainer } from "../../styles/noItem";

const NoRoomItem = ({ t }) => {
  return (
    <StyledNoItemContainer className="info-panel_gallery-empty-screen">
      <img src="images/info-panel-room-empty-screen.svg" alt="No Room Image" />
      <Text className="no-item-text" textAlign="center">
        {t("See rooms details here")}
      </Text>
    </StyledNoItemContainer>
  );
};
export default NoRoomItem;
