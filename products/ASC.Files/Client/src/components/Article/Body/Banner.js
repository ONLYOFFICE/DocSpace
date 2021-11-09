import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import i18n from "i18next";
import Backend from "i18next-http-backend";
import { getLanguage } from "@appserver/common/utils";

import CampaignsBanner from "@appserver/components/campaigns-banner";
import { ADS_TIMEOUT } from "../../../helpers/constants";
import { LANGUAGE } from "@appserver/common/constants";

const i18nConfig = i18n.createInstance();

let translationUrl;
let errors = 0;

const loadLanguagePath = async () => {
  if (!window.firebaseHelper) return;

  const lng = localStorage.getItem(LANGUAGE) || "en";
  const language = getLanguage(lng instanceof Array ? lng[0] : lng);

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  if (errors === campaigns.length) return;

  const index = Number(localStorage.getItem("bannerIndex") || 0);
  const campaign = campaigns[index];

  try {
    translationUrl = await window.firebaseHelper.getCampaignsTranslations(
      campaign,
      language
    );
  } catch (e) {
    try {
      translationUrl = await window.firebaseHelper.getCampaignsTranslations(
        campaign,
        "en"
      );
    } catch (e) {
      errors++;
      console.error(e);
    }
  }
  return translationUrl;
};

const bannerHOC = (WrappedComponent) => (props) => {
  const { FirebaseHelper } = props;

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const [bannerImage, setBannerImage] = useState("");
  const [bannerTranslation, setBannerTranslation] = useState();

  const updateBanner = async () => {
    //console.log("errors", errors);
    if (errors === campaigns.length) return;

    let index = Number(localStorage.getItem("bannerIndex") || 0);
    const campaign = campaigns[index];

    if (campaigns.length < 1 || index + 1 >= campaigns.length) {
      index = 0;
    } else {
      index++;
    }

    try {
      const translationUrl = await loadLanguagePath();
      setBannerTranslation(translationUrl);

      i18nConfig.use(Backend).init({
        lng: localStorage.getItem(LANGUAGE) || "en",
        fallbackLng: "en",
        load: "all",
        debug: false,
        defaultNS: "",

        backend: {
          loadPath: function () {
            return translationUrl;
          },
        },
      });

      try {
        const image = await FirebaseHelper.getCampaignsImages(
          campaign.toLowerCase()
        );
        setBannerImage(image);
      } catch (e) {
        console.error(e);
      }
    } catch (e) {
      updateBanner();
      //console.error(e);
    }

    localStorage.setItem("bannerIndex", index);
  };

  useEffect(() => {
    updateBanner();
    setInterval(updateBanner, ADS_TIMEOUT);
  }, []);

  if (!bannerTranslation || !bannerImage) return <></>;

  return <WrappedComponent bannerImage={bannerImage} {...props} />;
};

const Banner = (props) => {
  //console.log("Banner render", props);
  const { t, tReady, bannerImage } = props;
  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  if (!campaigns.length) {
    return <></>;
  }

  /*if (!tReady) {
    return <h1>Loading...</h1>;
  }*/

  return (
    <CampaignsBanner
      headerLabel={t("Header")}
      subHeaderLabel={t("SubHeader")}
      img={bannerImage}
      btnLabel={t("ButtonLabel")}
      link={t("Link")}
      isLoading={!tReady}
    />
  );
};

const BannerWithTranslation = withTranslation()(Banner);

const WrapperBanner = (props) => (
  <BannerWithTranslation i18n={i18nConfig} useSuspense={false} {...props} />
);

export default bannerHOC(WrapperBanner);
