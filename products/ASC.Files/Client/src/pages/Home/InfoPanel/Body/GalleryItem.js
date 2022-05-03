import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import { LANGUAGE } from "@appserver/common/constants";
import Text from "@appserver/components/text";
import { ReactSVG } from "react-svg";
import {
  StyledProperties,
  StyledSubtitle,
  StyledGalleryThumbnail,
  StyledTitle,
} from "./styles/styles.js";
import moment from "moment";

const SingleItem = (props) => {
  const { t, selectedItem, getIcon } = props;

  const parseAndFormatDate = (date) => {
    return moment(date)
      .locale(localStorage.getItem(LANGUAGE))
      .format("DD.MM.YY hh:mm A");
  };

  const src = getIcon(32, ".docxf");
  const thumbnailBlank = getIcon(96, ".docxf");

  const thumbnailUrl =
    selectedItem.attributes.template_image.data.attributes.formats.small.url;

  return (
    <>
      <StyledTitle>
        <ReactSVG className="icon" src={src} />
        <Text className="text">{selectedItem.attributes.name_form}</Text>
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
          <Text className="property-title">{t("Home:ByLastModifiedDate")}</Text>
          <Text className="property-content">
            {parseAndFormatDate(selectedItem.updatedAt)}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Size")}</Text>
          <Text className="property-content">
            {selectedItem.attributes.file_size}
          </Text>
        </div>
        <div className="property">
          <Text className="property-title">{t("Common:Pages")}</Text>
          <Text className="property-content">
            {selectedItem.attributes.file_pages}
          </Text>
        </div>
        <Text
          as="div"
          fontSize="12px"
          fontWeight={400}
          className="oforms-description"
        >
          <Trans t={t} i18nKey="OFORMsDescription" ns="InfoPanel">
            Fill out the form online and get a simple Design Project Proposal
            ready, or just download the fillable template in the desirable
            format: DOCXF, OFORM, or PDF.
            <p className="oforms-description-text">
              Propose a project or a series of projects to an freelance designer
              team. Outline project and task structure, payments, and terms.
            </p>
          </Trans>
        </Text>
      </StyledProperties>
    </>
  );
};

export default inject(({ settingsStore }) => {
  const { getIcon } = settingsStore;

  return {
    getIcon,
  };
})(withTranslation(["InfoPanel", "Common", "Home"])(observer(SingleItem)));
