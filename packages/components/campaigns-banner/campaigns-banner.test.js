import React from "react";
import { mount } from "enzyme";
import CampaignsBanner from ".";

describe("<CampaignsBanner />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <CampaignsBanner
        headerLabel="ONLYOFFICE for business"
        textLabel="Docs, projects, clients & emails"
        btnLabel="START FREE TRIAL"
        img="static/images/campaign.cloud.png"
        btnLink="https://onlyoffice.com"
      />
    );

    expect(wrapper).toExist();
  });
});
