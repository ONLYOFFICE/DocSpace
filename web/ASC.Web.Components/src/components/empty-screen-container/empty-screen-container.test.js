import React from "react";
import { mount } from "enzyme";
import EmptyScreenContainer from ".";

const baseProps = {
  imageSrc: "empty_screen_filter.png",
  imageAlt: "Empty Screen Filter image",
  headerText: "No results matching your search could be found",
  descriptionText: "No results matching your search could be found",
  buttons: <a href="/">Go to home</a>,
};

describe("<EmptyScreenContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(<EmptyScreenContainer {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<EmptyScreenContainer {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <EmptyScreenContainer {...baseProps} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <EmptyScreenContainer {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
