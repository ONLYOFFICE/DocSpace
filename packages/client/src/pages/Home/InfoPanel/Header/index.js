import {
  isTablet,
  isMobile as isMobileUtils,
  isDesktop,
} from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { StyledInfoPanelHeader } from "./styles/styles";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../../HOCs/withLoader";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const InfoPanelHeaderContent = (props) => {
  const { t, setIsVisible, isGallery } = props;
  const closeInfoPanel = () => setIsVisible(false);

  return (
    <StyledInfoPanelHeader>
      <Text className="header-text" fontSize="21px" fontWeight="700">
        {t("Common:Info")}
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
    </StyledInfoPanelHeader>
  );
};

export default inject(({ auth }) => {
  const { setIsVisible } = auth.infoPanelStore;
  return { setIsVisible };
})(
  withTranslation(["Common", "FormGallery"])(
    withLoader(observer(InfoPanelHeaderContent))(
      <Loaders.InfoPanelHeaderLoader />
    )
  )
);
