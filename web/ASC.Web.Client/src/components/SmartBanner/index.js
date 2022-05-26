import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { isMobile, isIOS } from "react-device-detect";
import SmartBanner from "react-smartbanner";
import "./main.css";

const Wrapper = styled.div`
  padding-bottom: 80px;
`;

const ReactSmartBanner = (props) => {
  const { t } = props;
  const [isVisible, setIsVisible] = useState(true);
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
    setIsVisible(false);
  };

  useEffect(() => {
    const cookie = getCookie("smartbanner-closed");
    if (cookie) hideBanner();
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

  return (
    isMobile &&
    isVisible && (
      <Wrapper>
        <SmartBanner
          title="ONLYOFFICE Documents"
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
    )
  );
};

export default ReactSmartBanner;
