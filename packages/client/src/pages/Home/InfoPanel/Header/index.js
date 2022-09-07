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
  isDesktop,
  isMobile,
  isTablet,
} from "@docspace/components/utils/device";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { StyledInfoPanelHeader } from "./styles/styles";

const InfoPanelHeaderContent = (props) => {
  const {
    t,
    selection,
    setIsVisible,
    roomView,
    itemView,
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

  const roomSubmenu = [...submenuData];
  const itemSubmenu = [submenuData[1], submenuData[2]];

  return (
    <StyledInfoPanelHeader withSubmenu={!!selection}>
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

      <div className="submenu">
        {selection?.isRoom ? (
          <Submenu
            style={{ width: "100%" }}
            data={roomSubmenu}
            forsedActiveItemId={roomView}
          />
        ) : (
          <Submenu
            style={{ width: "100%" }}
            data={itemSubmenu}
            forsedActiveItemId={itemView}
          />
        )}
      </div>
    </StyledInfoPanelHeader>
  );
};

export default inject(({ auth }) => {
  const {
    selection,
    setIsVisible,
    roomView,
    itemView,
    setView,
  } = auth.infoPanelStore;
  return { selection, setIsVisible, roomView, itemView, setView };
})(
  withTranslation(["Common", "FormGallery"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
