import React, { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import { FileType } from "@docspace/common/constants";
import Text from "@docspace/components/text";

import DetailsHelper from "../../helpers/DetailsHelper.js";
import { StyledNoThumbnail, StyledThumbnail } from "../../styles/details.js";
import { StyledProperties, StyledSubtitle } from "../../styles/common.js";
import RoomIcon from "@docspace/client/src/components/RoomIcon";
const Details = ({
  t,
  selection,
  personal,
  culture,
  createThumbnail,
  getInfoPanelItemIcon,
  openUser,
  isVisitor,
  isCollaborator,
  selectTag,
}) => {
  const [itemProperties, setItemProperties] = useState([]);

  const [isThumbnailError, setIsThumbmailError] = useState(false);
  const onThumbnailError = () => setIsThumbmailError(true);

  const navigate = useNavigate();

  const detailsHelper = new DetailsHelper({
    isCollaborator,
    isVisitor,
    t,
    item: selection,
    openUser,
    navigate,
    personal,
    culture,
    selectTag,
  });

  const createThumbnailAction = useCallback(async () => {
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

  useEffect(() => {
    createThumbnailAction();
  }, [selection, createThumbnailAction]);

  const currentIcon =
    !selection.isArchive && selection?.logo?.large
      ? selection?.logo?.large
      : getInfoPanelItemIcon(selection, 96);

  //console.log("InfoPanel->Details render", { selection });

  const isLoadedRoomIcon = !!selection.logo?.large;
  const showDefaultRoomIcon = !isLoadedRoomIcon && selection.isRoom;

  return (
    <>
      {selection.thumbnailUrl && !isThumbnailError ? (
        <StyledThumbnail
          isImageOrMedia={
            selection?.viewAccessability?.ImageView ||
            selection?.viewAccessability?.MediaView
          }
        >
          <img
            src={`${selection.thumbnailUrl}&size=1280x720`}
            alt="thumbnail-image"
            //height={260}
            //width={360}
            onError={onThumbnailError}
          />
        </StyledThumbnail>
      ) : (
        <StyledNoThumbnail>
          {showDefaultRoomIcon ? (
            <RoomIcon
              color={selection.logo.color}
              title={selection.title}
              isArchive={selection.isArchive}
              size="96px"
              radius="16px"
            />
          ) : (
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
          )}
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

export default inject(({ auth, filesStore, filesActionsStore }) => {
  const { userStore } = auth;
  const { selection, getInfoPanelItemIcon, openUser } = auth.infoPanelStore;
  const { createThumbnail } = filesStore;
  const { personal, culture } = auth.settingsStore;
  const { user } = userStore;

  const { selectTag } = filesActionsStore;

  const isVisitor = user.isVisitor;
  const isCollaborator = user.isCollaborator;

  return {
    personal,
    culture,
    selection,
    createThumbnail,
    getInfoPanelItemIcon,
    openUser,
    isVisitor,
    isCollaborator,
    selectTag,
  };
})(withTranslation(["InfoPanel", "Common", "Translations", "Files"])(Details));
