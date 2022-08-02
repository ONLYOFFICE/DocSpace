import React from "react";
import { mount } from "enzyme";
import IconButton from ".";

const baseProps = {
  size: "25",
  isDisabled: false,
  iconName: "static/images/search.react.svg",
  isFill: true,
};

describe("<IconButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(<IconButton {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<IconButton {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<IconButton {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <IconButton {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
