import React from "react";
import { mount } from "enzyme";
import Text from ".";

describe("<Text />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Text as="p" title="Some title">
        Some text
      </Text>
    );

    expect(wrapper).toExist();
  });
});
