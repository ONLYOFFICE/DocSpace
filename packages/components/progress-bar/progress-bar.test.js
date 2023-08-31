import React from "react";
import { mount } from "enzyme";
import ProgressBar from ".";

describe("<ProgressBar />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <ProgressBar percent={50} label="Some work in progress" />
    );

    expect(wrapper).toExist();
  });
});
