import React from "react";
import CampaignsBanner from "./";
import CampaignsCloudPng from "PUBLIC_DIR/images/campaigns.cloud.png";

export default {
  title: "Components/CampaignsBanner",
  component: CampaignsBanner,
  parameters: {
    docs: {
      description: {
        component: "Used to display an campaigns banner.",
      },
    },
  },
};

const Template = (args) => <CampaignsBanner {...args} />;

export const Default = Template.bind({});

Default.args = {
  headerLabel: "ONLYOFFICE for business",
  subHeaderLabel: "Docs, projects, clients & emails",
  img: CampaignsCloudPng,
  buttonLabel: "START FREE TRIAL",
  link: "https://www.onlyoffice.com/ru/registration.aspx?utm_source=personal&utm_campaign=BannerPersonalCloud",
};
