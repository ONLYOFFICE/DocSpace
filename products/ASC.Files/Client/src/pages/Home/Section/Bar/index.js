import React, { useEffect, useState } from "react";
import { LANGUAGE } from "@appserver/common/constants";
import { ADS_TIMEOUT } from "../../../../helpers/constants";
import { getLanguage } from "@appserver/common/utils";
import BarBanner from "@appserver/components/bar-banner";
import Loaders from "@appserver/common/components/Loaders";

let htmlUrl;

const loadLanguagePath = async () => {
  if (!window.firebaseHelper) return;

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  let index = Number(localStorage.getItem("barIndex") || 0);
  const currentBar = bar[index];

  try {
    htmlUrl = await window.firebaseHelper.getBarHtml(currentBar, language);
  } catch (e) {
    htmlUrl = await window.firebaseHelper.getBarHtml(currentBar, "en");
  }

  return htmlUrl;
};

const bannerHOC = (props) => {
  const { firstload } = props;

  const [html, setHtml] = useState();

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  const updateBanner = async () => {
    let index = Number(localStorage.getItem("barIndex") || 0);

    if (bar.length < 1 || index + 1 >= bar.length) {
      index = 0;
    } else {
      index++;
    }

    try {
      const htmlUrl = await loadLanguagePath();
      setHtml(htmlUrl);
    } catch (e) {
      updateBanner();
    }

    localStorage.setItem("barIndex", index);
  };

  useEffect(() => {
    updateBanner();
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);
  return html ? <BarBanner html={html} /> : <Loaders.Rectangle />;
};

export default bannerHOC;
