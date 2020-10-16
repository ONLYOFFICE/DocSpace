import React from "react";
import { mount } from "enzyme";
import MainButton from ".";
import Button from "../button";

describe("<MainButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <MainButton text="Button" isDisabled={false} isDropdown={true}>
        <div>Some button</div>
        <Button label="Some button" />
      </MainButton>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <MainButton
        text="Button"
        isDisabled={false}
        isDropdown={true}
        id="testId"
      >
        <div>Some button</div>
        <Button label="Some button" />
      </MainButton>
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <MainButton
        text="Button"
        isDisabled={false}
        isDropdown={true}
        className="test"
      >
        <div>Some button</div>
        <Button label="Some button" />
      </MainButton>
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <MainButton
        text="Button"
        isDisabled={false}
        isDropdown={true}
        style={{ color: "red" }}
      >
        <div>Some button</div>
        <Button label="Some button" />
      </MainButton>
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
