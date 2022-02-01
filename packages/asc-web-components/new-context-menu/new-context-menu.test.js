import React from "react";
import { mount, shallow } from "enzyme";
import NewContextMenu from ".";

describe("<MenuItem />", () => {
  it("renders without error", () => {
    const wrapper = mount(<NewContextMenu />);

    expect(wrapper).toExist();
  });
});
