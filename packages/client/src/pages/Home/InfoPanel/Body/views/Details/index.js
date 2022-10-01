import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders/index.js";

import { FileType } from "@docspace/common/constants";
import Text from "@docspace/components/text";

import DetailsHelper from "../../helpers/DetailsHelper.js";
import { StyledNoThumbnail, StyledThumbnail } from "../../styles/details.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";

const Details = ({ t, selection, setSelection, ...props }) => {
  const [itemProperties, setItemProperties] = useState([]);

  const detailsHelper = new DetailsHelper({ t, item: selection, ...props });

  useEffect(async () => {
    setItemProperties(detailsHelper.getPropertyList());

    if (
      !selection.thumbnailUrl &&
      !selection.isFolder &&
      selection.thumbnailStatus === 0 &&
      (selection.fileType === FileType.Image ||
        selection.fileType === FileType.Spreadsheet ||
        selection.fileType === FileType.Presentation ||
        selection.fileType === FileType.Document)
    ) {
      await createThumbnail(selection.id);
      setSelection({ ...selection, hasCustonThumbnail: true });
    }
  }, [selection]);

  return (
    <>
      {selection?.hasCustonThumbnail ? (
        <StyledThumbnail>
          <img
            src={selection.thumbnailUrl}
            alt="thumbnail-image"
            height={260}
            width={360}
          />
        </StyledThumbnail>
      ) : (
        <StyledNoThumbnail>
          <img
            className={`no-thumbnail-img ${selection.isRoom && "is-room"} ${
              selection.isRoom && selection.logo?.large && "custom-logo"
            }`}
            src={selection.thumbnailUrl}
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

export default withTranslation(["InfoPanel", "Common", "Translations"])(
  withLoader(Details)(<Loaders.InfoPanelViewLoader view="details" />)
);
