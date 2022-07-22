import React from "react";
import { mount } from "enzyme";
import Paging from ".";

const baseProps = {
  previousLabel: "Previous",
  nextLabel: "Next",
  selectedPageItem: { label: "1 of 1" },
  selectedCountItem: { label: "25 per page" },
  openDirection: "bottom",
};

describe("<Paging />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Paging {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Paging {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Paging {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Paging {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
