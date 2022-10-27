import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import { inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import { FileType } from "@docspace/common/constants";
import Text from "@docspace/components/text";

import DetailsHelper from "../../helpers/DetailsHelper.js";
import { StyledNoThumbnail, StyledThumbnail } from "../../styles/details.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";
import { ItemIcon } from "@docspace/components";

const Details = ({
  t,
  selection,
  personal,
  culture,
  createThumbnail,
  getItemIcon,
  openUser,
}) => {
  const [itemProperties, setItemProperties] = useState([]);

  const [isThumbnailError, setIsThumbmailError] = useState(false);
  const onThumbnailError = () => setIsThumbmailError(true);

  const history = useHistory();

  const detailsHelper = new DetailsHelper({
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
          <ItemIcon item={!!selection && selection} roomLogoSize="large" />
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
            <div key={property.title} className="property">
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
  const { selection, getItemIcon, openUser } = auth.infoPanelStore;
  const { createThumbnail } = filesStore;
  const { personal, culture } = auth.settingsStore;

  return {
    personal,
    culture,
    selection,
    createThumbnail,
    getItemIcon,
    openUser,
  };
})(withTranslation(["InfoPanel", "Common", "Translations"])(Details));
