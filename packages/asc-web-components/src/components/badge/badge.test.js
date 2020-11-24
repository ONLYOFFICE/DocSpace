import React from "react";
import { mount } from "enzyme";
import Badge from ".";

describe("<Badge />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Badge />);

    expect(wrapper).toExist();
  });

  it("displays label", () => {
    const wrapper = mount(<Badge label="10" />);

    expect(wrapper.prop("label")).toBe("10");
  });

  it("call onClick()", () => {
    const onClick = jest.fn();
    const wrapper = mount(<Badge onClick={onClick} />);

    wrapper.simulate("click");

    expect(onClick).toBeCalled();
  });

  it("call onClick() without wrapper", () => {
    const wrapper = mount(<Badge />);

    wrapper.simulate("click");

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Badge id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Badge className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Badge style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
