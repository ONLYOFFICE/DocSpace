import React from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders/index.js";

import Text from "@docspace/components/text";

import { parseAndFormatDate } from "../../helpers/DetailsHelper.js";
import { StyledGalleryThumbnail } from "../../styles/gallery.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";

const Gallery = ({ t, gallerySelected, getIcon, culture, personal }) => {
  const thumbnailBlank = getIcon(96, ".docxf");
  const thumbnailUrl =
    gallerySelected?.attributes?.template_image?.data.attributes?.formats?.small
      ?.url;

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
          <Text className="property-title">{t("InfoPanel:DateModified")}</Text>
          <Text className="property-content">
            {parseAndFormatDate(
              gallerySelected.attributes.updatedAt,
              personal,
              culture
            )}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Size")}</Text>
          <Text className="property-content">
            {gallerySelected.attributes.file_size}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Pages")}</Text>
          <Text className="property-content">
            {gallerySelected.attributes.file_pages}
          </Text>
        </div>
      </StyledProperties>
    </>
  );
};

export default withTranslation([
  "InfoPanel",
  "FormGallery",
  "Common",
  "Translations",
])(withLoader(Gallery)(<Loaders.InfoPanelViewLoader view="gallery" />));
