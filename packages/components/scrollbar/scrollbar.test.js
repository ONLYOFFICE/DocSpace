import React from "react";
import { mount } from "enzyme";
import Scrollbar from ".";

describe("<Scrollbar />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Scrollbar>Some content</Scrollbar>);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Scrollbar id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Scrollbar className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Scrollbar style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
