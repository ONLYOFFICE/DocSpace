import React, { useEffect, useState } from "react";
import { LANGUAGE } from "@appserver/common/constants";
import { ADS_TIMEOUT } from "../../../../helpers/constants";
import { getLanguage } from "@appserver/common/utils";
import SnackBar from "@appserver/components/snackbar";
import { Consumer } from "@appserver/components/utils/context";
import difference from "lodash/difference";

const loadLanguagePath = async () => {
  if (!window.firebaseHelper) return;

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  const closed = JSON.parse(localStorage.getItem("barClose"));

  const banner = difference(bar, closed);

  let index = Number(localStorage.getItem("barIndex") || 0);
  if (index >= banner.length) {
    index -= 1;
  }
  const currentBar = banner[index];

  let htmlUrl =
    currentBar && window.firebaseHelper.config.authDomain
      ? `https://${window.firebaseHelper.config.authDomain}/${language}/${currentBar}/index.html`
      : null;

  await fetch(htmlUrl).then((data) => {
    if (data.ok) return;
    htmlUrl = null;
  });
  return [htmlUrl, currentBar];
};

const bannerHOC = (props) => {
  const { firstLoad, setMaintenanceExist } = props;

  const [htmlLink, setHtmlLink] = useState();
  const [campaigns, setCampaigns] = useState();

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  const updateBanner = async () => {
    const closed = JSON.parse(localStorage.getItem("barClose"));
    const banner = difference(bar, closed);
    let index = Number(localStorage.getItem("barIndex") || 0);

    if (banner.length < 1 || index + 1 >= banner.length) {
      index = 0;
    } else {
      index++;
    }

    try {
      const [htmlUrl, campaigns] = await loadLanguagePath();
      setHtmlLink(htmlUrl);
      setCampaigns(campaigns);
    } catch (e) {
      updateBanner();
    }

    localStorage.setItem("barIndex", index);
    return;
  };

  useEffect(() => {
    setTimeout(() => updateBanner(), 10000);
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);

  const onClose = () => {
    setMaintenanceExist(false);
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];
    const closed =
      closeItems.length > 0 ? [...closeItems, campaigns] : [campaigns];
    localStorage.setItem("barClose", JSON.stringify(closed));
    setHtmlLink(null);
  };

  const onLoad = () => {
    setMaintenanceExist(true);
  };

  return htmlLink && !firstLoad ? (
    <Consumer>
      {(context) => (
        <SnackBar
          sectionWidth={context.sectionWidth}
          onLoad={onLoad}
          clickAction={onClose}
          isCampaigns={true}
          htmlContent={htmlLink}
        />
      )}
    </Consumer>
  ) : null;
};

export default bannerHOC;
