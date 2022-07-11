import { mount, shallow } from "enzyme";
import React from "react";

import Submenu from "./";
import { testData, testStartSelect } from "./data";

const props = {
  data: testData,
  startSelect: testStartSelect,
};

const onlyData = {
  data: testData,
};

describe("<Submenu />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Submenu {...props} />);
    expect(wrapper).toExist(true);
  });

  it("gets data prop", () => {
    const wrapper = mount(<Submenu {...onlyData} />);
    expect(wrapper.prop("data")).toEqual(testData);
  });

  it("doesnt render without data prop", () => {
    const wrapper = mount(<Submenu {...props} />);
    expect(wrapper).toExist(false);
  });

  it("gets startSelect prop", () => {
    const wrapper = mount(<Submenu {...props} />);
    expect(wrapper.prop("startSelect")).toEqual(testStartSelect);
  });

  it("selects first data item as currentItem without startSelect prop", () => {
    const wrapper = shallow(<Submenu {...onlyData} />)
      .find("styled-submenu__StyledSubmenuContentWrapper")
      .childAt(0);
    const currentItemWrapper = shallow(testData[0].content);
    expect(wrapper.debug()).toEqual(currentItemWrapper.debug());
  });
});
