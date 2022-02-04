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
    const wrapper = mount(<Submenu />);
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

  // TODO : Develop a test that will monitor content after tab clicks
  // it("changes currentItem after clicking on SubmenuItem", () => {
  //   let wrapper = shallow(<Submenu data={testData} startSelect={1} />);
  //   const tabs = wrapper.find("styled-submenu__StyledSubmenuItem");
  //   console.log(wrapper.debug());
  //   tabs.forEach((t, i = 0) => {
  //     console.log("\n\n --- ITERATION", i, " ---\n");
  //     console.log(t.debug());

  //     const text = mount();
  //     t.simulate("click", { target: { text } });

  //     const newDisplayedContent = wrapper
  //       .update()
  //       .find("styled-submenu__StyledSubmenuContentWrapper")
  //       .childAt(0);

  //     const newCurrentItem = mount(testData[i].content);
  //     i++;

  //     console.log(
  //       "\nCONTENT ON THE PAGE - \n",
  //       newDisplayedContent.debug(),
  //       "\n",
  //       "\nCONTENT FROM THE DATA - \n",
  //       newCurrentItem.debug(),
  //       "\n\n\n"
  //     );
  //     expect(newDisplayedContent.debug()).toEqual(newCurrentItem.debug());
  //   });
  // });
});
