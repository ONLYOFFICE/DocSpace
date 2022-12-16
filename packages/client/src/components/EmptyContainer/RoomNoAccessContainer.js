import React from "react";

import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";

import RoomsFilter from "@docspace/common/api/rooms/filter";
import { combineUrl } from "@docspace/common/utils";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { AppServerConfig } from "@docspace/common/constants";
import history from "@docspace/common/history";
import config from "PACKAGE_FILE";

const RoomNoAccessContainer = (props) => {
  const {
    t,
    setIsLoading,
    linkStyles,
    fetchRooms,
    setAlreadyFetchingRooms,
    categoryType,
    isEmptyPage,
    sectionWidth,
  } = props;

  const descriptionRoomNoAccess = t("NoAccessRoomDescription");
  const titleRoomNoAccess = t("NoAccessRoomTitle");

  React.useEffect(() => {
    const timer = setTimeout(onGoToShared, 5000);
    return () => clearTimeout(timer);
  }, []);

  const onGoToShared = () => {
    setIsLoading(true);

    setAlreadyFetchingRooms(true);
    fetchRooms(null, null)
      .then(() => {
        const filter = RoomsFilter.getDefault();

        const filterParamsStr = filter.toUrlParams();

        const url = getCategoryUrl(categoryType, filter.folder);

        const pathname = `${url}?${filterParamsStr}`;

        history.push(
          combineUrl(AppServerConfig.proxyURL, config.homepage, pathname)
        );
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const goToButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container-image"
        src="images/empty-folder-image.svg"
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
    imageSrc: "static/images/manage.access.rights.react.svg",
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

export default inject(({ filesStore }) => {
  const {
    setIsLoading,
    fetchRooms,
    categoryType,
    setAlreadyFetchingRooms,
    isEmptyPage,
  } = filesStore;
  return {
    setIsLoading,
    fetchRooms,
    categoryType,
    setAlreadyFetchingRooms,
    isEmptyPage,
  };
})(withTranslation(["Files"])(observer(RoomNoAccessContainer)));
