import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import { inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import { FileType } from "@docspace/common/constants";
import Text from "@docspace/components/text";

import DetailsHelper from "../../helpers/DetailsHelper.js";
import { StyledNoThumbnail, StyledThumbnail } from "../../styles/details.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";
import api from "@docspace/common/api/index.js";

const Details = ({
  t,
  selection,
  personal,
  culture,
  createThumbnail,
  getInfoPanelItemIcon,
  openUser,
  isVisitor,
}) => {
  const [itemProperties, setItemProperties] = useState([]);

  const [isThumbnailError, setIsThumbmailError] = useState(false);
  const onThumbnailError = () => setIsThumbmailError(true);

  const history = useHistory();

  const detailsHelper = new DetailsHelper({
    isVisitor,
    t,
    item: selection,
    openUser,
    history,
    personal,
    culture,
  });

  useEffect(async () => {
    setItemProperties(detailsHelper.getPropertyList());

    if (
      !selection.isFolder &&
      selection.thumbnailStatus === 0 &&
      (selection.fileType === FileType.Image ||
        selection.fileType === FileType.Spreadsheet ||
        selection.fileType === FileType.Presentation ||
        selection.fileType === FileType.Document)
    ) {
      await createThumbnail(selection.id);
    }
  }, [selection]);

  const currentIcon =
    !selection.isArchive && selection?.logo?.large
      ? selection?.logo?.large
      : getInfoPanelItemIcon(selection, 96);

  return (
    <>
      {selection.thumbnailUrl && !isThumbnailError ? (
        <StyledThumbnail>
          <img
            src={selection.thumbnailUrl}
            alt="thumbnail-image"
            height={260}
            width={360}
            onError={onThumbnailError}
          />
        </StyledThumbnail>
      ) : (
        <StyledNoThumbnail>
          <img
            className={`no-thumbnail-img ${selection.isRoom && "is-room"} ${
              selection.isRoom &&
              !selection.isArchive &&
              selection.logo?.large &&
              "custom-logo"
            }`}
            src={currentIcon}
            alt="thumbnail-icon-big"
          />
        </StyledNoThumbnail>
      )}

      <StyledSubtitle>
        <Text fontWeight="600" fontSize="14px">
          {t("Properties")}
        </Text>
      </StyledSubtitle>

      <StyledProperties>
        {itemProperties.map((property) => {
          return (
            <div
              id={property.id}
              key={property.title}
              className={`property ${property.className}`}
            >
              <Text className="property-title">{property.title}</Text>
              {property.content}
            </div>
          );
        })}
      </StyledProperties>
    </>
  );
};

export default inject(({ auth, filesStore }) => {
  const { userStore } = auth;
  const { selection, getInfoPanelItemIcon, openUser } = auth.infoPanelStore;
  const { createThumbnail } = filesStore;
  const { personal, culture } = auth.settingsStore;
  const { user } = userStore;

  const isVisitor = user.isVisitor;

  return {
    personal,
    culture,
    selection,
    createThumbnail,
    getInfoPanelItemIcon,
    openUser,
    isVisitor,
  };
})(withTranslation(["InfoPanel", "Common", "Translations", "Files"])(Details));
