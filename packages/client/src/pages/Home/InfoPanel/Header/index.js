import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { StyledInfoPanelHeader } from "./styles/styles";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../../HOCs/withLoader";
import Submenu from "@docspace/components/submenu";
import { isMobile } from "react-device-detect";
import {
  isDesktop,
  isMobile as isMobileUtils,
  isTablet,
} from "@docspace/components/utils/device";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const InfoPanelHeaderContent = (props) => {
  const {
    t,
    selection,
    setIsVisible,
    roomState,
    setRoomState,
    isGallery,
  } = props;
  const closeInfoPanel = () => setIsVisible(false);

  const setMembers = () => setRoomState("members");
  const setHistory = () => setRoomState("history");
  const setDetails = () => setRoomState("details");

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

  const [submenuStartSelect] = submenuData.filter(
    (submenuItem) => submenuItem.id === roomState
  );

  return (
    <StyledInfoPanelHeader isRoom={selection && selection.isRoom}>
      <div className="main">
        <Text className="header-text" fontSize="21px" fontWeight="700">
          {isGallery ? t("FormGallery:FormTemplateInfo") : t("Common:Info")}
        </Text>

        <ColorTheme
          {...props}
          themeId={ThemeType.InfoPanelToggle}
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
        </ColorTheme>
      </div>

      {selection?.isRoom && (
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
  const {
    selection,
    setIsVisible,
    roomState,
    setRoomState,
    isRoom,
  } = auth.infoPanelStore;
  return { selection, setIsVisible, roomState, setRoomState, isRoom };
})(
  withTranslation(["Common", "FormGallery"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
