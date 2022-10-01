import React from "react";
import { StyledInfoPanelHeader } from "./StyledInfoPanelHeaderLoader";
import RectangleLoader from "../RectangleLoader";

import { isMobile as isMobileRDD } from "react-device-detect";
import {
  isDesktop as isDesktopUtils,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

const InfoPanelHeaderLoader = () => {
  const isTablet =
    isTabletUtils() || isMobileUtils() || isMobileRDD || !isDesktopUtils();

  const pathname = window.location.pathname.toLowerCase();
  const isGallery = pathname.indexOf("form-gallery") !== -1;
  const isRooms = pathname.indexOf("rooms") !== -1;
  const isAccounts =
    pathname.indexOf("accounts") !== -1 && !(pathname.indexOf("view") !== -1);
  const withSubmenu = !isGallery && !isAccounts;

  return (
    <StyledInfoPanelHeader isTablet={isTablet} withSubmenu={false}>
      <div className="main">
        <RectangleLoader width={"120px"} height={"24px"} borderRadius={"3px"} />
        {!isTablet && (
          <div className="info-panel-toggle-bg">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"50%"}
            />
          </div>
        )}
      </div>

      {/* {withSubmenu && (
        <div className="submenu">
          <RectangleLoader
            width={"60px"}
            height={"20px"}
            borderRadius={"3px"}
          />
          <RectangleLoader
            width={"60px"}
            height={"20px"}
            borderRadius={"3px"}
          />
          {isRooms && (
            <RectangleLoader
              width={"60px"}
              height={"20px"}
              borderRadius={"3px"}
            />
          )}
        </div>
      )} */}
    </StyledInfoPanelHeader>
  );
};

export default InfoPanelHeaderLoader;
