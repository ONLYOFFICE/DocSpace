import React from "react";
import { mount } from "enzyme";
import Headline from ".";

describe("<Headline />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Headline level={1} title="Some title">
        Some text
      </Headline>
    );

    expect(wrapper).toExist();
  });
});
