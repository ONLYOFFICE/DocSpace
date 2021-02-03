import React from "react";
import { mount } from "enzyme";
import { Icons } from ".";

describe("<Icons />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Icons.AZSortingIcon />);

    expect(wrapper).toExist();
  });

  it("size small", () => {
    const wrapper = mount(<Icons.AZSortingIcon size="small" />);

    expect(wrapper.prop("size")).toBe("small");
  });

  it("size medium", () => {
    const wrapper = mount(<Icons.AZSortingIcon size="medium" />);

    expect(wrapper.prop("size")).toBe("medium");
  });

  it("size big", () => {
    const wrapper = mount(<Icons.AZSortingIcon size="big" />);

    expect(wrapper.prop("size")).toBe("big");
  });

  it("size scale", () => {
    const wrapper = mount(<Icons.AZSortingIcon size="scale" />);

    expect(wrapper.prop("size")).toBe("scale");
  });

  it("isfill prop", () => {
    const wrapper = mount(<Icons.AZSortingIcon isfill />);

    expect(wrapper.prop("isfill")).toBe(true);
  });

  it("isStroke prop", () => {
    const wrapper = mount(<Icons.AZSortingIcon isStroke />);

    expect(wrapper.prop("isStroke")).toBe(true);
  });
});
