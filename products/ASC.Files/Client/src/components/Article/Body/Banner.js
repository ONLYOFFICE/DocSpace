import React, { useState, useEffect } from "react";
import CampaignsBanner from "@appserver/components/campaigns-banner";
import { ADS_TIMEOUT } from "../../../helpers/constants";
import { LANGUAGE } from "@appserver/common/constants";
import { getLanguage } from "@appserver/common/utils";

const Banner = () => {
  const [campaignImage, setCampaignImage] = useState();
  const [campaignTranslate, setCampaignTranslate] = useState();

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const getImage = async (campaign) => {
    const imageUrl = await window.firebaseHelper.getCampaignsImages(
      campaign.toLowerCase()
    );

    return imageUrl;
  };

  const getTranslation = async (campaign, lng) => {
    let translationUrl = await window.firebaseHelper.getCampaignsTranslations(
      campaign,
      lng
    );

    const res = await fetch(translationUrl);

    if (!res.ok) {
      translationUrl = await window.firebaseHelper.getCampaignsTranslations(
        campaign,
        "en"
      );
    }
    return await res.json();
  };

  const getBanner = async () => {
    let index = Number(localStorage.getItem("bannerIndex") || 0);
    const currentCampaign = campaigns[index];
    if (campaigns.length < 1 || index + 1 >= campaigns.length) {
      index = 0;
    } else {
      index++;
    }

    localStorage.setItem("bannerIndex", index);

    const image = await getImage(currentCampaign);
    const translate = await getTranslation(currentCampaign, language);

    setCampaignImage(image);
    setCampaignTranslate(translate);
  };

  useEffect(() => {
    getBanner();
    const adsInterval = setInterval(getBanner, ADS_TIMEOUT);

    return function cleanup() {
      clearInterval(adsInterval);
    };
  }, []);

  return (
    <>
      {campaignImage && campaignTranslate && (
        <CampaignsBanner
          headerLabel={campaignTranslate.Header}
          subHeaderLabel={campaignTranslate.SubHeader}
          img={campaignImage}
          btnLabel={campaignTranslate.ButtonLabel}
          link={campaignTranslate.Link}
        />
      )}
    </>
  );
};

export default Banner;
