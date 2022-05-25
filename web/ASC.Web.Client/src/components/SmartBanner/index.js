import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { isMobile, isIOS } from "react-device-detect";
import SmartBanner from "react-smartbanner";
import "./main.css";

const Wrapper = styled.div`
  padding-bottom: 80px;
`;

const ReactSmartBanner = () => {
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

  return (
    isMobile &&
    isVisible && (
      <Wrapper>
        <SmartBanner
          title="Onlyoffice"
          author="Onlyoffice"
          force={force}
          onClose={hideBanner}
          onInstall={hideBanner}
        />
      </Wrapper>
    )
  );
};

export default ReactSmartBanner;
