import React, { useState, useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import CampaignsBanner from "@appserver/components/campaigns-banner";
import Loaders from "@appserver/common/components/Loaders";

const PureBanner = ({ t, tReady, getBannerType, bannerTypes }) => {
  const [bannerType, setBannerType] = useState("");

  useEffect(() => {
    if (!localStorage.getItem("banner")) {
      localStorage.setItem("banner", 0);
    }

    const index = Number(localStorage.getItem("banner"));
    const banner = getBannerType(index);

    if (index + 1 === bannerTypes.length) {
      localStorage.setItem("banner", 0);
    } else {
      localStorage.setItem("banner", Number(index + 1));
    }

    setBannerType(banner);
  }, []);

  if (tReady)
    return (
      <CampaignsBanner
        headerLabel={t(`CampaignPersonal${bannerType}:Header`)}
        subHeaderLabel={t(`CampaignPersonal${bannerType}:SubHeader`)}
        img={`/static/images/campaigns.${bannerType}.png`}
        btnLabel={t(`CampaignPersonal${bannerType}:ButtonLabel`)}
        link={t(`CampaignPersonal${bannerType}:Link`)}
      />
    );
  else return <Loaders.Rectangle />;
};

const Banner = withTranslation([
  "CampaignPersonalCloud",
  "CampaignPersonalDesktop",
  "CampaignPersonalEducation",
  "CampaignPersonalEnterprise",
  "CampaignPersonalIntegration",
])(withRouter(PureBanner));

export default inject(({ bannerStore }) => {
  const { getBannerType, bannerTypes } = bannerStore;

  return { getBannerType, bannerTypes };
})(observer(Banner));
