import React from "react";

import Box from "@docspace/components/box";
import Link from "@docspace/components/link";

import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";

import EmptyFolderContainer from "SRC_DIR/components/EmptyContainer/EmptyContainer";

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  className: "empty-folder_link",
  display: "flex",
};

const EmptyScreen = ({ t, onAddAction }) => {
  return (
    <EmptyFolderContainer
      headerText={t("FilesSettings:ConnectEmpty")}
      descriptionText={t("UploadPluginsHere")}
      style={{ gridColumnGap: "39px" }}
      buttonStyle={{ marginTop: "16px" }}
      imageSrc={EmptyScreenAltSvgUrl}
      buttons={
        <>
          <div className="empty-folder_container-links empty-connect_container-links">
            <img
              className="empty-folder_container_plus-image"
              src={PlusSvgUrl}
              onClick={onAddAction}
              alt="plus_icon"
            />
            <Box className="flex-wrapper_container">
              <Link {...linkStyles} onClick={onAddAction}>
                {t("UploadPlugin")}
              </Link>
            </Box>
          </div>
        </>
      }
    />
  );
};

export default EmptyScreen;
