import React from "react";
import { mount } from "enzyme";
import ProgressBar from ".";

describe("<Box />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ProgressBar value={50} maxValue={100} />);

    expect(wrapper).toExist();
  });
});
