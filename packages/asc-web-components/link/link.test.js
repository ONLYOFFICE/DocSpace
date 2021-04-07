import React from "react";
import { mount } from "enzyme";
import Link from ".";

const baseProps = {
  type: "page",
  color: "black",
  href: "https://github.com",
};

describe("<Link />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Link {...baseProps}>link</Link>);

    expect(wrapper).toExist();
  });

  it("with isBold prop", () => {
    const wrapper = mount(<Link {...baseProps} isBold />);

    expect(wrapper.prop("isBold")).toEqual(true);
  });

  it("with isHovered prop", () => {
    const wrapper = mount(<Link {...baseProps} isHovered />);

    expect(wrapper.prop("isHovered")).toEqual(true);
  });

  it("with isSemitransparent prop", () => {
    const wrapper = mount(<Link {...baseProps} isSemitransparent />);

    expect(wrapper.prop("isSemitransparent")).toEqual(true);
  });

  it("with type prop action", () => {
    const wrapper = mount(<Link {...baseProps} type="action" />);

    expect(wrapper.prop("type")).toEqual("action");
  });

  it("accepts id", () => {
    const wrapper = mount(<Link {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Link {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Link {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
