import React from "react";
import {
  StyledInfoPanelHeader,
  StyledInfoPanelToggleWrapper,
} from "./StyledInfoPanelHeaderLoader";
import RectangleLoader from "../RectangleLoader";

import {
  isTablet,
  isMobile as isMobileUtils,
  isDesktop,
} from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

const InfoPanelHeaderLoader = () => {
  const customRectangleLoader = (width, height, rounded) => (
    <RectangleLoader
      width={"" + width}
      height={"" + height}
      borderRadius={"" + rounded}
    />
  );

  return (
    <StyledInfoPanelHeader>
      <div className="header-text">{customRectangleLoader(200, 28, 3)}</div>
      <StyledInfoPanelToggleWrapper
        isRootFolder={true}
        isInfoPanelVisible={true}
      >
        {!(isTablet() || isMobile || isMobileUtils() || !isDesktop()) &&
          customRectangleLoader(32, 32, "50%")}
      </StyledInfoPanelToggleWrapper>
    </StyledInfoPanelHeader>
  );
};

export default InfoPanelHeaderLoader;
