import React, { useEffect, useState } from "react";
import { LANGUAGE } from "@appserver/common/constants";
import { ADS_TIMEOUT } from "../../../../helpers/constants";
import { getLanguage } from "@appserver/common/utils";
import BarBanner from "@appserver/components/bar-banner";
import { isDesktop, isTablet } from "react-device-detect";

const loadLanguagePath = () => {
  if (!window.firebaseHelper) return;

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  let index = Number(localStorage.getItem("barIndex") || 0);
  const currentBar = bar[index];

  const htmlUrl = `https://${window.firebaseHelper.config.authDomain}/${language}/${currentBar}/index.html`;

  return htmlUrl;
};

const bannerHOC = (props) => {
  const { firstLoad, personal } = props;

  const [htmlLink, setHtmlLink] = useState();

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  const updateBanner = () => {
    window.firebaseHelper.checkMaintenance().then((campaign) => {
      if (!campaign) {
        let index = Number(localStorage.getItem("barIndex") || 0);

        if (bar.length < 1 || index + 1 >= bar.length) {
          index = 0;
        } else {
          index++;
        }

        try {
          const htmlUrl = loadLanguagePath();
          setHtmlLink(htmlUrl);
        } catch (e) {
          updateBanner();
        }

        localStorage.setItem("barIndex", index);
        return;
      }

      return;
    });
  };

  useEffect(() => {
    updateBanner();
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);

  return (isDesktop || isTablet) && personal && htmlLink && !firstLoad ? (
    <BarBanner htmlLink={htmlLink} />
  ) : (
    ""
  );
};

export default bannerHOC;
