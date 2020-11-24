import React from "react";
import { mount } from "enzyme";
import Toast from ".";
import toastr from "./toastr";

describe("<Textarea />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Toast>{toastr.success("Some text for toast")}</Toast>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Toast id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Toast className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });
});
