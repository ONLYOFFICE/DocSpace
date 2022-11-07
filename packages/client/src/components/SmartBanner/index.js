import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { isMobile, isIOS } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { useLocation } from "react-router-dom";
import SmartBanner from "react-smartbanner";
import "./main.css";

const Wrapper = styled.div`
  padding-bottom: 80px;
`;

const ReactSmartBanner = (props) => {
  const { t, ready, isBannerVisible, setIsBannerVisible } = props;
  const force = isIOS ? "ios" : "android";
  const location = useLocation();

  const [isDocuments, setIsDocuments] = useState(false);

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

  useEffect(() => {
    const path = window.location.pathname.toLowerCase();
    if (path.includes("rooms") || path.includes("files")) {
      let vh = (window.innerHeight - 80) * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);
      setIsDocuments(true);
    } else {
      let vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);
      setIsDocuments(false);
    }
  }, [location]);

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

  return isMobile &&
    isBannerVisible &&
    ready &&
    isTouchDevice &&
    isDocuments ? (
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
