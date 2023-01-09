import React from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";
import { StyledTitle } from "../../styles/common";

const GalleryItemTitle = ({ selection, getIcon }) => {
  return (
    <StyledTitle>
      <ReactSVG className="icon" src={getIcon(32, ".docxf")} />
      <Text className="text">{selection?.attributes?.name_form}</Text>
    </StyledTitle>
  );
};

export default withTranslation([])(GalleryItemTitle);
