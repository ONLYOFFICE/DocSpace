import EmptyFolderImageSvgUrl from "PUBLIC_DIR/images/empty-folder-image.svg?url";
import ManageAccessRightsReactSvgUrl from "PUBLIC_DIR/images/manage.access.rights.react.svg?url";
import ManageAccessRightsReactSvgDarkUrl from "PUBLIC_DIR/images/manage.access.rights.dark.react.svg?url";
import React from "react";

import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";

import RoomsFilter from "@docspace/common/api/rooms/filter";

import { getCategoryUrl } from "SRC_DIR/helpers/utils";

const RoomNoAccessContainer = (props) => {
  const {
    t,
    setIsLoading,
    linkStyles,

    isEmptyPage,
    sectionWidth,
    theme,
  } = props;

  const descriptionRoomNoAccess = t("NoAccessRoomDescription");
  const titleRoomNoAccess = t("NoAccessRoomTitle");

  const navigate = useNavigate();

  React.useEffect(() => {
    const timer = setTimeout(onGoToShared, 5000);
    return () => clearTimeout(timer);
  }, []);

  const onGoToShared = () => {
    setIsLoading(true);

    const filter = RoomsFilter.getDefault();

    const filterParamsStr = filter.toUrlParams();

    navigate(`rooms/shared/filter?${filterParamsStr}`);
  };

  const goToButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container-image"
        src={EmptyFolderImageSvgUrl}
        onClick={onGoToShared}
        alt="folder_icon"
      />
      <Link onClick={onGoToShared} {...linkStyles}>
        {t("GoToMyRooms")}
      </Link>
    </div>
  );

  const propsRoomNotFoundOrMoved = {
    headerText: titleRoomNoAccess,
    descriptionText: descriptionRoomNoAccess,
    imageSrc: theme.isBase
      ? ManageAccessRightsReactSvgUrl
      : ManageAccessRightsReactSvgDarkUrl,
    buttons: goToButtons,
  };

  return (
    <EmptyContainer
      isEmptyPage={isEmptyPage}
      sectionWidth={sectionWidth}
      imageStyle={{ marginRight: "20px" }}
      className="empty-folder_room-not-found"
      {...propsRoomNotFoundOrMoved}
    />
  );
};

export default inject(({ auth, filesStore }) => {
  const {
    setIsLoading,

    isEmptyPage,
  } = filesStore;
  return {
    setIsLoading,

    isEmptyPage,
    theme: auth.settingsStore.theme,
  };
})(withTranslation(["Files"])(observer(RoomNoAccessContainer)));
