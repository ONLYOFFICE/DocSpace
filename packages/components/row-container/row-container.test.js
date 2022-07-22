import React from "react";
import { mount, shallow } from "enzyme";
import RowContainer from ".";

const baseProps = {
  manualHeight: "500px",
};

describe("<RowContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <RowContainer {...baseProps}>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });

  it("stop event on context click", () => {
    const wrapper = shallow(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>
    );

    const event = { preventDefault: () => {} };

    jest.spyOn(event, "preventDefault");

    wrapper.simulate("contextmenu", event);

    expect(event.preventDefault).not.toBeCalled();
  });

  it("renders like list", () => {
    const wrapper = mount(
      <RowContainer useReactWindow={false}>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper).toExist();
    expect(wrapper.getDOMNode().style).toHaveProperty("height", "");
  });

  it("renders without manualHeight", () => {
    const wrapper = mount(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });

  it("render with normal rows", () => {
    const wrapper = mount(
      <RowContainer {...baseProps}>
        <div contextOptions={[{ key: "1", label: "test" }]}>test</div>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <RowContainer {...baseProps} id="testId">
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <RowContainer {...baseProps} className="test">
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <RowContainer {...baseProps} style={{ color: "red" }}>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
