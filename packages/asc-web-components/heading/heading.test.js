import React from "react";
import { mount } from "enzyme";
import Heading from ".";

describe("<Heading />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Heading level={4} title="Some title">
        Some text
      </Heading>
    );

    expect(wrapper).toExist();
  });
});
