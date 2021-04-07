import React from "react";
import { mount } from "enzyme";
import Box from ".";

describe("<Box />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Box />);

    expect(wrapper).toExist();
  });
});
