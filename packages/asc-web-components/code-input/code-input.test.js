import React from "react";
import { mount } from "enzyme";
import CodeInput from ".";

describe("<CodeInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<CodeInput onSubmit={() => console.log("code")} />);

    expect(wrapper).toExist();
  });
});
