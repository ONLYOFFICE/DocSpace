import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import CampaignsBanner from "@appserver/components/campaigns-banner";
import Loaders from "@appserver/common/components/Loaders";

const PureBanner = ({ t, tReady }) => {
  const bannerTypes = [
    "Cloud",
    "Desktop",
    "Education",
    "Enterprise",
    "Integration",
  ];

  const type = bannerTypes[Math.floor(Math.random() * bannerTypes.length)];

  if (tReady)
    return (
      <CampaignsBanner
        headerLabel={t(`CampaignPersonal${type}:Header`)}
        subHeaderLabel={t(`CampaignPersonal${type}:SubHeader`)}
        img={`/static/images/campaigns.${type}.png`}
        btnLabel={t(`CampaignPersonal${type}:ButtonLabel`)}
        link={t(`CampaignPersonal${type}:Link`)}
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

export default Banner;
