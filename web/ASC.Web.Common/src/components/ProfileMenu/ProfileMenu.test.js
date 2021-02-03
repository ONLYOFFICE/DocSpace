import React from "react";
import { mount } from "enzyme";
import ProfileMenu from ".";

const baseProps = {
  avatarRole: "admin",
  avatarSource: "",
  displayName: "Jane Doe",
  email: "janedoe@gmail.com",
};

describe("<Layout />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ProfileMenu {...baseProps} />);

    expect(wrapper).toExist();
  });
});
