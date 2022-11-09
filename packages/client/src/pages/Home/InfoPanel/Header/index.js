import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile as isMobileRDD } from "react-device-detect";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "@docspace/client/src/HOCs/withLoader";
import Submenu from "@docspace/components/submenu";
import {
  isDesktop as isDesktopUtils,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { StyledInfoPanelHeader } from "./styles/common";
import { FolderType } from "@docspace/common/constants";

const InfoPanelHeaderContent = (props) => {
  const {
    t,
    selection,
    setIsVisible,
    roomsView,
    fileView,
    setView,
    getIsRooms,
    getIsGallery,
    getIsAccounts,
    isRootFolder,
    canRemoveUserFromRoom,
    rootFolderType,
  } = props;

  const isRooms = getIsRooms();
  const isGallery = getIsGallery();
  const isAccounts = getIsAccounts();

  const isNoItem = isRootFolder && selection?.isSelectedFolder;
  const isSeveralItems = selection && Array.isArray(selection);

  const withSubmenu = !isNoItem && !isSeveralItems && !isGallery && !isAccounts;

  const closeInfoPanel = () => setIsVisible(false);

  const setMembers = () => setView("members");
  const setHistory = () => setView("history");
  const setDetails = () => setView("details");

  const isArchiveRoot = rootFolderType === FolderType.Archive;

  const submenuData = [
    {
      id: "members",
      name: t("InfoPanel:SubmenuMembers"),
      onClick: setMembers,
      content: null,
    },
    {
      id: "history",
      name: t("InfoPanel:SubmenuHistory"),
      onClick: setHistory,
      content: null,
    },
    {
      id: "details",
      name: t("InfoPanel:SubmenuDetails"),
      onClick: setDetails,
      content: null,
    },
  ];

  const roomsSubmenu = isArchiveRoot
    ? [{ ...submenuData[0] }, { ...submenuData[2] }]
    : [...submenuData];
  const personalSubmenu = [submenuData[1], submenuData[2]];

  const isTablet =
    isTabletUtils() || isMobileUtils() || isMobileRDD || !isDesktopUtils();

  return (
    <StyledInfoPanelHeader isTablet={isTablet} withSubmenu={withSubmenu}>
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
          {!isTablet && (
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

      {withSubmenu && (
        <div className="submenu">
          {isRooms ? (
            <Submenu
              style={{ width: "100%" }}
              data={roomsSubmenu}
              forsedActiveItemId={roomsView}
            />
          ) : (
            <Submenu
              style={{ width: "100%" }}
              data={personalSubmenu}
              forsedActiveItemId={fileView}
            />
          )}
        </div>
      )}
    </StyledInfoPanelHeader>
  );
};

export default inject(({ auth, selectedFolderStore, accessRightsStore }) => {
  const {
    selection,
    setIsVisible,
    roomsView,
    fileView,
    setView,
    getIsFiles,
    getIsRooms,
    getIsGallery,
    getIsAccounts,
  } = auth.infoPanelStore;
  const { isRootFolder, rootFolderType } = selectedFolderStore;
  const { canRemoveUserFromRoom } = accessRightsStore;

  return {
    selection,
    setIsVisible,
    roomsView,
    fileView,
    setView,
    getIsFiles,
    getIsRooms,
    getIsGallery,
    getIsAccounts,

    isRootFolder,
    rootFolderType,
    canRemoveUserFromRoom,
  };
})(
  withTranslation(["Common", "InfoPanel"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
