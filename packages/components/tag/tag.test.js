import React from "react";
import { mount } from "enzyme";
import Tag from ".";

const baseProps = {
  tag: "script",
  label: "Script",
  isNewTag: false,
  isDisabled: false,
  onDelete: (tag) => console.log(tag),
  onClick: (tag) => console.log(tag),
  advancedOptions: null,
  tagMaxWidth: "160px",
};

describe("<Tag />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Tag {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Tag {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Tag {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });
});
