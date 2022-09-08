import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import { LANGUAGE } from "@docspace/common/constants";
import Text from "@docspace/components/text";
import { ReactSVG } from "react-svg";
import {
  StyledProperties,
  StyledSubtitle,
  StyledGalleryThumbnail,
  StyledTitle,
} from "../../styles/common.js";
import getCorrectDate from "@docspace/components/utils/getCorrectDate";
import { getCookie } from "@docspace/common/utils/index";

const Gallery = ({ t, selection, getIcon, culture, personal }) => {
  const parseAndFormatDate = (date) => {
    const locale = personal ? getCookie(LANGUAGE) : culture;
    const correctDate = getCorrectDate(locale, date);
    return correctDate;
  };

  const src = getIcon(32, ".docxf");
  const thumbnailBlank = getIcon(96, ".docxf");

  const thumbnailUrl =
    selection?.attributes?.template_image?.data.attributes?.formats?.small?.url;

  return (
    <>
      <StyledTitle>
        <ReactSVG className="icon" src={src} />
        <Text className="text">{selection.attributes.name_form}</Text>
      </StyledTitle>

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
            {parseAndFormatDate(selection.attributes.updatedAt)}
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
