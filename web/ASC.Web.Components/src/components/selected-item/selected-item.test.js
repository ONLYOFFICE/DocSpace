import React from "react";
import { mount } from "enzyme";
import SelectedItem from ".";

const baseProps = {
  text: "sample text",
  onClose: () => jest.fn(),
};

describe("<SelectedItem />", () => {
  it("renders without error", () => {
    const wrapper = mount(<SelectedItem {...baseProps}></SelectedItem>);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<SelectedItem {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<SelectedItem {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <SelectedItem {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
