import React, { useEffect, useState } from "react";
import { ADS_TIMEOUT } from "../../../../helpers/constants";
import SnackBar from "@appserver/components/snackbar";
import { Consumer } from "@appserver/components/utils/context";
import difference from "lodash/difference";
import { getBannerAttribute } from "@appserver/components/utils/banner";

const bannerHOC = (props) => {
  const { firstLoad, setMaintenanceExist } = props;

  const [htmlLink, setHtmlLink] = useState();
  const [campaigns, setCampaigns] = useState();

  const { loadLanguagePath } = getBannerAttribute();

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
    const updateInterval = setInterval(updateBanner, ADS_TIMEOUT);
    return () => {
      if (updateInterval) {
        clearInterval(updateInterval);
      }
    };
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
