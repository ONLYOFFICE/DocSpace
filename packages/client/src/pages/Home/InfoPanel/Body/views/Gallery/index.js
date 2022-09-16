import React from "react";
import Text from "@docspace/components/text";
import { ReactSVG } from "react-svg";

import { parseAndFormatDate } from "../../helpers/DetailsHelper.js";
import { StyledGalleryThumbnail } from "../../styles/gallery.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";

const Gallery = ({ t, selection, getIcon, culture, personal }) => {
  const thumbnailBlank = getIcon(96, ".docxf");
  const thumbnailUrl =
    selection?.attributes?.template_image?.data.attributes?.formats?.small?.url;

  return (
    <>
      {thumbnailUrl ? (
        <StyledGalleryThumbnail>
          <img className="info-panel_gallery-img" src={thumbnailUrl} alt="" />
        </StyledGalleryThumbnail>
      ) : (
        <div className="no-thumbnail-img-wrapper">
          <ReactSVG className="no-thumbnail-img" src={thumbnailBlank} />
        </div>
      )}

      <StyledSubtitle>
        <Text fontWeight="600" fontSize="14px">
          {t("SystemProperties")}
        </Text>
      </StyledSubtitle>

      <StyledProperties>
        <div className="property">
          <Text className="property-title">
            {t("Files:ByLastModifiedDate")}
          </Text>
          <Text className="property-content">
            {parseAndFormatDate(
              selection.attributes.updatedAt,
              personal,
              culture
            )}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Size")}</Text>
          <Text className="property-content">
            {selection.attributes.file_size}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Pages")}</Text>
          <Text className="property-content">
            {selection.attributes.file_pages}
          </Text>
        </div>
      </StyledProperties>
    </>
  );
};

export default Gallery;
