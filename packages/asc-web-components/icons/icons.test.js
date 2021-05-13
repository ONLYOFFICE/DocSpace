import React from "react";
import { mount } from "enzyme";
import AZSortingIcon from "../../../../../public/images/a-z.sorting.react.svg"
describe("<Icons />", () => {
  it("renders without error", () => {
    const wrapper = mount(<AZSortingIcon />);

    expect(wrapper).toExist();
  });

  it("size small", () => {
    const wrapper = mount(<AZSortingIcon size="small" />);

    expect(wrapper.prop("size")).toBe("small");
  });

  it("size medium", () => {
    const wrapper = mount(<AZSortingIcon size="medium" />);

    expect(wrapper.prop("size")).toBe("medium");
  });

  it("size big", () => {
    const wrapper = mount(<AZSortingIcon size="big" />);

    expect(wrapper.prop("size")).toBe("big");
  });

  it("size scale", () => {
    const wrapper = mount(<AZSortingIcon size="scale" />);

    expect(wrapper.prop("size")).toBe("scale");
  });

  it("isfill prop", () => {
    const wrapper = mount(<AZSortingIcon isfill />);

    expect(wrapper.prop("isfill")).toBe(true);
  });

  it("isStroke prop", () => {
    const wrapper = mount(<AZSortingIcon isStroke />);

    expect(wrapper.prop("isStroke")).toBe(true);
  });
});
