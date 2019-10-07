import React from "react";
import { mount } from "enzyme";
import Tooltip from ".";
import { Text } from "../text";

describe("<Tooltip />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Tooltip
      />
    );

    expect(wrapper).toExist();
  });
});
