import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import React from "react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";
import { IconButton } from "@docspace/components";

const DialogHeader = ({ t, isEdit, isChooseRoomType, onArrowClick }) => {
  return (
    <>
      {isEdit ? (
        t("RoomEditing")
      ) : isChooseRoomType ? (
        t("ChooseRoomType")
      ) : (
        <div className="header-with-button">
          <IconButton
            size="15px"
            iconName={ArrowPathReactSvgUrl}
            className="sharing_panel-arrow"
            onClick={onArrowClick}
          />
          <div>{t("CreateRoom")}</div>
        </div>
      )}
    </>
  );
};

export default withTranslation(["CreateEditRoomDialog", "Files"])(
  withLoader(DialogHeader)(<Loaders.CreateEditRoomDilogHeaderLoader />)
);
