import React from "react";
import { mount } from "enzyme";
import AZSortingReactSvg from "PUBLIC_DIR/images/a-z.sorting.react.svg";
describe("<Icons />", () => {
  it("renders without error", () => {
    const wrapper = mount(<AZSortingReactSvg />);

    expect(wrapper).toExist();
  });

  it("size small", () => {
    const wrapper = mount(<AZSortingReactSvg size="small" />);

    expect(wrapper.prop("size")).toBe("small");
  });

  it("size medium", () => {
    const wrapper = mount(<AZSortingReactSvg size="medium" />);

    expect(wrapper.prop("size")).toBe("medium");
  });

  it("size big", () => {
    const wrapper = mount(<AZSortingReactSvg size="big" />);

    expect(wrapper.prop("size")).toBe("big");
  });

  it("size scale", () => {
    const wrapper = mount(<AZSortingReactSvg size="scale" />);

    expect(wrapper.prop("size")).toBe("scale");
  });

  it("isfill prop", () => {
    const wrapper = mount(<AZSortingReactSvg isfill />);

    expect(wrapper.prop("isfill")).toBe(true);
  });

  it("isStroke prop", () => {
    const wrapper = mount(<AZSortingReactSvg isStroke />);

    expect(wrapper.prop("isStroke")).toBe(true);
  });
});
