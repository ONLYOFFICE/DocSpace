import React from "react";
import { mount } from "enzyme";
import FieldContainer from ".";
import TextInput from "../text-input";

describe("<FieldContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <FieldContainer labelText="Name:">
        <TextInput value="" onChange={(e) => console.log(e.target.value)} />
      </FieldContainer>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<FieldContainer id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<FieldContainer className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<FieldContainer style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
