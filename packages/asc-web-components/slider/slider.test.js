import React from "react";
import { mount } from "enzyme";
import Slider from ".";

describe("<Textarea />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Slider />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Slider id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Slider className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });
});
