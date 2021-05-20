import React from "react";
import { mount } from "enzyme";
import TabContainer from ".";

const array_items = [
  {
    key: "tab0",
    title: "Title1",
    content: (
      <div>
        <button>BUTTON</button>
        <button>BUTTON</button>
        <button>BUTTON</button>
      </div>
    ),
  },
  {
    key: "tab1",
    title: "Title2",
    content: (
      <div>
        <label>LABEL</label>
        <label>LABEL</label>
        <label>LABEL</label>
      </div>
    ),
  },
  {
    key: "tab2",
    title: "Title3",
    content: (
      <div>
        <input></input>
        <input></input>
        <input></input>
      </div>
    ),
  },
  {
    key: "tab3",
    title: "Title4",
    content: (
      <div>
        <button>BUTTON</button>
        <button>BUTTON</button>
        <button>BUTTON</button>
      </div>
    ),
  },
  {
    key: "tab4",
    title: "Title5",
    content: (
      <div>
        <label>LABEL</label>
        <label>LABEL</label>
        <label>LABEL</label>
      </div>
    ),
  },
];

describe("<TabContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <TabContainer
        elements={[
          {
            key: "0",
            title: "Title1",
            content: (
              <div>
                <>
                  <button>BUTTON</button>
                </>
                <>
                  <button>BUTTON</button>
                </>
                <>
                  <button>BUTTON</button>
                </>
              </div>
            ),
          },
        ]}
      />
    );
    expect(wrapper).toExist();
  });

  it("TabsContainer not re-render test", () => {
    const wrapper = mount(<TabContainer elements={array_items} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );
    expect(shouldUpdate).toBe(false);
  });

  it("TabsContainer not re-render test", () => {
    const wrapper = mount(<TabContainer elements={array_items} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, {
      ...wrapper.state,
      activeTab: 3,
    });
    expect(shouldUpdate).toBe(true);
  });
});
