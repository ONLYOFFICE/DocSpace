import CrossReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import React, { useState, useEffect } from "react";
import { inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import Submenu from "@docspace/components/submenu";
import {
  isDesktop as isDesktopUtils,
  isSmallTablet as isSmallTabletUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

import { StyledInfoPanelHeader } from "./styles/common";
import { RoomsType } from "@docspace/common/constants";

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
    getIsTrash,
    isRootFolder,
    isPublicRoom,
  } = props;

  const [isTablet, setIsTablet] = useState(false);

  const isRooms = getIsRooms();
  const isGallery = getIsGallery();
  const isAccounts = getIsAccounts();
  const isTrash = getIsTrash();

  const isNoItem = isRootFolder && selection?.isSelectedFolder;
  const isSeveralItems = selection && Array.isArray(selection);

  const withSubmenu =
    !isNoItem && !isSeveralItems && !isGallery && !isAccounts && !isTrash;

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    const isTablet =
      isTabletUtils() || isSmallTabletUtils() || !isDesktopUtils();

    setIsTablet(isTablet);
  };

  const closeInfoPanel = () => setIsVisible(false);

  const setMembers = () => setView("info_members");
  const setHistory = () => setView("info_history");
  const setDetails = () => setView("info_details");

  const submenuData = [
    {
      id: "info_members",
      name: isPublicRoom ? t("Common:LinksAndMembers") : t("Common:Members"),
      onClick: setMembers,
      content: null,
    },
    {
      id: "info_history",
      name: t("InfoPanel:SubmenuHistory"),
      onClick: setHistory,
      content: null,
    },
    {
      id: "info_details",
      name: t("InfoPanel:SubmenuDetails"),
      onClick: setDetails,
      content: null,
    },
  ];

  const roomsSubmenu = [...submenuData];

  const personalSubmenu = [submenuData[1], submenuData[2]];

  return (
    <StyledInfoPanelHeader isTablet={isTablet} withSubmenu={withSubmenu}>
      <div className="main">
        <Text className="header-text" fontSize="21px" fontWeight="700">
          {t("Common:Info")}
        </Text>

        {!isTablet && (
          <div className="info-panel-toggle-bg">
            <IconButton
              id="info-panel-toggle--close"
              className="info-panel-toggle"
              iconName={CrossReactSvgUrl}
              size="16"
              isFill={true}
              onClick={closeInfoPanel}
              title={t("Common:InfoPanel")}
            />
          </div>
        )}
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

export default inject(({ auth, selectedFolderStore }) => {
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
    getIsTrash,
  } = auth.infoPanelStore;
  const { isRootFolder, roomType } = selectedFolderStore;

  const isPublicRoom =
    selection?.roomType === RoomsType.PublicRoom ||
    roomType === RoomsType.PublicRoom;

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
    getIsTrash,
    isRootFolder,
    isPublicRoom,
  };
})(withTranslation(["Common", "InfoPanel"])(InfoPanelHeaderContent));
