import React from "react";
import { mount } from "enzyme";
import ProgressBar from ".";

describe("<Box />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <ProgressBar percent={50} value={50} maxValue={100} />
    );

    expect(wrapper).toExist();
  });
});
