import React from "react";
import { mount } from "enzyme";
import RequestLoader from ".";

describe("<RequestLoader />", () => {
  it("renders without error", () => {
    const wrapper = mount(<RequestLoader label="Loading... Please wait..." />);

    expect(wrapper).toExist();
  });

  it("accepts className", () => {
    const wrapper = mount(<RequestLoader visible />);

    expect(wrapper.prop("visible")).toEqual(true);
  });

  it("accepts id", () => {
    const wrapper = mount(<RequestLoader id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<RequestLoader className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<RequestLoader style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
