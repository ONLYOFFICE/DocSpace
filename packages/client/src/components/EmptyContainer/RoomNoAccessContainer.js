import RoomsReactSvgUrl from "PUBLIC_DIR/images/rooms.react.svg?url";
import ManageAccessRightsReactSvgUrl from "PUBLIC_DIR/images/manage.access.rights.react.svg?url";
import ManageAccessRightsReactSvgDarkUrl from "PUBLIC_DIR/images/manage.access.rights.dark.react.svg?url";
import React from "react";

import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";

import IconButton from "@docspace/components/icon-button";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { combineUrl } from "@docspace/common/utils";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
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
    theme,
    isFrame,
  } = props;

  const descriptionRoomNoAccess = t("NoAccessRoomDescription");
  const titleRoomNoAccess = t("NoAccessRoomTitle");

  React.useEffect(() => {
    const timer = setTimeout(onGoToShared, 5000);
    return () => clearTimeout(timer);
  }, []);

  const onGoToShared = () => {
    if (isFrame) return;
    setIsLoading(true);

    setAlreadyFetchingRooms(true);
    fetchRooms(null, null)
      .then(() => {
        const filter = RoomsFilter.getDefault();

        const filterParamsStr = filter.toUrlParams();

        const url = getCategoryUrl(categoryType, filter.folder);

        const pathname = `${url}?${filterParamsStr}`;

        history.push(
          combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            config.homepage,
            pathname
          )
        );
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const goToButtons = (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        onClick={onGoToShared}
        iconName={RoomsReactSvgUrl}
        isFill
      />
      <Link onClick={onGoToShared} {...linkStyles}>
        {t("GoToMyRooms")}
      </Link>
    </div>
  );

  const propsRoomNotFoundOrMoved = {
    headerText: titleRoomNoAccess,
    descriptionText: isFrame ? "" : descriptionRoomNoAccess,
    imageSrc: theme.isBase
      ? ManageAccessRightsReactSvgUrl
      : ManageAccessRightsReactSvgDarkUrl,
    buttons: isFrame ? <></> : goToButtons,
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
    fetchRooms,
    categoryType,
    setAlreadyFetchingRooms,
    isEmptyPage,
  } = filesStore;
  const { isFrame } = auth.settingsStore;
  return {
    setIsLoading,
    fetchRooms,
    categoryType,
    setAlreadyFetchingRooms,
    isEmptyPage,
    theme: auth.settingsStore.theme,
    isFrame,
  };
})(withTranslation(["Files"])(observer(RoomNoAccessContainer)));
