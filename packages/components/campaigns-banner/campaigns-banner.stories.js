import React from "react";
import CampaignsBanner from "./";

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
  img: "static/images/campaigns.cloud.png",
  btnLabel: "START FREE TRIAL",
  link:
    "https://www.onlyoffice.com/ru/registration.aspx?utm_source=personal&utm_campaign=BannerPersonalCloud",
};
