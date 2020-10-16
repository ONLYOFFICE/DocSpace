import React from "react";
import { mount } from "enzyme";
import ErrorContainer from ".";

describe("<ErrorContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ErrorContainer />);

    expect(wrapper).toExist();
  });

  it("renders with props", () => {
    const wrapper = mount(
      <ErrorContainer
        headerText="Some error has happened"
        bodyText="Try again later"
        buttonText="Go back"
        buttonUrl="/page"
      />
    );

    expect(wrapper).toExist();
    expect(wrapper.prop("headerText")).toEqual("Some error has happened");
    expect(wrapper.prop("bodyText")).toEqual("Try again later");
    expect(wrapper.prop("buttonText")).toEqual("Go back");
    expect(wrapper.prop("buttonUrl")).toEqual("/page");
  });

  it("accepts id", () => {
    const wrapper = mount(<ErrorContainer id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<ErrorContainer className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<ErrorContainer style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
