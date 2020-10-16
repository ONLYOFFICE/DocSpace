import React from "react";
import { mount } from "enzyme";
import ToggleContent from ".";

describe("<ToggleContent />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <ToggleContent>
        <span>Some text</span>
      </ToggleContent>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<ToggleContent id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<ToggleContent className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<ToggleContent style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
