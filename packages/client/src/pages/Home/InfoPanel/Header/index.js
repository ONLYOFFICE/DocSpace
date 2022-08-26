import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  StyledInfoPanelHeader,
  StyledInfoPanelToggleWrapper,
} from "./styles/styles";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../../HOCs/withLoader";
import Submenu from "@docspace/components/submenu";
import { isMobile } from "react-device-detect";
import {
  isDesktop,
  isMobile as isMobileUtils,
  isTablet,
} from "@docspace/components/utils/device";

const InfoPanelHeaderContent = ({
  t,
  setIsVisible,
  roomState,
  setRoomState,
  isGallery,
  isRoom,
}) => {
  const closeInfoPanel = () => setIsVisible(false);

  const submenuData = [
    {
      id: "members",
      name: "Members",
      onClick: () => setRoomState("members"),
      content: null,
    },
    {
      id: "history",
      name: "History",
      onClick: () => setRoomState("history"),
      content: null,
    },
    {
      id: "details",
      name: "Details",
      onClick: () => setRoomState("details"),
      content: null,
    },
  ];

  const [submenuStartSelect] = submenuData.filter(
    (submenuItem) => submenuItem.id === roomState
  );

  return (
    <StyledInfoPanelHeader isRoom={isRoom}>
      <div className="main">
        <Text className="header-text" fontSize="21px" fontWeight="700">
          {isRoom
            ? t("Room")
            : isGallery
            ? t("FormGallery:FormTemplateInfo")
            : t("Common:Info")}
        </Text>
        <StyledInfoPanelToggleWrapper
          isRootFolder={true}
          isInfoPanelVisible={true}
        >
          {!(isTablet() || isMobile || isMobileUtils() || !isDesktop()) && (
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
        </StyledInfoPanelToggleWrapper>
      </div>

      {isRoom && (
        <div className="submenu">
          <Submenu
            style={{ width: "100%" }}
            data={submenuData}
            startSelect={submenuStartSelect}
          />
        </div>
      )}
    </StyledInfoPanelHeader>
  );
};

export default inject(({ auth }) => {
  const { setIsVisible, roomState, setRoomState, isRoom } = auth.infoPanelStore;
  return { setIsVisible, roomState, setRoomState, isRoom };
})(
  withTranslation(["Common", "FormGallery"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
