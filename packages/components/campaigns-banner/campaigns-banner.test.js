import React from "react";
import { mount } from "enzyme";
import CampaignsBanner from ".";
import CampaignsCloudPng from "PUBLIC_DIR/images/campaigns.cloud.png";

describe("<CampaignsBanner />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <CampaignsBanner
        headerLabel="ONLYOFFICE for business"
        textLabel="Docs, projects, clients & emails"
        buttonLabel="START FREE TRIAL"
        img={CampaignsCloudPng}
        btnLink="https://onlyoffice.com"
      />
    );

    expect(wrapper).toExist();
  });
});
