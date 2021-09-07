import React, { useEffect, useState } from "react";
import { useTranslation, withTranslation } from "react-i18next";

import CampaignsBanner from "@appserver/components/campaigns-banner";
import { ADS_TIMEOUT } from "../../../helpers/constants";
import { LANGUAGE } from "@appserver/common/constants";
import i18n from "i18next";
import Backend from "i18next-http-backend";
import { getLanguage } from "@appserver/common/utils";

const i18nConfig = i18n.createInstance();

let translationUrl;

const loadLanguagePath = async () => {
  if (!window.firebaseHelper) return;

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);
  const index = Number(localStorage.getItem("bannerIndex") || 0);
  const campaign = campaigns[index];

  translationUrl = await window.firebaseHelper.getCampaignsTranslations(
    campaign,
    language
  );
  console.log("translationUrl", translationUrl);
  return translationUrl;
};

i18nConfig.use(Backend).init({
  lng: localStorage.getItem(LANGUAGE) || "en",
  fallbackLng: "en",
  load: "all",
  debug: true,
  defaultNS: "",

  backend: {
    loadPath: function () {
      return translationUrl;
    },
  },
});

const bannerHOC = (WrappedComponent) => (props) => {
  const { FirebaseHelper } = props;

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const defaultBannerName = "Cloud";
  const [bannerName, setBannerName] = useState(defaultBannerName);
  const [bannerImage, setBannerImage] = useState("");
  const [bannerTranslation, setBannerTranslation] = useState();

  const updateBanner = async () => {
    console.log("update banner");

    let index = Number(localStorage.getItem("bannerIndex") || 0);
    const campaign = campaigns[index];

    if (campaigns.length < 1 || index + 1 >= campaigns.length) {
      index = 0;
    } else {
      index++;
    }

    const image = await FirebaseHelper.getCampaignsImages(
      campaign.toLowerCase()
    );
    setBannerImage(image);

    localStorage.setItem("bannerIndex", index);
    setBannerName(campaign);
  };

  useEffect(async () => {
    const translationUrl = await loadLanguagePath();
    setBannerTranslation(translationUrl);
    updateBanner();
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);

  console.log("banner hoc render", bannerTranslation);
  if (!bannerTranslation || !bannerName || !bannerImage) return <p>Loading</p>;

  return (
    <WrappedComponent
      bannerName={bannerName}
      bannerImage={bannerImage}
      {...props}
    />
  );
};

const Banner = (props) => {
  console.log("Banner render", props);
  const { t, tReady, bannerName, bannerImage, FirebaseHelper } = props;
  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  if (
    !campaigns.length ||
    !tReady /*||
    !i18n.exists(`CampaignPersonal${bannerName}:Header`)*/
  ) {
    return <></>;
  }

  return (
    <CampaignsBanner
      headerLabel={t("Header")}
      subHeaderLabel={t("SubHeader")}
      img={bannerImage}
      btnLabel={t("ButtonLabel")}
      link={t("Link")}
    />
  );
};

const ExtendedComponent = withTranslation()(Banner);

const WrapperComponent = (props) => (
  <ExtendedComponent i18n={i18nConfig} useSuspense={false} {...props} />
);

export default bannerHOC(WrapperComponent);
