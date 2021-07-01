import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import CampaignsBanner from "@appserver/components/campaigns-banner";
import Loaders from "@appserver/common/components/Loaders";

const PureBanner = ({ t, tReady, getBannerType }) => {
  const bannerType = getBannerType();

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
  const { getBannerType } = bannerStore;

  return { getBannerType };
})(observer(Banner));
