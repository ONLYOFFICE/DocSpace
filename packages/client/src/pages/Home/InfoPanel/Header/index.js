import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile as isMobileRDD } from "react-device-detect";

import { getCategoryType } from "@docspace/client/src/helpers/utils";
import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "@docspace/client/src/HOCs/withLoader";
import Submenu from "@docspace/components/submenu";
import {
  isDesktop,
  isMobile,
  isTablet,
} from "@docspace/components/utils/device";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { StyledInfoPanelHeader } from "./styles/styles";
import { CategoryType } from "@docspace/client/src/helpers/constants";

const InfoPanelHeaderContent = (props) => {
  const {
    t,
    selection,
    setIsVisible,
    roomsView,
    personalView,
    setView,
    isGallery,
  } = props;

  const closeInfoPanel = () => setIsVisible(false);

  const setMembers = () => setView("members");
  const setHistory = () => setView("history");
  const setDetails = () => setView("details");

  const submenuData = [
    {
      id: "members",
      name: "Members",
      onClick: setMembers,
      content: null,
    },
    {
      id: "history",
      name: "History",
      onClick: setHistory,
      content: null,
    },
    {
      id: "details",
      name: "Details",
      onClick: setDetails,
      content: null,
    },
  ];

  const roomsSubmenu = [...submenuData];
  const personalSubmenu = [submenuData[1], submenuData[2]];

  const categoryType = getCategoryType(location);
  let isRoomCategory =
    categoryType == CategoryType.Shared ||
    categoryType == CategoryType.SharedRoom ||
    categoryType == CategoryType.Archive ||
    categoryType == CategoryType.ArchivedRoom;

  return (
    <StyledInfoPanelHeader withSubmenu={!!selection}>
      <div className="main">
        <Text className="header-text" fontSize="21px" fontWeight="700">
          {t("Common:Info")}
        </Text>

        <ColorTheme
          {...props}
          themeId={ThemeType.InfoPanelToggle}
          isRootFolder={true}
          isInfoPanelVisible={true}
        >
          {!(isTablet() || isMobileRDD || isMobile() || !isDesktop()) && (
            <div className="info-panel-toggle-bg">
              <IconButton
                className="info-panel-toggle"
                iconName="images/panel.react.svg"
                size="16"
                isFill={true}
                onClick={closeInfoPanel}
              />
            </div>
          )}
        </ColorTheme>
      </div>

      {!isGallery && (
        <div className="submenu">
          {isRoomCategory ? (
            <Submenu
              style={{ width: "100%" }}
              data={roomsSubmenu}
              forsedActiveItemId={roomsView}
            />
          ) : (
            <Submenu
              style={{ width: "100%" }}
              data={personalSubmenu}
              forsedActiveItemId={personalView}
            />
          )}
        </div>
      )}
    </StyledInfoPanelHeader>
  );
};

export default inject(({ auth }) => {
  const {
    selection,
    setIsVisible,
    roomsView,
    personalView,
    setView,
  } = auth.infoPanelStore;
  return { selection, setIsVisible, roomsView, personalView, setView };
})(
  withTranslation(["Common"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
