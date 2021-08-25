import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import CampaignsBanner from "@appserver/components/campaigns-banner";
import { ADS_TIMEOUT } from "../../../helpers/constants";

const Banner = ({ FirebaseHelper }) => {
  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const defaultBannerName = "Cloud";
  const [bannerName, setBannerName] = useState(defaultBannerName);
  const [bannerImage, setBannerImage] = useState("");

  const updateBanner = async () => {
    console.log("update banner");

    let index = Number(localStorage.getItem("bannerIndex") || 0);
    const campaign = campaigns[index];

    if (campaigns.length < 1 || index + 1 >= campaigns.length) {
      index = 0;
    } else {
      index++;
    }

    localStorage.setItem("bannerIndex", index);
    setBannerName(campaign);

    const image = await FirebaseHelper.getCampaignsImages(
      campaign.toLowerCase()
    );
    setBannerImage(image);
  };

  useEffect(() => {
    updateBanner();
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);

  const { t, i18n, ready } = useTranslation(`CampaignPersonal${bannerName}`, {
    useSuspense: false,
  });

  if (
    !campaigns.length ||
    !ready ||
    !i18n.exists(`CampaignPersonal${bannerName}:Header`)
  ) {
    return <></>;
  }

  return (
    <CampaignsBanner
      headerLabel={t(`CampaignPersonal${bannerName}:Header`)}
      subHeaderLabel={t(`CampaignPersonal${bannerName}:SubHeader`)}
      img={bannerImage}
      btnLabel={t(`CampaignPersonal${bannerName}:ButtonLabel`)}
      link={t(`CampaignPersonal${bannerName}:Link`)}
    />
  );
};

export default Banner;
