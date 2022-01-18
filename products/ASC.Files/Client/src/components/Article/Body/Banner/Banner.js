import React, { useState, useEffect } from "react";
import CampaignsBanner from "@appserver/components/campaigns-banner";
import { DBConfig } from "./DBConfig";
import { ADS_TIMEOUT } from "../../../../helpers/constants";
import { LANGUAGE } from "@appserver/common/constants";
import { getLanguage } from "@appserver/common/utils";
import { initDB, useIndexedDB } from "react-indexed-db";

initDB(DBConfig);

const Banner = () => {
  const { add, getByIndex, getAll } = useIndexedDB("ads");
  const [campaign, setCampaign] = useState();

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const fetchBanners = async () => {
    if (!campaigns) return;
    const campaignsDB = await getAll();

    campaigns.map(async (campaign) => {
      if (campaignsDB.some((el) => el.campaign === campaign)) return;

      const t = await getTranslation(campaign, language);
      const i = await getImage(campaign);

      add({ campaign: campaign, image: i, translate: t }).then(
        (event) => {
          console.log("ID Generated: ", event);
        },
        (error) => {
          console.log(error);
        }
      );
    });
  };

  const getTranslation = async (campaign, lng) => {
    const translationUrl = await window.firebaseHelper.getCampaignsTranslations(
      campaign,
      lng
    );

    console.log(translationUrl);

    let obj = await (await fetch(translationUrl)).json();
    console.log(obj);

    return obj;
  };

  const getImage = async (campaign) => {
    const imageUrl = await window.firebaseHelper.getCampaignsImages(
      campaign.toLowerCase()
    );

    console.log(imageUrl);
    return imageUrl;
  };

  const getBanner = () => {
    console.log("getBanner");
    let index = Number(localStorage.getItem("bannerIndex") || 0);
    const currentCampaign = campaigns[index];
    if (campaigns.length < 1 || index + 1 >= campaigns.length) {
      index = 0;
    } else {
      index++;
    }

    getByIndex("campaign", currentCampaign).then((campaignFromDB) => {
      setCampaign(campaignFromDB);
    });

    localStorage.setItem("bannerIndex", index);
  };

  useEffect(() => {
    fetchBanners();
    getBanner();
    const adsInterval = setInterval(getBanner, 5000);

    return function cleanup() {
      clearInterval(adsInterval);
    };
  }, []);

  return (
    <>
      {campaign && (
        <CampaignsBanner
          headerLabel={campaign.translate.Header}
          subHeaderLabel={campaign.translate.SubHeader}
          img={campaign.image}
          btnLabel={campaign.translate.ButtonLabel}
          link={campaign.translate.Link}
        />
      )}
    </>
  );
};

export default Banner;
