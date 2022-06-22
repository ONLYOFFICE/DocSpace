import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { isMobile, isIOS } from "react-device-detect";
import { inject, observer } from "mobx-react";
import SmartBanner from "react-smartbanner";
import "./main.css";

const Wrapper = styled.div`
  padding-bottom: 80px;
`;

const ReactSmartBanner = (props) => {
  const { t, ready, isBannerVisible, setIsBannerVisible } = props;
  const force = isIOS ? "ios" : "android";

  const getCookie = (name) => {
    let matches = document.cookie.match(
      new RegExp(
        "(?:^|; )" +
          name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, "\\$1") +
          "=([^;]*)"
      )
    );
    return matches ? decodeURIComponent(matches[1]) : undefined;
  };

  const hideBanner = () => {
    setIsBannerVisible(false);
  };

  useEffect(() => {
    const cookieClosed = getCookie("smartbanner-closed");
    const cookieInstalled = getCookie("smartbanner-installed");
    if (cookieClosed || cookieInstalled) hideBanner();
  }, []);

  const storeText = {
    ios: t("SmartBanner:AppStore"),
    android: t("SmartBanner:GooglePlay"),
    windows: "",
    kindle: "",
  };

  const priceText = {
    ios: t("SmartBanner:Price"),
    android: t("SmartBanner:Price"),
    windows: "",
    kindle: "",
  };

  const appMeta = {
    ios: "react-apple-itunes-app",
    android: "react-google-play-app",
    windows: "msApplication-ID",
    kindle: "kindle-fire-app",
  };

  const isTouchDevice =
    "ontouchstart" in window ||
    navigator.maxTouchPoints > 0 ||
    navigator.msMaxTouchPoints > 0;

  return isMobile && isBannerVisible && ready && isTouchDevice ? (
    <Wrapper>
      <SmartBanner
        title={t("SmartBanner:AppName")}
        author="Ascensio System SIA"
        button={t("Common:View")}
        force={force}
        onClose={hideBanner}
        onInstall={hideBanner}
        storeText={storeText}
        price={priceText}
        appMeta={appMeta}
      />
    </Wrapper>
  ) : (
    <></>
  );
};

export default inject(({ bannerStore }) => {
  return {
    isBannerVisible: bannerStore.isBannerVisible,
    setIsBannerVisible: bannerStore.setIsBannerVisible,
  };
})(observer(ReactSmartBanner));
